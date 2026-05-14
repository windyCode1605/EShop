using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShop.API.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Browser",
                schema: "core",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SysVar",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrName = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    VarName = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    VarValue = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    VarDesc = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysVar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysVar",
                schema: "dbo",
                table: "SysVar",
                columns: new[] { "GrName", "VarName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysVar",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "Browser",
                schema: "core",
                table: "Users");
        }
    }
}
