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
            migrationBuilder.Sql(
                "UPDATE users SET password_hash = 'ATn9zvX5QksEsVvWt0cjfQ==:4E+jtMuMQuEaDBKkA09KLD/Fe0A17AgV/zgJehhBJLw=' WHERE id = 1 AND password_hash = 'password123456';"
            );
            migrationBuilder.Sql(
                "UPDATE users SET password_hash = '/7BAthMALkzyCHQsoaFFXA==:7x7XdZMvXdK1JAAm0q0ZZcvkLcSTqL8EOuYjF5459KQ=' WHERE id = 2 AND password_hash = 'password';"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE users SET password_hash = 'password123456' WHERE id = 1 AND password_hash = 'ATn9zvX5QksEsVvWt0cjfQ==:4E+jtMuMQuEaDBKkA09KLD/Fe0A17AgV/zgJehhBJLw=';"
            );
            migrationBuilder.Sql(
                "UPDATE users SET password_hash = 'password' WHERE id = 2 AND password_hash = '/7BAthMALkzyCHQsoaFFXA==:7x7XdZMvXdK1JAAm0q0ZZcvkLcSTqL8EOuYjF5459KQ=';"
            );
        }
    }
}
