using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewsDashboard.Migrations
{
    /// <inheritdoc />
    public partial class x3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "review");

            migrationBuilder.DropColumn(
                name: "City",
                table: "review");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "review");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "review");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "review",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "ReviewLink",
                table: "review",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "review",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "review",
                newName: "ReviewLink");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "review",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "review",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "review",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "review",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
