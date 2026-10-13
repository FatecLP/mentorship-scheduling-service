using MentorshipScheduling.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorshipScheduling.Infrastructure.Persistence.Configurations;

public sealed class MentorshipEntityConfiguration : IEntityTypeConfiguration<MentorshipEntity>
{
    public void Configure(EntityTypeBuilder<MentorshipEntity> builder)
    {
        builder.ToTable("mentorships", table => table.ExcludeFromMigrations());
        builder.HasKey(mentorship => mentorship.Id);

        builder.Property(mentorship => mentorship.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(mentorship => mentorship.StudentId)
            .HasColumnName("student_id")
            .IsRequired();

        builder.Property(mentorship => mentorship.TeacherId)
            .HasColumnName("teacher_id")
            .IsRequired();

        builder.Property(mentorship => mentorship.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(255)")
            .IsRequired();
    }
}
