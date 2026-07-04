using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class EAV_Schema_Refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_AttributeValue_AttributeId",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "ProductVariantAttribute",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "dbo",
                table: "ProductVariantAttribute",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                schema: "dbo",
                table: "ProductVariantAttribute",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                schema: "dbo",
                table: "ProductVariantAttribute",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "dbo",
                table: "ProductVariantAttribute",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "dbo",
                table: "ProductVariantAttribute",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "AttributeValue",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                schema: "dbo",
                table: "AttributeValue",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "dbo",
                table: "AttributeValue",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                schema: "dbo",
                table: "AttributeValue",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                schema: "dbo",
                table: "AttributeValue",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "dbo",
                table: "AttributeValue",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "dbo",
                table: "AttributeValue",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "dbo",
                table: "Attribute",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModifiedBy",
                schema: "dbo",
                table: "Attribute",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedDate",
                schema: "dbo",
                table: "Attribute",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Attribute",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Attribute",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "dbo",
                table: "Attribute",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVariantDefining",
                schema: "dbo",
                table: "Attribute",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CategoryAttribute",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryAttribute_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "dbo",
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryAttribute_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    AttributeValueId = table.Column<int>(type: "int", nullable: true),
                    CustomValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => x.Id);
                    table.CheckConstraint("CK_PA_ValueXor", "(AttributeValueId IS NOT NULL AND CustomValue IS NULL) OR (AttributeValueId IS NULL AND CustomValue IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ProductAttribute_AttributeValue_AttributeValueId",
                        column: x => x.AttributeValueId,
                        principalSchema: "dbo",
                        principalTable: "AttributeValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "dbo",
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId_AttributeId",
                schema: "dbo",
                table: "ProductVariantAttribute",
                columns: new[] { "ProductVariantId", "AttributeId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_PVA_ValueXor",
                schema: "dbo",
                table: "ProductVariantAttribute",
                sql: "(AttributeValueId IS NOT NULL AND CustomValue IS NULL) OR (AttributeValueId IS NULL AND CustomValue IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_AttributeId_Value",
                schema: "dbo",
                table: "AttributeValue",
                columns: new[] { "AttributeId", "Value" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Attribute_Type",
                schema: "dbo",
                table: "Attribute",
                sql: "AttributeType IN ('Text', 'Number', 'Color', 'Boolean')");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttribute_AttributeId",
                schema: "dbo",
                table: "CategoryAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttribute_CategoryId_AttributeId",
                schema: "dbo",
                table: "CategoryAttribute",
                columns: new[] { "CategoryId", "AttributeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_AttributeId",
                schema: "dbo",
                table: "ProductAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_AttributeValueId",
                schema: "dbo",
                table: "ProductAttribute",
                column: "AttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductId_AttributeId",
                schema: "dbo",
                table: "ProductAttribute",
                columns: new[] { "ProductId", "AttributeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryAttribute",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductAttribute",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId_AttributeId",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PVA_ValueXor",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_AttributeValue_AttributeId_Value",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Attribute_Type",
                schema: "dbo",
                table: "Attribute");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "dbo",
                table: "ProductVariantAttribute");

            migrationBuilder.DropColumn(
                name: "ColorHex",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "dbo",
                table: "AttributeValue");

            migrationBuilder.DropColumn(
                name: "IsVariantDefining",
                schema: "dbo",
                table: "Attribute");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "ProductVariantAttribute",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "AttributeValue",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "dbo",
                table: "Attribute",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                schema: "dbo",
                table: "Attribute",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedDate",
                schema: "dbo",
                table: "Attribute",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Attribute",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Attribute",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "Attribute",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId",
                schema: "dbo",
                table: "ProductVariantAttribute",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_AttributeId",
                schema: "dbo",
                table: "AttributeValue",
                column: "AttributeId");
        }
    }
}
