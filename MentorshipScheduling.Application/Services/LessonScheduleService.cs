using Andras.Result;
using Andras.Result.Errors;
using FluentValidation;
using MentorshipScheduling.Application.DTOs;
using MentorshipScheduling.Domain.Entities;
using MentorshipScheduling.Domain.Enums;
using MentorshipScheduling.Domain.Repositories;

namespace MentorshipScheduling.Application.Services;

public sealed class LessonScheduleService(
    ILessonScheduleRepository repository,
    IValidator<CreateLessonScheduleRequest> createValidator,
    IValidator<UpdateLessonScheduleRequest> updateValidator) : ILessonScheduleService
{
    public async Task<Result<IReadOnlyList<LessonScheduleResponse>>> GetAllAsync(
        long? mentorshipId,
        long? studentId,
        long? teacherId)
    {
        var actorError = ValidateActor(studentId, teacherId);
        if (actorError is not null)
        {
            return Result<IReadOnlyList<LessonScheduleResponse>>.Failure(actorError);
        }

        if (mentorshipId is <= 0)
        {
            return Result<IReadOnlyList<LessonScheduleResponse>>.Failure(
                Error.Validation("schedule.invalid_mentorship_id", "O ID da mentoria deve ser maior que zero."));
        }

        var schedules = await repository.GetAllAsync(mentorshipId, studentId, teacherId);
        return Result<IReadOnlyList<LessonScheduleResponse>>.Success(schedules.Select(Map).ToArray());
    }

    public async Task<Result<LessonScheduleResponse>> GetByIdAsync(
        long id,
        long? studentId,
        long? teacherId)
    {
        if (id <= 0)
        {
            return InvalidId();
        }

        var actorError = ValidateActor(studentId, teacherId);
        if (actorError is not null)
        {
            return Result<LessonScheduleResponse>.Failure(actorError);
        }

        var schedule = await repository.GetByIdAsync(id);
        if (schedule is null)
        {
            return NotFound(id);
        }

        var participantError = await ValidateParticipantAsync(
            schedule.MentorshipId,
            studentId,
            teacherId);

        return participantError is not null
            ? Result<LessonScheduleResponse>.Failure(participantError)
            : Result<LessonScheduleResponse>.Success(Map(schedule));
    }

    public async Task<Result<LessonScheduleResponse>> CreateAsync(
        CreateLessonScheduleRequest request)
    {
        var validation = await createValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ValidationFailure<LessonScheduleResponse>(validation.Errors);
        }

        var mentorship = await repository.GetMentorshipByIdAsync(request.MentorshipId);
        if (mentorship is null)
        {
            return MentorshipNotFound();
        }

        if (mentorship.StudentId != request.StudentId)
        {
            return Forbidden<LessonScheduleResponse>(
                "O aluno informado não pertence à mentoria.");
        }

        if (!string.Equals(mentorship.Status, "ACCEPTED", StringComparison.OrdinalIgnoreCase))
        {
            return Result<LessonScheduleResponse>.Failure(
                Error.Conflict(
                    "schedule.mentorship_not_accepted",
                    "A solicitação de aula só pode ser criada para uma mentoria aceita."));
        }

        var now = DateTimeOffset.UtcNow;
        var schedule = new LessonSchedule(
            0,
            request.MentorshipId,
            request.StartAt,
            request.EndAt,
            request.Format,
            LessonScheduleStatus.PendingApproval,
            now,
            now);

        var createdSchedule = await repository.AddIfNoConflictAsync(
            schedule,
            mentorship.StudentId,
            mentorship.TeacherId);

        return createdSchedule is null
            ? ScheduleConflict()
            : Result<LessonScheduleResponse>.Success(Map(createdSchedule));
    }

    public async Task<Result<LessonScheduleResponse>> UpdateAsync(
        long id,
        UpdateLessonScheduleRequest request)
    {
        if (id <= 0)
        {
            return InvalidId();
        }

        var validation = await updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ValidationFailure<LessonScheduleResponse>(validation.Errors);
        }

        var schedule = await repository.GetByIdAsync(id);
        if (schedule is null)
        {
            return NotFound(id);
        }

        var mentorship = await repository.GetMentorshipByIdAsync(schedule.MentorshipId);
        if (mentorship is null)
        {
            return MentorshipNotFound();
        }

        if (mentorship.StudentId != request.StudentId)
        {
            return Forbidden<LessonScheduleResponse>(
                "O aluno informado não pertence à mentoria.");
        }

        if (!schedule.CanBeRescheduled)
        {
            return InvalidTransition<LessonScheduleResponse>(
                "Somente solicitações de aula aguardando aprovação podem ser alteradas.");
        }

        var updatedSchedule = schedule.Reschedule(
            request.StartAt,
            request.EndAt,
            request.Format,
            DateTimeOffset.UtcNow);

        return await PersistUpdateAsync(updatedSchedule, schedule.Status, mentorship);
    }

    public Task<Result<LessonScheduleResponse>> ApproveAsync(long id, long teacherId) =>
        ChangeStatusAsync(id, null, teacherId, LessonScheduleStatus.Approved);

    public Task<Result<LessonScheduleResponse>> RejectAsync(long id, long teacherId) =>
        ChangeStatusAsync(id, null, teacherId, LessonScheduleStatus.Rejected);

    public Task<Result<LessonScheduleResponse>> CancelAsync(
        long id,
        long? studentId,
        long? teacherId) =>
        ChangeStatusAsync(id, studentId, teacherId, LessonScheduleStatus.Cancelled);

    private async Task<Result<LessonScheduleResponse>> ChangeStatusAsync(
        long id,
        long? studentId,
        long? teacherId,
        LessonScheduleStatus targetStatus)
    {
        if (id <= 0)
        {
            return InvalidId();
        }

        var actorError = ValidateActor(studentId, teacherId);
        if (actorError is not null)
        {
            return Result<LessonScheduleResponse>.Failure(actorError);
        }

        var schedule = await repository.GetByIdAsync(id);
        if (schedule is null)
        {
            return NotFound(id);
        }

        var mentorship = await repository.GetMentorshipByIdAsync(schedule.MentorshipId);
        if (mentorship is null)
        {
            return MentorshipNotFound();
        }

        var participantError = ValidateParticipant(mentorship, studentId, teacherId);
        if (participantError is not null)
        {
            return Result<LessonScheduleResponse>.Failure(participantError);
        }

        if (!schedule.CanTransitionTo(targetStatus))
        {
            return InvalidTransition<LessonScheduleResponse>(
                GetInvalidTransitionMessage(schedule.Status, targetStatus));
        }

        var updatedSchedule = schedule.TransitionTo(targetStatus, DateTimeOffset.UtcNow);
        return await PersistUpdateAsync(updatedSchedule, schedule.Status, mentorship);
    }

    private async Task<Result<LessonScheduleResponse>> PersistUpdateAsync(
        LessonSchedule schedule,
        LessonScheduleStatus expectedStatus,
        Mentorship mentorship)
    {
        var updateResult = await repository.UpdateIfNoConflictAsync(
            schedule,
            expectedStatus,
            mentorship.StudentId,
            mentorship.TeacherId);

        return updateResult switch
        {
            ScheduleUpdateResult.Updated => Result<LessonScheduleResponse>.Success(Map(schedule)),
            ScheduleUpdateResult.ScheduleConflict => ScheduleConflict(),
            _ => InvalidTransition<LessonScheduleResponse>(
                "O agendamento foi alterado por outra operação. Atualize os dados e tente novamente.")
        };
    }

    private async Task<Error?> ValidateParticipantAsync(
        long mentorshipId,
        long? studentId,
        long? teacherId)
    {
        var mentorship = await repository.GetMentorshipByIdAsync(mentorshipId);
        return mentorship is null
            ? MentorshipNotFoundError()
            : ValidateParticipant(mentorship, studentId, teacherId);
    }

    private static Error? ValidateParticipant(
        Mentorship mentorship,
        long? studentId,
        long? teacherId)
    {
        var isParticipant = studentId.HasValue
            ? mentorship.StudentId == studentId.Value
            : mentorship.TeacherId == teacherId.GetValueOrDefault();

        return isParticipant
            ? null
            : Error.Forbidden(
                "schedule.actor_not_in_mentorship",
                "O ID informado não pertence a nenhum participante desta mentoria.");
    }

    private static Error? ValidateActor(long? studentId, long? teacherId)
    {
        var hasExactlyOneActor = studentId.HasValue != teacherId.HasValue;
        var hasPositiveId = (studentId is > 0) || (teacherId is > 0);

        return hasExactlyOneActor && hasPositiveId
            ? null
            : Error.Validation(
                "schedule.invalid_actor",
                "Informe somente um ID válido: studentId ou teacherId.");
    }

    private static Result<T> ValidationFailure<T>(
        IEnumerable<FluentValidation.Results.ValidationFailure> failures) =>
        Result<T>.Failure(Error.Validation(
            "schedule.validation_failed",
            "Um ou mais dados informados são inválidos.",
            failures.Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}").ToArray()));

    private static Result<LessonScheduleResponse> InvalidId() =>
        Result<LessonScheduleResponse>.Failure(
            Error.Validation("schedule.invalid_id", "O ID do agendamento deve ser maior que zero."));

    private static Result<LessonScheduleResponse> NotFound(long id) =>
        Result<LessonScheduleResponse>.Failure(
            Error.NotFound("schedule.not_found", $"O agendamento de aula com ID {id} não foi encontrado."));

    private static Error MentorshipNotFoundError() =>
        Error.NotFound("mentorship.not_found", "A mentoria vinculada não foi encontrada.");

    private static Result<LessonScheduleResponse> MentorshipNotFound() =>
        Result<LessonScheduleResponse>.Failure(MentorshipNotFoundError());

    private static Result<T> Forbidden<T>(string message) =>
        Result<T>.Failure(Error.Forbidden("schedule.actor_not_in_mentorship", message));

    private static Result<LessonScheduleResponse> ScheduleConflict() =>
        Result<LessonScheduleResponse>.Failure(
            Error.Conflict(
                "schedule.time_conflict",
                "O aluno ou professor já possui um agendamento nesse horário."));

    private static Result<T> InvalidTransition<T>(string message) =>
        Result<T>.Failure(Error.Conflict("schedule.invalid_status_transition", message));

    private static string GetInvalidTransitionMessage(
        LessonScheduleStatus currentStatus,
        LessonScheduleStatus targetStatus)
    {
        if (targetStatus is LessonScheduleStatus.Approved or LessonScheduleStatus.Rejected &&
            currentStatus != LessonScheduleStatus.PendingApproval)
        {
            return "Somente solicitações aguardando aprovação podem ser aprovadas ou rejeitadas.";
        }

        if (targetStatus == LessonScheduleStatus.Cancelled)
        {
            return "Somente solicitações pendentes ou aulas aprovadas podem ser canceladas.";
        }

        return "A alteração solicitada não é permitida para o estado atual do agendamento.";
    }

    private static LessonScheduleResponse Map(LessonSchedule schedule) =>
        new(
            schedule.Id,
            schedule.MentorshipId,
            schedule.StartAt,
            schedule.EndAt,
            schedule.Format,
            schedule.Status,
            schedule.CreatedAt,
            schedule.UpdatedAt);
}
