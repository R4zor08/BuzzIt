using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuzzIt.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnershipToPostsAndReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "Users" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
                    "Username" TEXT NOT NULL,
                    "PasswordHash" TEXT NOT NULL,
                    "Role" TEXT NOT NULL,
                    "CreatedAtUtc" TEXT NOT NULL
                );
                """);

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username" ON "Users" ("Username");
                """);

            migrationBuilder.Sql(
                """
                INSERT OR IGNORE INTO "Users" ("Id", "Username", "PasswordHash", "Role", "CreatedAtUtc")
                VALUES (1, '__legacy_buzzit__', '__invalid__', 'User', '2026-01-01T00:00:00.0000000');
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "Posts" ADD COLUMN "UserId" INTEGER NOT NULL DEFAULT 1;
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "Reminders" ADD COLUMN "UserId" INTEGER NOT NULL DEFAULT 1;
                """);

            migrationBuilder.Sql(
                """
                UPDATE "Posts" SET "UserId" = (
                    SELECT "Id" FROM "Users" WHERE "Username" <> '__legacy_buzzit__' ORDER BY "Id" LIMIT 1)
                WHERE EXISTS (SELECT 1 FROM "Users" WHERE "Username" <> '__legacy_buzzit__' LIMIT 1);
                UPDATE "Reminders" SET "UserId" = (
                    SELECT "Id" FROM "Users" WHERE "Username" <> '__legacy_buzzit__' ORDER BY "Id" LIMIT 1)
                WHERE EXISTS (SELECT 1 FROM "Users" WHERE "Username" <> '__legacy_buzzit__' LIMIT 1);
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_Posts_UserId" ON "Posts" ("UserId");
                CREATE INDEX IF NOT EXISTS "IX_Reminders_UserId" ON "Reminders" ("UserId");
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Reminders_UserId"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Posts_UserId"";");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reminders");
        }
    }
}
