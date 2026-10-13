using MentorshipScheduling.Domain.Entities;
using MentorshipScheduling.Domain.Enums;

namespace MentorshipScheduling.Infrastructure.Persistence.Entities;

public sealed class LessonScheduleEntity
{
    public long Id { get; set; }

    public long MentorshipId { get; set; }

    public DateTime StartAtUtc { get; set; }

    public DateTime EndAtUtc { get; set; }

    public LessonFormat Format { get; set; }

    public LessonScheduleStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public MentorshipEntity Mentorship { get; set; } = null!;

    public static LessonScheduleEntity FromDomain(LessonSchedule schedule) =>
        new()
        {
            Id = schedule.Id,
            MentorshipId = schedule.MentorshipId,
            StartAtUtc = schedule.StartAt.UtcDateTime,
            EndAtUtc = schedule.EndAt.UtcDateTime,
            Format = schedule.Format,
            Status = schedule.Status,
            CreatedAtUtc = schedule.CreatedAt.UtcDateTime,
            UpdatedAtUtc = schedule.UpdatedAt.UtcDateTime
        };

    public LessonSchedule ToDomain() =>
        new(
            Id,
            MentorshipId,
            new DateTimeOffset(DateTime.SpecifyKind(StartAtUtc, DateTimeKind.Utc)),
            new DateTimeOffset(DateTime.SpecifyKind(EndAtUtc, DateTimeKind.Utc)),
            Format,
            Status,
            new DateTimeOffset(DateTime.SpecifyKind(CreatedAtUtc, DateTimeKind.Utc)),
            new DateTimeOffset(DateTime.SpecifyKind(UpdatedAtUtc, DateTimeKind.Utc)));
}
