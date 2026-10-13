using MentorshipScheduling.Domain.Enums;

namespace MentorshipScheduling.Application.DTOs;

/// <summary>Dados para alterar uma solicitação de aula que aguarda aprovação.</summary>
public sealed record UpdateLessonScheduleRequest
{
    public required long StudentId { get; init; }

    public required DateTimeOffset StartAt { get; init; }

    public required DateTimeOffset EndAt { get; init; }

    public required LessonFormat Format { get; init; }
}
