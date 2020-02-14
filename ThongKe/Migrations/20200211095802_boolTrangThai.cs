using Microsoft.EntityFrameworkCore.Migrations;

namespace ThongKe.Migrations
{
    public partial class boolTrangThai : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_account",
                table: "account");

            migrationBuilder.RenameTable(
                name: "account",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "account");

            migrationBuilder.AddPrimaryKey(
                name: "PK_account",
                table: "account",
                column: "username");
        }
    }
}
