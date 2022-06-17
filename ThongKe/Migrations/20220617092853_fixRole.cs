using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThongKe.Migrations
{
    public partial class fixRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiNhanhs",
                table: "Roles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "Roles",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySua",
                table: "Roles",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ChiNhanhQL",
                table: "Roles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiNhanhQL",
                table: "Roles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayTao",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySua",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChiNhanhs",
                table: "Roles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
