using MentorshipScheduling.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MentorshipScheduling.Infrastructure.Persistence;

public sealed class MentorshipSchedulingDbContext(
    DbContextOptions<MentorshipSchedulingDbContext> options) : DbContext(options)
{
    public DbSet<LessonScheduleEntity> LessonSchedules => Set<LessonScheduleEntity>();

    public DbSet<MentorshipEntity> Mentorships => Set<MentorshipEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MentorshipSchedulingDbContext).Assembly);
}
