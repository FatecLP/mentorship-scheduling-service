using MentorshipScheduling.Domain.Enums;

namespace MentorshipScheduling.Application.DTOs;

/// <summary>Dados do agendamento de aula retornados pela API.</summary>
public sealed record LessonScheduleResponse(
    long Id,
    long MentorshipId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    LessonFormat Format,
    LessonScheduleStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
