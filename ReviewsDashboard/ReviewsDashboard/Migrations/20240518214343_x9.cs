using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewsDashboard.Migrations
{
    /// <inheritdoc />
    public partial class x9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "businessReviews");

            migrationBuilder.AddColumn<int>(
                name: "Week1",
                table: "business",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Week2",
                table: "business",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Week3",
                table: "business",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Week4",
                table: "business",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Week1",
                table: "business");

            migrationBuilder.DropColumn(
                name: "Week2",
                table: "business");

            migrationBuilder.DropColumn(
                name: "Week3",
                table: "business");

            migrationBuilder.DropColumn(
                name: "Week4",
                table: "business");

            migrationBuilder.CreateTable(
                name: "businessReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewDateTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrapingDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_businessReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_businessReviews_business_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "business",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_businessReviews_BusinessId",
                table: "businessReviews",
                column: "BusinessId");
        }
    }
}
