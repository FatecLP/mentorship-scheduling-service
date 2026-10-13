using MentorshipScheduling.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorshipScheduling.Infrastructure.Migrations;

[DbContext(typeof(MentorshipSchedulingDbContext))]
[Migration("20261013000000_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.Sql(
            """
            CREATE TABLE `lesson_schedules` (
                `id` BIGINT NOT NULL AUTO_INCREMENT,
                `mentorship_id` BIGINT NOT NULL,
                `scheduled_at_utc` DATETIME(6) NOT NULL,
                `format` VARCHAR(20) NOT NULL,
                `status` VARCHAR(30) NOT NULL,
                `created_at_utc` DATETIME(6) NOT NULL,
                `updated_at_utc` DATETIME(6) NOT NULL,
                CONSTRAINT `pk_lesson_schedules` PRIMARY KEY (`id`),
                INDEX `ix_lesson_schedules_mentorship_scheduled_at` (`mentorship_id`, `scheduled_at_utc`),
                CONSTRAINT `fk_lesson_schedules_mentorships_mentorship_id`
                    FOREIGN KEY (`mentorship_id`) REFERENCES `mentorships` (`id`)
                    ON DELETE RESTRICT
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            """);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.Sql("DROP TABLE `lesson_schedules`;");
}
