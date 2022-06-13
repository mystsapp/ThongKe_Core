using Microsoft.EntityFrameworkCore.Migrations;

namespace ThongKe.Migrations
{
    public partial class fixMoreUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaiLyQL",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhongBanQL",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoginViewModels",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    Mact = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: false),
                    Trangthai = table.Column<bool>(nullable: false),
                    Doimk = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginViewModels", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "ThongKeDoanhThuViewModels",
                columns: table => new
                {
                    DaiLyXuatVe = table.Column<string>(nullable: false),
                    DoanhThuHT = table.Column<decimal>(nullable: true),
                    DoanhThuTT = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeDoanhThuViewModels", x => x.DaiLyXuatVe);
                });

            migrationBuilder.CreateTable(
                name: "ThongKeKhachViewModels",
                columns: table => new
                {
                    DaiLyXuatVe = table.Column<string>(nullable: false),
                    SoKhachHT = table.Column<int>(nullable: true),
                    SoKhachTT = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeKhachViewModels", x => x.DaiLyXuatVe);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginViewModels");

            migrationBuilder.DropTable(
                name: "ThongKeDoanhThuViewModels");

            migrationBuilder.DropTable(
                name: "ThongKeKhachViewModels");

            migrationBuilder.DropColumn(
                name: "DaiLyQL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhongBanQL",
                table: "Users");
        }
    }
}
