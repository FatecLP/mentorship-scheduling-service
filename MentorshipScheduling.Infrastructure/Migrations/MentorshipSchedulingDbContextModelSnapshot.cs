using MentorshipScheduling.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MentorshipScheduling.Infrastructure.Migrations;

[DbContext(typeof(MentorshipSchedulingDbContext))]
public sealed class MentorshipSchedulingDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.9");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MentorshipSchedulingDbContext).Assembly);
    }
}
