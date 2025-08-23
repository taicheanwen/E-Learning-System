using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assg.Migrations
{
    /// <inheritdoc />
    public partial class IsHidden1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "ChapterContent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "ChapterContent",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
