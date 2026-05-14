using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class Sync_Current_Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "core",
                table: "Users");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameColumn(
                name: "Username",
                schema: "core",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "core",
                table: "Users",
                newName: "DateTimeLoginFailCount");

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageUri",
                schema: "core",
                table: "Users",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brower",
                schema: "core",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                schema: "core",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                schema: "core",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                schema: "core",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                schema: "core",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                schema: "core",
                table: "Users",
                type: "int",
                maxLength: 255,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HomeTown",
                schema: "core",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdCode",
                schema: "core",
                table: "Users",
                type: "varchar(125)",
                unicode: false,
                maxLength: 125,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdminCustomer",
                schema: "core",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstTime",
                schema: "core",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTempPassword",
                schema: "core",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTempPin",
                schema: "core",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedStatus",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginFailCount",
                schema: "core",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                schema: "core",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatingSystem",
                schema: "core",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpRequestId",
                schema: "core",
                table: "Users",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassWord",
                schema: "core",
                table: "Users",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                schema: "core",
                table: "Users",
                type: "varchar(128)",
                unicode: false,
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResendOtpDate",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "S3Key",
                schema: "core",
                table: "Users",
                type: "nvarchar(2024)",
                maxLength: 2024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretPassWordCode",
                schema: "core",
                table: "Users",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SecretPasswordExpireTime",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "core",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TenantDomainRegister",
                schema: "core",
                table: "Users",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                schema: "core",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantLanguageRegister",
                schema: "core",
                table: "Users",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantNameRegister",
                schema: "core",
                table: "Users",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeLockUser",
                schema: "core",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCode",
                schema: "core",
                table: "Users",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                schema: "core",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerRole",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PermissionInWeb = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationToken",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FcmToken = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                    ApnsToken = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationToken_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PermissionInWeb = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerRolePermission",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerRoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionKey = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerRolePermission_CustomerRole_CustomerRoleId",
                        column: x => x.CustomerRoleId,
                        principalSchema: "dbo",
                        principalTable: "CustomerRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerUserRole",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CustomerRoleId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CustomerUserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerUserRole_CustomerRole_CustomerRoleId",
                        column: x => x.CustomerRoleId,
                        principalSchema: "dbo",
                        principalTable: "CustomerRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerUserRole_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRolePermission_CustomerRoleId",
                schema: "dbo",
                table: "CustomerRolePermission",
                column: "CustomerRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUserRole_CustomerRoleId",
                schema: "dbo",
                table: "CustomerUserRole",
                column: "CustomerRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUserRole_UserId",
                schema: "dbo",
                table: "CustomerUserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationToken_UserId",
                schema: "dbo",
                table: "NotificationToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole",
                schema: "dbo",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId", "Deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "dbo",
                table: "UserRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerRolePermission",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CustomerUserRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NotificationToken",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CustomerRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "AvatarImageUri",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Brower",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deleted",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HomeTown",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IdCode",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsAdminCustomer",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsFirstTime",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsTempPassword",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsTempPin",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockedStatus",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginFailCount",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OperatingSystem",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpRequestId",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PassWord",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PinCode",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResendOtpDate",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "S3Key",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecretPassWordCode",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecretPasswordExpireTime",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantDomainRegister",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantLanguageRegister",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantNameRegister",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TimeLockUser",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserCode",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserType",
                schema: "core",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserName",
                schema: "core",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "DateTimeLoginFailCount",
                schema: "core",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "core",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "core",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "core",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
