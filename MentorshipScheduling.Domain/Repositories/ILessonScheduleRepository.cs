using MentorshipScheduling.Domain.Entities;
using MentorshipScheduling.Domain.Enums;

namespace MentorshipScheduling.Domain.Repositories;

public interface ILessonScheduleRepository
{
    Task<LessonSchedule?> GetByIdAsync(long id);

    Task<IReadOnlyList<LessonSchedule>> GetAllAsync(
        long? mentorshipId,
        long? studentId,
        long? teacherId);

    Task<Mentorship?> GetMentorshipByIdAsync(long mentorshipId);

    Task<LessonSchedule?> AddIfNoConflictAsync(
        LessonSchedule lessonSchedule,
        long studentId,
        long teacherId);

    Task<ScheduleUpdateResult> UpdateIfNoConflictAsync(
        LessonSchedule lessonSchedule,
        LessonScheduleStatus expectedStatus,
        long studentId,
        long teacherId);
}
