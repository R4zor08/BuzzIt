using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuzzIt.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Reminders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Reminders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Reminders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Reminders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Reminders");
        }
    }
}
