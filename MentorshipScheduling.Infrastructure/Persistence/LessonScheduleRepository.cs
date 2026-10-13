using System.Data.Common;
using System.Globalization;
using MentorshipScheduling.Domain.Entities;
using MentorshipScheduling.Domain.Enums;
using MentorshipScheduling.Domain.Repositories;
using MentorshipScheduling.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MentorshipScheduling.Infrastructure.Persistence;

public sealed class LessonScheduleRepository(MentorshipSchedulingDbContext dbContext)
    : ILessonScheduleRepository
{
    private const int NamedLockTimeoutSeconds = 10;

    public async Task<LessonSchedule?> GetByIdAsync(long id)
    {
        var entity = await dbContext.LessonSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(schedule => schedule.Id == id);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyList<LessonSchedule>> GetAllAsync(
        long? mentorshipId,
        long? studentId,
        long? teacherId)
    {
        var query = dbContext.LessonSchedules
            .AsNoTracking()
            .Where(schedule =>
                studentId.HasValue
                    ? schedule.Mentorship.StudentId == studentId.Value
                    : schedule.Mentorship.TeacherId == teacherId.GetValueOrDefault());

        if (mentorshipId.HasValue)
        {
            query = query.Where(schedule => schedule.MentorshipId == mentorshipId.Value);
        }

        var schedules = await query
            .OrderBy(schedule => schedule.StartAtUtc)
            .ToListAsync();

        return schedules.Select(schedule => schedule.ToDomain()).ToArray();
    }

    public async Task<Mentorship?> GetMentorshipByIdAsync(long mentorshipId)
    {
        var mentorship = await dbContext.Mentorships
            .AsNoTracking()
            .Where(entity => entity.Id == mentorshipId)
            .Select(entity => new
            {
                entity.Id,
                entity.StudentId,
                entity.TeacherId,
                entity.Status
            })
            .FirstOrDefaultAsync();

        return mentorship is null
            ? null
            : new Mentorship(
                mentorship.Id,
                mentorship.StudentId,
                mentorship.TeacherId,
                mentorship.Status);
    }

    public async Task<LessonSchedule?> AddIfNoConflictAsync(
        LessonSchedule lessonSchedule,
        long studentId,
        long teacherId)
    {
        var lockNames = GetLockNames(studentId, teacherId);
        await dbContext.Database.OpenConnectionAsync();

        try
        {
            await AcquireLocksAsync(lockNames);

            if (await HasConflictAsync(lessonSchedule, studentId, teacherId))
            {
                return null;
            }

            var entity = LessonScheduleEntity.FromDomain(lessonSchedule);
            dbContext.LessonSchedules.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity.ToDomain();
        }
        finally
        {
            try
            {
                await ReleaseLocksAsync(lockNames);
            }
            finally
            {
                await dbContext.Database.CloseConnectionAsync();
            }
        }
    }

    public async Task<ScheduleUpdateResult> UpdateIfNoConflictAsync(
        LessonSchedule lessonSchedule,
        LessonScheduleStatus expectedStatus,
        long studentId,
        long teacherId)
    {
        var lockNames = GetLockNames(studentId, teacherId);
        await dbContext.Database.OpenConnectionAsync();

        try
        {
            await AcquireLocksAsync(lockNames);

            if (await HasConflictAsync(lessonSchedule, studentId, teacherId))
            {
                return ScheduleUpdateResult.ScheduleConflict;
            }

            var updatedRows = await dbContext.LessonSchedules
                .Where(schedule =>
                    schedule.Id == lessonSchedule.Id &&
                    schedule.Status == expectedStatus)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            schedule => schedule.StartAtUtc,
                            lessonSchedule.StartAt.UtcDateTime)
                        .SetProperty(
                            schedule => schedule.EndAtUtc,
                            lessonSchedule.EndAt.UtcDateTime)
                        .SetProperty(schedule => schedule.Format, lessonSchedule.Format)
                        .SetProperty(schedule => schedule.Status, lessonSchedule.Status)
                        .SetProperty(
                            schedule => schedule.UpdatedAtUtc,
                            lessonSchedule.UpdatedAt.UtcDateTime));

            return updatedRows == 1
                ? ScheduleUpdateResult.Updated
                : ScheduleUpdateResult.StatusChangedOrNotFound;
        }
        finally
        {
            try
            {
                await ReleaseLocksAsync(lockNames);
            }
            finally
            {
                await dbContext.Database.CloseConnectionAsync();
            }
        }
    }

    private Task<bool> HasConflictAsync(
        LessonSchedule lessonSchedule,
        long studentId,
        long teacherId)
    {
        if (lessonSchedule.Status is not (
            LessonScheduleStatus.PendingApproval or LessonScheduleStatus.Approved))
        {
            return Task.FromResult(false);
        }

        return dbContext.LessonSchedules
            .AsNoTracking()
            .AnyAsync(schedule =>
                schedule.Id != lessonSchedule.Id &&
                (schedule.Status == LessonScheduleStatus.PendingApproval ||
                 schedule.Status == LessonScheduleStatus.Approved) &&
                schedule.StartAtUtc < lessonSchedule.EndAt.UtcDateTime &&
                schedule.EndAtUtc > lessonSchedule.StartAt.UtcDateTime &&
                (schedule.Mentorship.StudentId == studentId ||
                 schedule.Mentorship.TeacherId == teacherId));
    }

    private static IReadOnlyList<string> GetLockNames(long studentId, long teacherId) =>
        new[]
        {
            $"lesson-schedule:student:{studentId}",
            $"lesson-schedule:teacher:{teacherId}"
        }
        .Order(StringComparer.Ordinal)
        .ToArray();

    private async Task AcquireLocksAsync(IReadOnlyList<string> lockNames)
    {
        var acquiredLocks = new List<string>(lockNames.Count);
        foreach (var lockName in lockNames)
        {
            var acquired = await AcquireLockAsync(lockName);
            if (acquired != 1)
            {
                await ReleaseLocksAsync(acquiredLocks);
                throw new TimeoutException(
                    "Não foi possível reservar os participantes do agendamento. Tente novamente.");
            }

            acquiredLocks.Add(lockName);
        }
    }

    private async Task ReleaseLocksAsync(IReadOnlyList<string> lockNames)
    {
        for (var index = lockNames.Count - 1; index >= 0; index--)
        {
            await ReleaseLockAsync(lockNames[index]);
        }
    }

    private Task<int> AcquireLockAsync(string lockName) =>
        ExecuteLockCommandAsync(
            "SELECT GET_LOCK(@lockName, @timeoutSeconds)",
            lockName,
            NamedLockTimeoutSeconds);

    private Task<int> ReleaseLockAsync(string lockName) =>
        ExecuteLockCommandAsync("SELECT RELEASE_LOCK(@lockName)", lockName, null);

    private async Task<int> ExecuteLockCommandAsync(
        string sql,
        string lockName,
        int? timeoutSeconds)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;

        var nameParameter = command.CreateParameter();
        nameParameter.ParameterName = "@lockName";
        nameParameter.Value = lockName;
        command.Parameters.Add(nameParameter);

        if (timeoutSeconds.HasValue)
        {
            var timeoutParameter = command.CreateParameter();
            timeoutParameter.ParameterName = "@timeoutSeconds";
            timeoutParameter.Value = timeoutSeconds.Value;
            command.Parameters.Add(timeoutParameter);
        }

        var result = await command.ExecuteScalarAsync();
        return result is null or DBNull
            ? 0
            : Convert.ToInt32(result, CultureInfo.InvariantCulture);
    }
}
