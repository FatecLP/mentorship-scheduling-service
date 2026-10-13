using MentorshipScheduling.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorshipScheduling.Infrastructure.Migrations;

[DbContext(typeof(MentorshipSchedulingDbContext))]
[Migration("20261013010000_AddLessonScheduleTimeRange")]
public sealed class AddLessonScheduleTimeRange : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE `lesson_schedules`
                ADD COLUMN `start_at_utc` DATETIME(6) NULL,
                ADD COLUMN `end_at_utc` DATETIME(6) NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE `lesson_schedules`
            SET `start_at_utc` = `scheduled_at_utc`,
                `end_at_utc` = DATE_ADD(`scheduled_at_utc`, INTERVAL 1 HOUR);
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `lesson_schedules`
                MODIFY COLUMN `start_at_utc` DATETIME(6) NOT NULL,
                MODIFY COLUMN `end_at_utc` DATETIME(6) NOT NULL,
                DROP INDEX `ix_lesson_schedules_mentorship_scheduled_at`,
                DROP COLUMN `scheduled_at_utc`,
                ADD INDEX `ix_lesson_schedules_mentorship_time_range`
                    (`mentorship_id`, `start_at_utc`, `end_at_utc`),
                ADD INDEX `ix_lesson_schedules_status_time_range`
                    (`status`, `start_at_utc`, `end_at_utc`, `mentorship_id`);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE `lesson_schedules`
                ADD COLUMN `scheduled_at_utc` DATETIME(6) NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE `lesson_schedules`
            SET `scheduled_at_utc` = `start_at_utc`;
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE `lesson_schedules`
                MODIFY COLUMN `scheduled_at_utc` DATETIME(6) NOT NULL,
                DROP INDEX `ix_lesson_schedules_mentorship_time_range`,
                DROP INDEX `ix_lesson_schedules_status_time_range`,
                DROP COLUMN `end_at_utc`,
                DROP COLUMN `start_at_utc`,
                ADD INDEX `ix_lesson_schedules_mentorship_scheduled_at`
                    (`mentorship_id`, `scheduled_at_utc`);
            """);
    }
}
