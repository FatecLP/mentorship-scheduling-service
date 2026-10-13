using FluentValidation;
using MentorshipScheduling.Application.DTOs;

namespace MentorshipScheduling.Application.Validators;

public sealed class UpdateLessonScheduleRequestValidator
    : AbstractValidator<UpdateLessonScheduleRequest>
{
    public UpdateLessonScheduleRequestValidator()
    {
        RuleFor(request => request.StudentId)
            .GreaterThan(0)
            .WithMessage("O ID do aluno deve ser maior que zero.");
        RuleFor(request => request.StartAt)
            .Must(startAt => startAt > DateTimeOffset.UtcNow)
            .WithMessage("O início da aula deve estar no futuro.");
        RuleFor(request => request.EndAt)
            .GreaterThan(request => request.StartAt)
            .WithMessage("O término da aula deve ser posterior ao início.");
        RuleFor(request => request.Format)
            .IsInEnum()
            .WithMessage("O formato da aula informado é inválido.");
    }
}
