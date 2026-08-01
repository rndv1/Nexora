using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Database.Migrations
{
    /// <inheritdoc />
    public partial class MultiCurrencyAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_accounts_user_id",
                table: "accounts");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "transactions",
                type: "text",
                nullable: false,
                defaultValue: "RUB");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "accounts",
                type: "text",
                nullable: false,
                defaultValue: "RUB");

            migrationBuilder.Sql("""
                                     INSERT INTO accounts (user_id, balance, currency)
                                     SELECT u.id, 0, 'USD'
                                     FROM users u
                                     WHERE NOT EXISTS (
                                         SELECT 1 FROM accounts a
                                         WHERE a.user_id = u.id AND a.currency = 'USD'
                                     );
                                 """);

            migrationBuilder.Sql("""
                                     INSERT INTO accounts (user_id, balance, currency)
                                     SELECT u.id, 0, 'EUR'
                                     FROM users u
                                     WHERE NOT EXISTS (
                                         SELECT 1 FROM accounts a
                                         WHERE a.user_id = u.id AND a.currency = 'EUR'
                                     );
                                 """);

            migrationBuilder.Sql("""
                                     INSERT INTO accounts (user_id, balance, currency)
                                     SELECT u.id, 0, 'GBP'
                                     FROM users u
                                     WHERE NOT EXISTS (
                                         SELECT 1 FROM accounts a
                                         WHERE a.user_id = u.id AND a.currency = 'GBP'
                                     );
                                 """);
            
            migrationBuilder.UpdateData(
                table: "accounts",
                keyColumn: "id",
                keyValue: 1,
                column: "currency",
                value: "RUB");

            migrationBuilder.UpdateData(
                table: "accounts",
                keyColumn: "id",
                keyValue: 2,
                column: "currency",
                value: "RUB");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_user_id_currency",
                table: "accounts",
                columns: new[] { "user_id", "currency" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_accounts_user_id_currency",
                table: "accounts");
            
            migrationBuilder.Sql("""
                                     DELETE FROM transactions t
                                     USING accounts a
                                     WHERE (t.sender_account_id = a.id OR t.receiver_account_id = a.id)
                                       AND a.currency <> 'RUB';
                                 """);

            migrationBuilder.Sql("""
                                     DELETE FROM accounts
                                     WHERE currency <> 'RUB';
                                 """);

            migrationBuilder.DropColumn(
                name: "currency",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "accounts");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_user_id",
                table: "accounts",
                column: "user_id",
                unique: true);
        }
    }
}
