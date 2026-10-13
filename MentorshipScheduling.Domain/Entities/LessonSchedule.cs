using MentorshipScheduling.Domain.Enums;

namespace MentorshipScheduling.Domain.Entities;

public sealed record LessonSchedule(
    long Id,
    long MentorshipId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    LessonFormat Format,
    LessonScheduleStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public bool CanBeRescheduled => Status == LessonScheduleStatus.PendingApproval;

    public bool CanTransitionTo(LessonScheduleStatus targetStatus) =>
        Status switch
        {
            LessonScheduleStatus.PendingApproval =>
                targetStatus is LessonScheduleStatus.Approved
                    or LessonScheduleStatus.Rejected
                    or LessonScheduleStatus.Cancelled,
            LessonScheduleStatus.Approved => targetStatus == LessonScheduleStatus.Cancelled,
            _ => false
        };

    public LessonSchedule Reschedule(
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        LessonFormat format,
        DateTimeOffset updatedAt) =>
        this with
        {
            StartAt = startAt,
            EndAt = endAt,
            Format = format,
            UpdatedAt = updatedAt
        };

    public LessonSchedule TransitionTo(
        LessonScheduleStatus status,
        DateTimeOffset updatedAt) =>
        this with
        {
            Status = status,
            UpdatedAt = updatedAt
        };
}
