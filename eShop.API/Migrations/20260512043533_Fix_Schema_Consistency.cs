using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Schema_Consistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserProfile",
                schema: "core",
                newName: "UserProfile",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Reviews",
                schema: "core",
                newName: "Reviews",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "core",
                newName: "Payments",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Coupons",
                schema: "core",
                newName: "Coupons",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "core",
                newName: "Addresses",
                newSchema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserProfile",
                schema: "dbo",
                newName: "UserProfile",
                newSchema: "core");

            migrationBuilder.RenameTable(
                name: "Reviews",
                schema: "dbo",
                newName: "Reviews",
                newSchema: "core");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "dbo",
                newName: "Payments",
                newSchema: "core");

            migrationBuilder.RenameTable(
                name: "Coupons",
                schema: "dbo",
                newName: "Coupons",
                newSchema: "core");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "dbo",
                newName: "Addresses",
                newSchema: "core");
        }
    }
}
