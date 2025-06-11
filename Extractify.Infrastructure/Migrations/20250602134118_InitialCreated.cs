using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Extractify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrapingTasks",
                columns: table => new
                {
                    ScrapingTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Selector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapingTasks", x => x.ScrapingTaskId);
                });

            migrationBuilder.CreateTable(
                name: "ScrapedData",
                columns: table => new
                {
                    ScrapedDataId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScrapingTaskId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScrapedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapedData", x => x.ScrapedDataId);
                    table.ForeignKey(
                        name: "FK_ScrapedData_ScrapingTasks_ScrapingTaskId",
                        column: x => x.ScrapingTaskId,
                        principalTable: "ScrapingTasks",
                        principalColumn: "ScrapingTaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapedData_ScrapingTaskId",
                table: "ScrapedData",
                column: "ScrapingTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrapedData");

            migrationBuilder.DropTable(
                name: "ScrapingTasks");
        }
    }
}
