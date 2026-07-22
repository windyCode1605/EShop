using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                schema: "dbo",
                table: "ProductImage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductVariantId",
                schema: "dbo",
                table: "ProductImage",
                column: "ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImage_ProductVariant_ProductVariantId",
                schema: "dbo",
                table: "ProductImage",
                column: "ProductVariantId",
                principalSchema: "dbo",
                principalTable: "ProductVariant",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImage_ProductVariant_ProductVariantId",
                schema: "dbo",
                table: "ProductImage");

            migrationBuilder.DropIndex(
                name: "IX_ProductImage_ProductVariantId",
                schema: "dbo",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                schema: "dbo",
                table: "ProductImage");
        }
    }
}
