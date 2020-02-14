using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThongKe.Migrations
{
    public partial class changeAccountName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    username = table.Column<string>(maxLength: 50, nullable: false),
                    password = table.Column<string>(maxLength: 50, nullable: true),
                    hoten = table.Column<string>(maxLength: 50, nullable: true),
                    daily = table.Column<string>(maxLength: 50, nullable: true),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    role = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    doimatkhau = table.Column<bool>(nullable: false),
                    ngaydoimk = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    trangthai = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    khoi = table.Column<string>(unicode: false, maxLength: 5, nullable: false, defaultValueSql: "('OB')"),
                    nguoitao = table.Column<string>(maxLength: 50, nullable: true),
                    ngaytao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    nguoicapnhat = table.Column<string>(maxLength: 50, nullable: true),
                    ngaycapnhat = table.Column<DateTime>(type: "datetime", nullable: true),
                    nhom = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.username);
                });

            migrationBuilder.CreateTable(
                name: "chinhanh",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: false),
                    tencn = table.Column<string>(maxLength: 50, nullable: true),
                    diachi = table.Column<string>(maxLength: 100, nullable: true),
                    thanhpho = table.Column<string>(maxLength: 70, nullable: true),
                    dienthoai = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    fax = table.Column<string>(maxLength: 50, nullable: true),
                    masothue = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    trangthai = table.Column<bool>(nullable: false, defaultValueSql: "((1))"),
                    nhom = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chinhanh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "chitiettour",
                columns: table => new
                {
                    sgtcode = table.Column<string>(unicode: false, maxLength: 17, nullable: false),
                    tuyentq = table.Column<string>(maxLength: 70, nullable: true),
                    batdau = table.Column<DateTime>(type: "datetime", nullable: true),
                    ketthuc = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chitiettour", x => x.sgtcode);
                });

            migrationBuilder.CreateTable(
                name: "dmdaily",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Daily = table.Column<string>(maxLength: 25, nullable: true),
                    TenDaily = table.Column<string>(maxLength: 100, nullable: true),
                    Diachi = table.Column<string>(maxLength: 100, nullable: true),
                    Dienthoai = table.Column<string>(maxLength: 50, nullable: true),
                    fax = table.Column<string>(maxLength: 50, nullable: true),
                    chinhanh = table.Column<string>(maxLength: 3, nullable: true),
                    trangthai = table.Column<bool>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dmdaily", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuDoanChitiet",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false),
                    vetourid = table.Column<int>(nullable: true),
                    stt = table.Column<int>(nullable: true),
                    serial = table.Column<string>(maxLength: 70, nullable: true),
                    tenkhach = table.Column<string>(maxLength: 100, nullable: true),
                    diachi = table.Column<string>(maxLength: 200, nullable: true),
                    diemdon = table.Column<string>(maxLength: 200, nullable: true),
                    giave = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    thucthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    congno = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    ghichu = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthuDoanChitiet", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuDoanNgayDi",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    sgtcode = table.Column<string>(unicode: false, maxLength: 17, nullable: true),
                    tuyentq = table.Column<string>(maxLength: 50, nullable: true),
                    batdau = table.Column<DateTime>(type: "datetime", nullable: true),
                    ketthuc = table.Column<DateTime>(type: "datetime", nullable: true),
                    sokhach = table.Column<int>(nullable: true),
                    doanhthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthuDoanNgayDi", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuQuayChitiet",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    sgtcode = table.Column<string>(unicode: false, maxLength: 17, nullable: true),
                    serial = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    tenkhach = table.Column<string>(maxLength: 50, nullable: true),
                    hanhtrinh = table.Column<string>(maxLength: 150, nullable: true),
                    ngaydi = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    ngayve = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    sokhach = table.Column<int>(nullable: false),
                    giave = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    nguoiban = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthuQuayChitiet", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuSaleChitiet",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    sgtcode = table.Column<string>(unicode: false, maxLength: 17, nullable: true),
                    tuyentq = table.Column<string>(maxLength: 50, nullable: true),
                    tenkhach = table.Column<string>(maxLength: 50, nullable: true),
                    chiemcho = table.Column<int>(nullable: true),
                    doanhthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    thucthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    nguoixuatve = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthusalechitiet", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuSaleQuay",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    nguoixuatve = table.Column<string>(maxLength: 50, nullable: true),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    doanhso = table.Column<decimal>(type: "decimal(18, 0)", nullable: true, defaultValueSql: "((0))"),
                    thucthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthuSaleQuay", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuSaleTuyen",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    nguoixuatve = table.Column<string>(maxLength: 50, nullable: true),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    tuyentq = table.Column<string>(maxLength: 50, nullable: true),
                    doanhso = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    thucthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthuSaleTuyen", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuSaleTuyentqChitiet",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    sgtcode = table.Column<string>(unicode: false, maxLength: 17, nullable: true),
                    tuyentq = table.Column<string>(maxLength: 50, nullable: true),
                    tenkhach = table.Column<string>(maxLength: 50, nullable: true),
                    chiemcho = table.Column<int>(nullable: true),
                    doanhthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    thucthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    nguoixuatve = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthuSaleTuyentqChitiet", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "doanhthuToanhethong",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    dailyxuatve = table.Column<string>(maxLength: 50, nullable: true),
                    khachht = table.Column<int>(nullable: true),
                    thucthuht = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    khachcu = table.Column<int>(nullable: true),
                    thucthucu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanhthuToanhethong_1", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "doanthuQuayNgayBan",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    dailyxuatve = table.Column<string>(maxLength: 50, nullable: true),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    sokhach = table.Column<int>(nullable: true),
                    doanhso = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    doanhthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doanthuQuayNgayBan_1", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "KhachHuys",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenkhach = table.Column<string>(nullable: true),
                    sgtcode = table.Column<string>(nullable: true),
                    vetourid = table.Column<int>(nullable: false),
                    tuyentq = table.Column<string>(nullable: true),
                    batdau = table.Column<DateTime>(nullable: false),
                    ketthuc = table.Column<DateTime>(nullable: false),
                    giatour = table.Column<decimal>(nullable: true),
                    nguoihuyve = table.Column<string>(nullable: true),
                    dailyhuyve = table.Column<string>(nullable: true),
                    chinhanh = table.Column<string>(nullable: true),
                    ngayhuyve = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHuys", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "QuayNgayBan",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    dailyxuatve = table.Column<string>(maxLength: 50, nullable: true),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    sokhach = table.Column<int>(nullable: true),
                    doanhso = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    doanhthu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuayNgayBan", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "thongkeweb",
                columns: table => new
                {
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: false),
                    taove = table.Column<int>(nullable: true),
                    tuhuy = table.Column<int>(nullable: true),
                    chuaxuatve = table.Column<int>(nullable: true),
                    thanhcong = table.Column<int>(nullable: true),
                    huy = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thongkeweb", x => x.chinhanh);
                });

            migrationBuilder.CreateTable(
                name: "thongkewebchitiet",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    sgtcode = table.Column<string>(unicode: false, maxLength: 17, nullable: true),
                    hanhtrinh = table.Column<string>(maxLength: 50, nullable: true),
                    ngaydi = table.Column<DateTime>(type: "date", nullable: true),
                    ngayve = table.Column<DateTime>(type: "date", nullable: true),
                    tenkhach = table.Column<string>(maxLength: 50, nullable: true),
                    serial = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
                    huyve = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
                    sokhach = table.Column<int>(nullable: true),
                    doanhso = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    nguoixuatve = table.Column<string>(maxLength: 50, nullable: true),
                    dailyxuatve = table.Column<string>(maxLength: 50, nullable: true),
                    kenhgd = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    trangthai = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ngaytao = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thongkewebchitiet", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "TourBySGTCodeViewModels",
                columns: table => new
                {
                    sgtcode = table.Column<string>(nullable: false),
                    tuyentq = table.Column<string>(nullable: true),
                    batdau = table.Column<DateTime>(nullable: false),
                    ketthuc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourBySGTCodeViewModels", x => x.sgtcode);
                });

            migrationBuilder.CreateTable(
                name: "tuyentheoquy",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    tuyentq = table.Column<string>(maxLength: 50, nullable: true),
                    sk1 = table.Column<int>(nullable: true),
                    doanhso1 = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    sk_1 = table.Column<int>(nullable: true),
                    doanhso_1 = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    sk2 = table.Column<int>(nullable: true),
                    doanhso2 = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    sk_2 = table.Column<int>(nullable: true),
                    doanhso_2 = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    sk3 = table.Column<int>(nullable: true),
                    doanhso3 = table.Column<decimal>(type: "decimal(18, 0)", nullable: true),
                    sk_3 = table.Column<int>(nullable: true),
                    doanhso_3 = table.Column<decimal>(type: "decimal(18, 0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tuyentheoquy", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "Tuyentq",
                columns: table => new
                {
                    tuyentq = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tuyentq", x => x.tuyentq);
                });

            migrationBuilder.CreateTable(
                name: "TuyentqChiTietViewModels",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chinhanh = table.Column<string>(nullable: true),
                    tuyentq = table.Column<string>(nullable: true),
                    sgtcode = table.Column<string>(nullable: true),
                    vetourid = table.Column<int>(nullable: false),
                    batdau = table.Column<DateTime>(nullable: false),
                    ketthuc = table.Column<DateTime>(nullable: false),
                    dailyxuatve = table.Column<string>(nullable: true),
                    sk = table.Column<int>(nullable: false),
                    doanhthu = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuyentqChiTietViewModels", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "tuyentqNgayban",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    tuyentq = table.Column<string>(maxLength: 50, nullable: true),
                    sokhach = table.Column<int>(nullable: true),
                    tongtien = table.Column<decimal>(type: "decimal(12, 0)", nullable: true),
                    thucthu = table.Column<decimal>(type: "decimal(12, 0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tuyentqNgayban", x => x.stt);
                });

            migrationBuilder.CreateTable(
                name: "tuyentqNgaydi",
                columns: table => new
                {
                    stt = table.Column<long>(nullable: false),
                    chinhanh = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
                    tuyentq = table.Column<string>(maxLength: 50, nullable: true),
                    khachht = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
                    thucthuht = table.Column<decimal>(type: "decimal(18, 0)", nullable: true, defaultValueSql: "((0))"),
                    khachcu = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
                    thucthucu = table.Column<decimal>(type: "decimal(18, 0)", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tuyentqNgaydi_1", x => x.stt);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "chinhanh");

            migrationBuilder.DropTable(
                name: "chitiettour");

            migrationBuilder.DropTable(
                name: "dmdaily");

            migrationBuilder.DropTable(
                name: "doanhthuDoanChitiet");

            migrationBuilder.DropTable(
                name: "doanhthuDoanNgayDi");

            migrationBuilder.DropTable(
                name: "doanhthuQuayChitiet");

            migrationBuilder.DropTable(
                name: "doanhthuSaleChitiet");

            migrationBuilder.DropTable(
                name: "doanhthuSaleQuay");

            migrationBuilder.DropTable(
                name: "doanhthuSaleTuyen");

            migrationBuilder.DropTable(
                name: "doanhthuSaleTuyentqChitiet");

            migrationBuilder.DropTable(
                name: "doanhthuToanhethong");

            migrationBuilder.DropTable(
                name: "doanthuQuayNgayBan");

            migrationBuilder.DropTable(
                name: "KhachHuys");

            migrationBuilder.DropTable(
                name: "QuayNgayBan");

            migrationBuilder.DropTable(
                name: "thongkeweb");

            migrationBuilder.DropTable(
                name: "thongkewebchitiet");

            migrationBuilder.DropTable(
                name: "TourBySGTCodeViewModels");

            migrationBuilder.DropTable(
                name: "tuyentheoquy");

            migrationBuilder.DropTable(
                name: "Tuyentq");

            migrationBuilder.DropTable(
                name: "TuyentqChiTietViewModels");

            migrationBuilder.DropTable(
                name: "tuyentqNgayban");

            migrationBuilder.DropTable(
                name: "tuyentqNgaydi");
        }
    }
}
