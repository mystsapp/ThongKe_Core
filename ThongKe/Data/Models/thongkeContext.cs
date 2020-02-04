using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ThongKe.Models;

namespace ThongKe.Data.Models
{
    public partial class thongkeContext : DbContext
    {
        public thongkeContext()
        {
        }

        public thongkeContext(DbContextOptions<thongkeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Chinhanh> Chinhanh { get; set; }
        public virtual DbSet<Chitiettour> Chitiettour { get; set; }
        public virtual DbSet<Dmdaily> Dmdaily { get; set; }
        public virtual DbSet<DoanhthuDoanChitiet> DoanhthuDoanChitiet { get; set; }
        public virtual DbSet<DoanhthuDoanNgayDi> DoanhthuDoanNgayDi { get; set; }
        public virtual DbSet<DoanhthuQuayChitiet> DoanhthuQuayChitiet { get; set; }
        public virtual DbSet<DoanhthuSaleChitiet> DoanhthuSaleChitiet { get; set; }
        public virtual DbSet<DoanhthuSaleQuay> DoanhthuSaleQuay { get; set; }
        public virtual DbSet<DoanhthuSaleTuyen> DoanhthuSaleTuyen { get; set; }
        public virtual DbSet<DoanhthuSaleTuyentqChitiet> DoanhthuSaleTuyentqChitiet { get; set; }
        public virtual DbSet<DoanhthuToanhethong> DoanhthuToanhethong { get; set; }
        public virtual DbSet<DoanthuQuayNgayBan> DoanthuQuayNgayBan { get; set; }
        public virtual DbSet<QuayNgayBan> QuayNgayBan { get; set; }
        public virtual DbSet<Thongkeweb> Thongkeweb { get; set; }
        public virtual DbSet<Thongkewebchitiet> Thongkewebchitiet { get; set; }
        public virtual DbSet<Tuyentheoquy> Tuyentheoquy { get; set; }
        public virtual DbSet<TuyentqNgayban> TuyentqNgayban { get; set; }
        public virtual DbSet<TuyentqNgaydi> TuyentqNgaydi { get; set; }
        public virtual DbSet<TuyenThamQuanViewModel> Tuyentq { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=118.68.170.128;database=thongke;Trusted_Connection=true;User Id=vanhong;Password=Hong@2019;Integrated security=false;MultipleActiveResultSets=true");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.ToTable("account");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50);

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Daily)
                    .HasColumnName("daily")
                    .HasMaxLength(50);

                entity.Property(e => e.Doimatkhau).HasColumnName("doimatkhau");

                entity.Property(e => e.Hoten)
                    .HasColumnName("hoten")
                    .HasMaxLength(50);

                entity.Property(e => e.Khoi)
                    .IsRequired()
                    .HasColumnName("khoi")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('OB')");

                entity.Property(e => e.Ngaycapnhat)
                    .HasColumnName("ngaycapnhat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ngaydoimk)
                    .HasColumnName("ngaydoimk")
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ngaytao)
                    .HasColumnName("ngaytao")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Nguoicapnhat)
                    .HasColumnName("nguoicapnhat")
                    .HasMaxLength(50);

                entity.Property(e => e.Nguoitao)
                    .HasColumnName("nguoitao")
                    .HasMaxLength(50);

                entity.Property(e => e.Nhom)
                    .HasColumnName("nhom")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Trangthai)
                    .IsRequired()
                    .HasColumnName("trangthai")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Chinhanh>(entity =>
            {
                entity.ToTable("chinhanh");

                entity.Property(e => e.Chinhanh1)
                    .IsRequired()
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Diachi)
                    .HasColumnName("diachi")
                    .HasMaxLength(100);

                entity.Property(e => e.Dienthoai)
                    .HasColumnName("dienthoai")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50);

                entity.Property(e => e.Masothue)
                    .HasColumnName("masothue")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nhom)
                    .HasColumnName("nhom")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tencn)
                    .HasColumnName("tencn")
                    .HasMaxLength(50);

                entity.Property(e => e.Thanhpho)
                    .HasColumnName("thanhpho")
                    .HasMaxLength(70);

                entity.Property(e => e.Trangthai)
                    .IsRequired()
                    .HasColumnName("trangthai")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Chitiettour>(entity =>
            {
                entity.HasKey(e => e.Sgtcode);

                entity.ToTable("chitiettour");

                entity.Property(e => e.Sgtcode)
                    .HasColumnName("sgtcode")
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.Batdau)
                    .HasColumnName("batdau")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ketthuc)
                    .HasColumnName("ketthuc")
                    .HasColumnType("datetime");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(70);
            });

            modelBuilder.Entity<Dmdaily>(entity =>
            {
                entity.ToTable("dmdaily");

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3);

                entity.Property(e => e.Daily).HasMaxLength(25);

                entity.Property(e => e.Diachi).HasMaxLength(100);

                entity.Property(e => e.Dienthoai).HasMaxLength(50);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50);

                entity.Property(e => e.TenDaily).HasMaxLength(100);

                entity.Property(e => e.Trangthai)
                    .IsRequired()
                    .HasColumnName("trangthai")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<DoanhthuDoanChitiet>(entity =>
            {
                entity.ToTable("doanhthuDoanChitiet");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Congno)
                    .HasColumnName("congno")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Diachi)
                    .HasColumnName("diachi")
                    .HasMaxLength(200);

                entity.Property(e => e.Diemdon)
                    .HasColumnName("diemdon")
                    .HasMaxLength(200);

                entity.Property(e => e.Ghichu)
                    .HasColumnName("ghichu")
                    .HasMaxLength(200);

                entity.Property(e => e.Giave)
                    .HasColumnName("giave")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Serial)
                    .HasColumnName("serial")
                    .HasMaxLength(70);

                entity.Property(e => e.Stt).HasColumnName("stt");

                entity.Property(e => e.Tenkhach)
                    .HasColumnName("tenkhach")
                    .HasMaxLength(100);

                entity.Property(e => e.Thucthu)
                    .HasColumnName("thucthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Vetourid).HasColumnName("vetourid");
            });

            modelBuilder.Entity<DoanhthuDoanNgayDi>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("doanhthuDoanNgayDi");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Batdau)
                    .HasColumnName("batdau")
                    .HasColumnType("datetime");

                entity.Property(e => e.Doanhthu)
                    .HasColumnName("doanhthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Ketthuc)
                    .HasColumnName("ketthuc")
                    .HasColumnType("datetime");

                entity.Property(e => e.Sgtcode)
                    .HasColumnName("sgtcode")
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.Sokhach).HasColumnName("sokhach");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DoanhthuQuayChitiet>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("doanhthuQuayChitiet");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Giave)
                    .HasColumnName("giave")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Hanhtrinh)
                    .HasColumnName("hanhtrinh")
                    .HasMaxLength(150);

                entity.Property(e => e.Ngaydi)
                    .HasColumnName("ngaydi")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Ngayve)
                    .HasColumnName("ngayve")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Nguoiban)
                    .HasColumnName("nguoiban")
                    .HasMaxLength(50);

                entity.Property(e => e.Serial)
                    .HasColumnName("serial")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Sgtcode)
                    .HasColumnName("sgtcode")
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.Sokhach).HasColumnName("sokhach");

                entity.Property(e => e.Tenkhach)
                    .HasColumnName("tenkhach")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DoanhthuSaleChitiet>(entity =>
            {
                entity.HasKey(e => e.Stt)
                    .HasName("PK_doanhthusalechitiet");

                entity.ToTable("doanhthuSaleChitiet");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chiemcho).HasColumnName("chiemcho");

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Doanhthu)
                    .HasColumnName("doanhthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Nguoixuatve)
                    .HasColumnName("nguoixuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Sgtcode)
                    .HasColumnName("sgtcode")
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.Tenkhach)
                    .HasColumnName("tenkhach")
                    .HasMaxLength(50);

                entity.Property(e => e.Thucthu)
                    .HasColumnName("thucthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DoanhthuSaleQuay>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("doanhthuSaleQuay");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Doanhso)
                    .HasColumnName("doanhso")
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Nguoixuatve)
                    .HasColumnName("nguoixuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Thucthu)
                    .HasColumnName("thucthu")
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DoanhthuSaleTuyen>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("doanhthuSaleTuyen");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Doanhso)
                    .HasColumnName("doanhso")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Nguoixuatve)
                    .HasColumnName("nguoixuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Thucthu)
                    .HasColumnName("thucthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DoanhthuSaleTuyentqChitiet>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("doanhthuSaleTuyentqChitiet");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chiemcho).HasColumnName("chiemcho");

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Doanhthu)
                    .HasColumnName("doanhthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Nguoixuatve)
                    .HasColumnName("nguoixuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Sgtcode)
                    .HasColumnName("sgtcode")
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.Tenkhach)
                    .HasColumnName("tenkhach")
                    .HasMaxLength(50);

                entity.Property(e => e.Thucthu)
                    .HasColumnName("thucthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DoanhthuToanhethong>(entity =>
            {
                entity.HasKey(e => e.Stt)
                    .HasName("PK_doanhthuToanhethong_1");

                entity.ToTable("doanhthuToanhethong");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Dailyxuatve)
                    .HasColumnName("dailyxuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Khachcu).HasColumnName("khachcu");

                entity.Property(e => e.Khachht).HasColumnName("khachht");

                entity.Property(e => e.Thucthucu)
                    .HasColumnName("thucthucu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Thucthuht)
                    .HasColumnName("thucthuht")
                    .HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<DoanthuQuayNgayBan>(entity =>
            {
                entity.HasKey(e => e.Stt)
                    .HasName("PK_doanthuQuayNgayBan_1");

                entity.ToTable("doanthuQuayNgayBan");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Dailyxuatve)
                    .HasColumnName("dailyxuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Doanhso)
                    .HasColumnName("doanhso")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhthu)
                    .HasColumnName("doanhthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Sokhach).HasColumnName("sokhach");
            });

            modelBuilder.Entity<QuayNgayBan>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Dailyxuatve)
                    .HasColumnName("dailyxuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Doanhso)
                    .HasColumnName("doanhso")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhthu)
                    .HasColumnName("doanhthu")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Sokhach).HasColumnName("sokhach");
            });

            modelBuilder.Entity<Thongkeweb>(entity =>
            {
                entity.HasKey(e => e.Chinhanh);

                entity.ToTable("thongkeweb");

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Chuaxuatve).HasColumnName("chuaxuatve");

                entity.Property(e => e.Huy).HasColumnName("huy");

                entity.Property(e => e.Taove).HasColumnName("taove");

                entity.Property(e => e.Thanhcong).HasColumnName("thanhcong");

                entity.Property(e => e.Tuhuy).HasColumnName("tuhuy");
            });

            modelBuilder.Entity<Thongkewebchitiet>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("thongkewebchitiet");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Dailyxuatve)
                    .HasColumnName("dailyxuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Doanhso)
                    .HasColumnName("doanhso")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Hanhtrinh)
                    .HasColumnName("hanhtrinh")
                    .HasMaxLength(50);

                entity.Property(e => e.Huyve)
                    .HasColumnName("huyve")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Kenhgd)
                    .HasColumnName("kenhgd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ngaydi)
                    .HasColumnName("ngaydi")
                    .HasColumnType("date");

                entity.Property(e => e.Ngaytao)
                    .HasColumnName("ngaytao")
                    .HasColumnType("date");

                entity.Property(e => e.Ngayve)
                    .HasColumnName("ngayve")
                    .HasColumnType("date");

                entity.Property(e => e.Nguoixuatve)
                    .HasColumnName("nguoixuatve")
                    .HasMaxLength(50);

                entity.Property(e => e.Serial)
                    .HasColumnName("serial")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Sgtcode)
                    .HasColumnName("sgtcode")
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.Sokhach).HasColumnName("sokhach");

                entity.Property(e => e.Tenkhach)
                    .HasColumnName("tenkhach")
                    .HasMaxLength(50);

                entity.Property(e => e.Trangthai)
                    .HasColumnName("trangthai")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tuyentheoquy>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("tuyentheoquy");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Doanhso1)
                    .HasColumnName("doanhso1")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhso11)
                    .HasColumnName("doanhso_1")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhso2)
                    .HasColumnName("doanhso2")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhso21)
                    .HasColumnName("doanhso_2")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhso3)
                    .HasColumnName("doanhso3")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhso31)
                    .HasColumnName("doanhso_3")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Sk1).HasColumnName("sk1");

                entity.Property(e => e.Sk11).HasColumnName("sk_1");

                entity.Property(e => e.Sk2).HasColumnName("sk2");

                entity.Property(e => e.Sk21).HasColumnName("sk_2");

                entity.Property(e => e.Sk3).HasColumnName("sk3");

                entity.Property(e => e.Sk31).HasColumnName("sk_3");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TuyentqNgayban>(entity =>
            {
                entity.HasKey(e => e.Stt);

                entity.ToTable("tuyentqNgayban");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Sokhach).HasColumnName("sokhach");

                entity.Property(e => e.Thucthu)
                    .HasColumnName("thucthu")
                    .HasColumnType("decimal(12, 0)");

                entity.Property(e => e.Tongtien)
                    .HasColumnName("tongtien")
                    .HasColumnType("decimal(12, 0)");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TuyentqNgaydi>(entity =>
            {
                entity.HasKey(e => e.Stt)
                    .HasName("PK_tuyentqNgaydi_1");

                entity.ToTable("tuyentqNgaydi");

                entity.Property(e => e.Stt)
                    .HasColumnName("stt")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Khachcu)
                    .HasColumnName("khachcu")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Khachht)
                    .HasColumnName("khachht")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Thucthucu)
                    .HasColumnName("thucthucu")
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Thucthuht)
                    .HasColumnName("thucthuht")
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
