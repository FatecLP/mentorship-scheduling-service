namespace MentorshipScheduling.Application.DTOs;

/// <summary>Identifica o aluno ou professor participante da mentoria.</summary>
public sealed record LessonScheduleActorRequest
{
    public long? StudentId { get; init; }

    public long? TeacherId { get; init; }
}
