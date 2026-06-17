using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nexora.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccountData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "accounts",
                columns: new[] { "Id", "Balance", "UserId" },
                values: new object[,]
                {
                    { 1, 1000m, 1 },
                    { 2, 2000m, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "accounts",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
