using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSizeColorFromProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                schema: "dbo",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "Size",
                schema: "dbo",
                table: "ProductVariant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                schema: "dbo",
                table: "ProductVariant",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                schema: "dbo",
                table: "ProductVariant",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
