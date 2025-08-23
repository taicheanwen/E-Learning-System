using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assg.Migrations
{
    /// <inheritdoc />
    public partial class ChatQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId1",
                table: "ChatQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_Course_CourseId",
                table: "ChatQuestion");

            migrationBuilder.DropIndex(
                name: "IX_ChatQuestion_ChapterContentId1",
                table: "ChatQuestion");

            migrationBuilder.DropColumn(
                name: "ChapterContentId1",
                table: "ChatQuestion");

            migrationBuilder.AlterColumn<int>(
                name: "ChapterContentId",
                table: "ChatQuestion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserAccountId",
                table: "ChatAnswer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatAnswer_UserAccountId",
                table: "ChatAnswer",
                column: "UserAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatAnswer_UserAccount_UserAccountId",
                table: "ChatAnswer",
                column: "UserAccountId",
                principalTable: "UserAccount",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion",
                column: "ChapterContentId",
                principalTable: "ChapterContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_Course_CourseId",
                table: "ChatQuestion",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatAnswer_UserAccount_UserAccountId",
                table: "ChatAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatQuestion_Course_CourseId",
                table: "ChatQuestion");

            migrationBuilder.DropIndex(
                name: "IX_ChatAnswer_UserAccountId",
                table: "ChatAnswer");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "ChatAnswer");

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
                name: "ChapterContentId1",
                table: "ChatQuestion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatQuestion_ChapterContentId1",
                table: "ChatQuestion",
                column: "ChapterContentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId",
                table: "ChatQuestion",
                column: "ChapterContentId",
                principalTable: "ChapterContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_ChapterContent_ChapterContentId1",
                table: "ChatQuestion",
                column: "ChapterContentId1",
                principalTable: "ChapterContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatQuestion_Course_CourseId",
                table: "ChatQuestion",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
