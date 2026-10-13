using MentorshipScheduling.Domain.Enums;

namespace MentorshipScheduling.Application.DTOs;

/// <summary>Dados para solicitar uma aula em data, horário e formato específicos.</summary>
public sealed record CreateLessonScheduleRequest
{
    public required long MentorshipId { get; init; }

    public required long StudentId { get; init; }

    public required DateTimeOffset StartAt { get; init; }

    public required DateTimeOffset EndAt { get; init; }

    public required LessonFormat Format { get; init; }
}
