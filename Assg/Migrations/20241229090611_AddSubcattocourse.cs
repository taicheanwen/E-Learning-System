using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assg.Migrations
{
    /// <inheritdoc />
    public partial class AddSubcattocourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Course",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "categoryName",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subcategoryId",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subcategoryName",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoryName",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "subcategoryId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "subcategoryName",
                table: "Course");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Course",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
