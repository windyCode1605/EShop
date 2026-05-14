using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AuthOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthOtp",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OtpCode = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    ExpireTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VerifyTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthOtp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthOtp_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SendOtp",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    SendCount = table.Column<int>(type: "int", nullable: false),
                    LastSentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeLimitCanVerifyOtp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendOtp", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthOtp_UserId",
                schema: "dbo",
                table: "AuthOtp",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SendOtp",
                schema: "dbo",
                table: "SendOtp",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthOtp",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SendOtp",
                schema: "dbo");
        }
    }
}
