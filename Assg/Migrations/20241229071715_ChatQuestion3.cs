using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assg.Migrations
{
    /// <inheritdoc />
    public partial class ChatQuestion3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ChatQuestion_UserId",
                table: "ChatQuestion",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_UserAccount_UserId",
                table: "ChatQuestion",
                column: "UserId",
                principalTable: "UserAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_UserAccount_UserId",
                table: "ChatQuestion");

            migrationBuilder.DropIndex(
                name: "IX_ChatQuestion_UserId",
                table: "ChatQuestion");
        }
    }
}
