using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assg.Migrations
{
    /// <inheritdoc />
    public partial class ChatQuestion1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion");

            migrationBuilder.AlterColumn<int>(
                name: "ChapterContentId",
                table: "ChatQuestion",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion",
                column: "ChapterContentId",
                principalTable: "ChapterContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_UserAccount_UserAccountId",
                table: "ChatQuestion",
                column: "UserAccountId",
                principalTable: "UserAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_UserAccount_UserAccountId",
                table: "ChatQuestion");

            migrationBuilder.DropIndex(
                name: "IX_ChatQuestion_UserAccountId",
                table: "ChatQuestion");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "ChatQuestion");

            migrationBuilder.AlterColumn<int>(
                name: "ChapterContentId",
                table: "ChatQuestion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion",
                column: "ChapterContentId",
                principalTable: "ChapterContent",
                principalColumn: "Id");
        }
    }
}
