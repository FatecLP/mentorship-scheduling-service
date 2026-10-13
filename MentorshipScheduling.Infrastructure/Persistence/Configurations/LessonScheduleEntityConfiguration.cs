using MentorshipScheduling.Domain.Enums;
using MentorshipScheduling.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorshipScheduling.Infrastructure.Persistence.Configurations;

public sealed class LessonScheduleEntityConfiguration : IEntityTypeConfiguration<LessonScheduleEntity>
{
    public void Configure(EntityTypeBuilder<LessonScheduleEntity> builder)
    {
        builder.ToTable("lesson_schedules");
        builder.HasKey(schedule => schedule.Id);

        builder.Property(schedule => schedule.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(schedule => schedule.MentorshipId)
            .HasColumnName("mentorship_id")
            .IsRequired();

        builder.Property(schedule => schedule.StartAtUtc)
            .HasColumnName("start_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(schedule => schedule.EndAtUtc)
            .HasColumnName("end_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(schedule => schedule.Format)
            .HasColumnName("format")
            .HasConversion<string>()
            .HasColumnType("varchar(20)")
            .IsRequired();

        builder.Property(schedule => schedule.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasColumnType("varchar(30)")
            .IsRequired();

        builder.Property(schedule => schedule.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(schedule => schedule.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasOne(schedule => schedule.Mentorship)
            .WithMany()
            .HasForeignKey(schedule => schedule.MentorshipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(schedule => new
        {
            schedule.MentorshipId,
            schedule.StartAtUtc,
            schedule.EndAtUtc
        })
            .HasDatabaseName("ix_lesson_schedules_mentorship_time_range");

        builder.HasIndex(schedule => new
        {
            schedule.Status,
            schedule.StartAtUtc,
            schedule.EndAtUtc,
            schedule.MentorshipId
        })
            .HasDatabaseName("ix_lesson_schedules_status_time_range");
    }
}
