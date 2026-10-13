namespace MentorshipScheduling.Domain.Entities;

public sealed record Mentorship(
    long Id,
    long StudentId,
    long TeacherId,
    string Status);
