using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "password123456");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "admin");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "user");
        }
    }
}
