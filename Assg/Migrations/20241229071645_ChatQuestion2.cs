using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assg.Migrations
{
    /// <inheritdoc />
    public partial class ChatQuestion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_UserAccount_UserAccountId",
                table: "ChatQuestion");

            migrationBuilder.DropIndex(
                name: "IX_ChatQuestion_UserAccountId",
                table: "ChatQuestion");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "ChatQuestion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserAccountId",
                table: "ChatQuestion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatQuestion_UserAccountId",
                table: "ChatQuestion",
                column: "UserAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_UserAccount_UserAccountId",
                table: "ChatQuestion",
                column: "UserAccountId",
                principalTable: "UserAccount",
                principalColumn: "Id");
        }
    }
}
