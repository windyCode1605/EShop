using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveToPublicSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "dbo",
                newName: "Users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserRole",
                schema: "dbo",
                newName: "UserRole",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserProfile",
                schema: "dbo",
                newName: "UserProfile",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "SysVar",
                schema: "dbo",
                newName: "SysVar",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Shipment",
                schema: "dbo",
                newName: "Shipment",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "SendOtp",
                schema: "dbo",
                newName: "SendOtp",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "RolePermission",
                schema: "dbo",
                newName: "RolePermission",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Role",
                schema: "dbo",
                newName: "Role",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Reviews",
                schema: "dbo",
                newName: "Reviews",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ProductVariantAttribute",
                schema: "dbo",
                newName: "ProductVariantAttribute",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ProductVariant",
                schema: "dbo",
                newName: "ProductVariant",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ProductImage",
                schema: "dbo",
                newName: "ProductImage",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ProductAttribute",
                schema: "dbo",
                newName: "ProductAttribute",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Product",
                schema: "dbo",
                newName: "Product",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Permission",
                schema: "dbo",
                newName: "Permission",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "dbo",
                newName: "Payments",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OrderRefund",
                schema: "dbo",
                newName: "OrderRefund",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                schema: "dbo",
                newName: "OrderItem",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Order",
                schema: "dbo",
                newName: "Order",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "NotificationToken",
                schema: "dbo",
                newName: "NotificationToken",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "CouponUsage",
                schema: "dbo",
                newName: "CouponUsage",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Coupons",
                schema: "dbo",
                newName: "Coupons",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "CategoryAttribute",
                schema: "dbo",
                newName: "CategoryAttribute",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Category",
                schema: "dbo",
                newName: "Category",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "CartItem",
                schema: "dbo",
                newName: "CartItem",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Cart",
                schema: "dbo",
                newName: "Cart",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AuthOtp",
                schema: "dbo",
                newName: "AuthOtp",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AttributeValue",
                schema: "dbo",
                newName: "AttributeValue",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Attribute",
                schema: "dbo",
                newName: "Attribute",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "dbo",
                newName: "Addresses",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "public",
                newName: "Users",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserRole",
                schema: "public",
                newName: "UserRole",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserProfile",
                schema: "public",
                newName: "UserProfile",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SysVar",
                schema: "public",
                newName: "SysVar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Shipment",
                schema: "public",
                newName: "Shipment",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SendOtp",
                schema: "public",
                newName: "SendOtp",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "RolePermission",
                schema: "public",
                newName: "RolePermission",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Role",
                schema: "public",
                newName: "Role",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Reviews",
                schema: "public",
                newName: "Reviews",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ProductVariantAttribute",
                schema: "public",
                newName: "ProductVariantAttribute",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ProductVariant",
                schema: "public",
                newName: "ProductVariant",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ProductImage",
                schema: "public",
                newName: "ProductImage",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ProductAttribute",
                schema: "public",
                newName: "ProductAttribute",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Product",
                schema: "public",
                newName: "Product",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Permission",
                schema: "public",
                newName: "Permission",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "public",
                newName: "Payments",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "OrderRefund",
                schema: "public",
                newName: "OrderRefund",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                schema: "public",
                newName: "OrderItem",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Order",
                schema: "public",
                newName: "Order",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "NotificationToken",
                schema: "public",
                newName: "NotificationToken",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CouponUsage",
                schema: "public",
                newName: "CouponUsage",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Coupons",
                schema: "public",
                newName: "Coupons",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CategoryAttribute",
                schema: "public",
                newName: "CategoryAttribute",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Category",
                schema: "public",
                newName: "Category",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CartItem",
                schema: "public",
                newName: "CartItem",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Cart",
                schema: "public",
                newName: "Cart",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AuthOtp",
                schema: "public",
                newName: "AuthOtp",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AttributeValue",
                schema: "public",
                newName: "AttributeValue",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Attribute",
                schema: "public",
                newName: "Attribute",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "public",
                newName: "Addresses",
                newSchema: "dbo");
        }
    }
}
