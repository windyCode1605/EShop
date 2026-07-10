using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionMasterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The table and constraints were already created manually via SQL script by the user.
            // Leaving this empty to sync the EF Core model without executing duplicate SQL.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionKey",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.DropTable(
                name: "Permission",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_RolePermission_PermissionKey",
                schema: "dbo",
                table: "RolePermission");

            migrationBuilder.AlterColumn<string>(
                name: "PermissionKey",
                schema: "dbo",
                table: "RolePermission",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);
        }
    }
}
