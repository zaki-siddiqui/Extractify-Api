using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Extractify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageSelectorInScraoingTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageSelector",
                table: "ScrapingTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageSelector",
                table: "ScrapingTasks");
        }
    }
}
