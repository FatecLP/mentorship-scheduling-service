using Andras.Result;
using MentorshipScheduling.Application.DTOs;

namespace MentorshipScheduling.Application.Services;

public interface ILessonScheduleService
{
    Task<Result<IReadOnlyList<LessonScheduleResponse>>> GetAllAsync(
        long? mentorshipId,
        long? studentId,
        long? teacherId);

    Task<Result<LessonScheduleResponse>> GetByIdAsync(
        long id,
        long? studentId,
        long? teacherId);

    Task<Result<LessonScheduleResponse>> CreateAsync(CreateLessonScheduleRequest request);

    Task<Result<LessonScheduleResponse>> UpdateAsync(
        long id,
        UpdateLessonScheduleRequest request);

    Task<Result<LessonScheduleResponse>> ApproveAsync(long id, long teacherId);

    Task<Result<LessonScheduleResponse>> RejectAsync(long id, long teacherId);

    Task<Result<LessonScheduleResponse>> CancelAsync(
        long id,
        long? studentId,
        long? teacherId);
}
