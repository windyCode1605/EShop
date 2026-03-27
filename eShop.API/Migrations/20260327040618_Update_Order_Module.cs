using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class Update_Order_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_AddressId",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariants_VariantId",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "core",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "stock",
                schema: "core",
                table: "Products",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "core",
                table: "OrderItems",
                newName: "ProductVariantsId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_AddressId",
                schema: "core",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductVariantsId");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                schema: "core",
                table: "Coupons",
                newName: "StartDate");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "core",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "core",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "core",
                table: "Reviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "core",
                table: "ProductVariants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "core",
                table: "ProductVariants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "core",
                table: "ProductVariants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "core",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "core",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "core",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "core",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                schema: "core",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "core",
                table: "OrderItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "core",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                schema: "core",
                table: "OrderItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                schema: "core",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "core",
                table: "OrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantName",
                schema: "core",
                table: "OrderItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinOrderValue",
                schema: "core",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                schema: "core",
                table: "Coupons",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "core",
                table: "Coupons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                schema: "core",
                table: "Coupons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "core",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "core",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountValue",
                schema: "core",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "core",
                table: "Coupons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageLimit",
                schema: "core",
                table: "Coupons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageLimitPerUser",
                schema: "core",
                table: "Coupons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsedCount",
                schema: "core",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "core",
                table: "Carts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "core",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "core",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                schema: "core",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                schema: "core",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                schema: "core",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantsId",
                schema: "core",
                table: "OrderItems",
                column: "ProductVariantsId",
                principalSchema: "core",
                principalTable: "ProductVariants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariants_VariantId",
                schema: "core",
                table: "OrderItems",
                column: "VariantId",
                principalSchema: "core",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                schema: "core",
                table: "Orders",
                column: "AddressId",
                principalSchema: "core",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantsId",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariants_VariantId",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                schema: "core",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "core");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressId",
                schema: "core",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "core",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "core",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "core",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "core",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "core",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "core",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "core",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                schema: "core",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Total",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VariantName",
                schema: "core",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "MaxDiscountValue",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UsageLimit",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UsageLimitPerUser",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UsedCount",
                schema: "core",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "core",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "core",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "Stock",
                schema: "core",
                table: "Products",
                newName: "stock");

            migrationBuilder.RenameColumn(
                name: "ProductVariantsId",
                schema: "core",
                table: "OrderItems",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductVariantsId",
                schema: "core",
                table: "OrderItems",
                newName: "IX_OrderItems_AddressId");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                schema: "core",
                table: "Coupons",
                newName: "ExpiresAt");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "core",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "core",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "core",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinOrderValue",
                schema: "core",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiscountType",
                schema: "core",
                table: "Coupons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                schema: "core",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_AddressId",
                schema: "core",
                table: "OrderItems",
                column: "AddressId",
                principalSchema: "core",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariants_VariantId",
                schema: "core",
                table: "OrderItems",
                column: "VariantId",
                principalSchema: "core",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
