using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Extractify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToScrapedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ScrapedData",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ScrapedData");
        }
    }
}
