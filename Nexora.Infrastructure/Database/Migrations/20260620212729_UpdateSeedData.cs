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
                value: "ATn9zvX5QksEsVvWt0cjfQ==:4E+jtMuMQuEaDBKkA09KLD/Fe0A17AgV/zgJehhBJLw=");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "/7BAthMALkzyCHQsoaFFXA==:7x7XdZMvXdK1JAAm0q0ZZcvkLcSTqL8EOuYjF5459KQ=");
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
