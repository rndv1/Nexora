using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeededPasswordsToHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "password_hash",
                value: "ATn9zvX5QksEsVvWt0cjfQ==:4E+jtMuMQuEaDBKkA09KLD/Fe0A17AgV/zgJehhBJLw=");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 2,
                column: "password_hash",
                value: "/7BAthMALkzyCHQsoaFFXA==:7x7XdZMvXdK1JAAm0q0ZZcvkLcSTqL8EOuYjF5459KQ=");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1,
                column: "password_hash",
                value: "password123456");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 2,
                column: "password_hash",
                value: "password");
        }
    }
}
