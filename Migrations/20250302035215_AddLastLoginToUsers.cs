using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodoyCordoba.Migrations
{
    /// <inheritdoc />
    public partial class AddLastLoginToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "LastLogin",
                table: "users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "users",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "users");
        }
    }
}
