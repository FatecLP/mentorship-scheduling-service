using MentorshipScheduling.Domain.Repositories;
using MentorshipScheduling.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MentorshipScheduling.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MentorshipDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'MentorshipDatabase' is not configured.");

        services.AddDbContext<MentorshipSchedulingDbContext>(options =>
            options.UseMySQL(connectionString));

        services.AddScoped<ILessonScheduleRepository, LessonScheduleRepository>();
        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MentorshipSchedulingDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
