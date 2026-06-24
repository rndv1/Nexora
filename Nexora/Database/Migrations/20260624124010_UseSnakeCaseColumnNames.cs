using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexora.Database.Migrations
{
    /// <inheritdoc />
    public partial class UseSnakeCaseColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_UserId",
                table: "accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_ReceiverAccountId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_SenderAccountId",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "users",
                newName: "login");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameIndex(
                name: "IX_users_Login",
                table: "users",
                newName: "IX_users_login");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "transactions",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transactions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SenderAccountId",
                table: "transactions",
                newName: "sender_account_id");

            migrationBuilder.RenameColumn(
                name: "ReceiverAccountId",
                table: "transactions",
                newName: "receiver_account_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "transactions",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_SenderAccountId",
                table: "transactions",
                newName: "IX_transactions_sender_account_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_ReceiverAccountId",
                table: "transactions",
                newName: "IX_transactions_receiver_account_id");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "sessions",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "sessions",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "sessions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "accounts",
                newName: "balance");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "accounts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "accounts",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_UserId",
                table: "accounts",
                newName: "IX_accounts_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_user_id",
                table: "accounts",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_users_user_id",
                table: "sessions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_receiver_account_id",
                table: "transactions",
                column: "receiver_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_sender_account_id",
                table: "transactions",
                column: "sender_account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_users_user_id",
                table: "accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_users_user_id",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_receiver_account_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_sender_account_id",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "login",
                table: "users",
                newName: "Login");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "IX_users_login",
                table: "users",
                newName: "IX_users_Login");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "transactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sender_account_id",
                table: "transactions",
                newName: "SenderAccountId");

            migrationBuilder.RenameColumn(
                name: "receiver_account_id",
                table: "transactions",
                newName: "ReceiverAccountId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "transactions",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_sender_account_id",
                table: "transactions",
                newName: "IX_transactions_SenderAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_receiver_account_id",
                table: "transactions",
                newName: "IX_transactions_ReceiverAccountId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "sessions",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "sessions",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "sessions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "balance",
                table: "accounts",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "accounts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "accounts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_user_id",
                table: "accounts",
                newName: "IX_accounts_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_users_UserId",
                table: "accounts",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_ReceiverAccountId",
                table: "transactions",
                column: "ReceiverAccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_SenderAccountId",
                table: "transactions",
                column: "SenderAccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
