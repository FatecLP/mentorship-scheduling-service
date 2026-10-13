namespace MentorshipScheduling.Infrastructure.Persistence.Entities;

public sealed class MentorshipEntity
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public long TeacherId { get; set; }

    public string Status { get; set; } = string.Empty;
}
