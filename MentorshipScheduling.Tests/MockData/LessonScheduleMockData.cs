using MentorshipScheduling.Application.DTOs;
using MentorshipScheduling.Domain.Enums;

namespace MentorshipScheduling.Tests.MockData;

public static class LessonScheduleMockData
{
    public static IReadOnlyList<LessonScheduleResponse> GetAll() =>
    [
        GetById(7),
        GetById(8)
    ];

    public static LessonScheduleResponse GetById(
        long id,
        LessonScheduleStatus status = LessonScheduleStatus.PendingApproval)
    {
        var startAt = new DateTimeOffset(2030, 10, 13, 14, 0, 0, TimeSpan.FromHours(-3));
        return new LessonScheduleResponse(
            id,
            12,
            startAt,
            startAt.AddHours(1),
            LessonFormat.Online,
            status,
            startAt.AddDays(-1),
            startAt.AddDays(-1));
    }

    public static CreateLessonScheduleRequest CreateRequest() =>
        new()
        {
            MentorshipId = 12,
            StudentId = 23,
            StartAt = new DateTimeOffset(2030, 10, 13, 14, 0, 0, TimeSpan.FromHours(-3)),
            EndAt = new DateTimeOffset(2030, 10, 13, 15, 0, 0, TimeSpan.FromHours(-3)),
            Format = LessonFormat.Online
        };

    public static UpdateLessonScheduleRequest UpdateRequest() =>
        new()
        {
            StudentId = 23,
            StartAt = new DateTimeOffset(2030, 10, 14, 14, 0, 0, TimeSpan.FromHours(-3)),
            EndAt = new DateTimeOffset(2030, 10, 14, 15, 0, 0, TimeSpan.FromHours(-3)),
            Format = LessonFormat.InPerson
        };
}
