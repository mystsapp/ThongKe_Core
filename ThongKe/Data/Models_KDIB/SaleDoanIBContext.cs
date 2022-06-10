using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ThongKe.Data.Models_KDIB
{
    public partial class SaleDoanIBContext : DbContext
    {
        public SaleDoanIBContext()
        {
        }

        public SaleDoanIBContext(DbContextOptions<SaleDoanIBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tours> Tours { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=118.68.170.128;database=SaleDoanIB;Trusted_Connection=true;User Id=vanhong;Password=Hong@2019;Integrated security=false;MultipleActiveResultSets=true");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tours>(entity =>
            {
                entity.Property(e => e.ChiNhanhDhid).HasColumnName("ChiNhanhDHId");

                entity.Property(e => e.ChuDeTour)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ChuongTrinhTour).HasMaxLength(300);

                entity.Property(e => e.DaiLy).HasMaxLength(100);

                entity.Property(e => e.DiaChi).HasMaxLength(250);

                entity.Property(e => e.DichVu).HasMaxLength(150);

                entity.Property(e => e.DienThoai)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.DoanhThuDk)
                    .HasColumnName("DoanhThuDK")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DoanhThuTt)
                    .HasColumnName("DoanhThuTT")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DoiTacNuocNgoai).HasMaxLength(150);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Fax)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.FileBienNhan)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FileKhachDiTour)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FileVeMayBay)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.GhiChu).HasMaxLength(50);

                entity.Property(e => e.HinhThucGiaoDich).HasMaxLength(50);

                entity.Property(e => e.KhachLe)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.LaiChuaVe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LaiGomVe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LaiThucTeGomVe).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LoaiKhach).HasMaxLength(50);

                entity.Property(e => e.LoaiTien)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LyDoNhanDu).HasMaxLength(250);

                entity.Property(e => e.MaKh)
                    .IsRequired()
                    .HasColumnName("MaKH")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.NdhuyTourId)
                    .HasColumnName("NDHuyTourId")
                    .HasDefaultValueSql("(CONVERT([bigint],(0)))");

                entity.Property(e => e.NgayDen).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.NgayDi).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.NgayThanhLyHd).HasColumnName("NgayThanhLyHD");

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(100);

                entity.Property(e => e.NguoiKhoa)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NguoiKyHopDong).HasMaxLength(50);

                entity.Property(e => e.NguoiSua)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NguoiTao)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NguonTour).HasMaxLength(100);

                entity.Property(e => e.NoiDungThanhLyHd)
                    .HasColumnName("NoiDungThanhLyHD")
                    .HasMaxLength(250);

                entity.Property(e => e.PhongBanMaCode).HasMaxLength(5);

                entity.Property(e => e.PhongDh)
                    .IsRequired()
                    .HasColumnName("PhongDH")
                    .HasMaxLength(150);

                entity.Property(e => e.Sgtcode)
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.SktreEm).HasColumnName("SKTreEm");

                entity.Property(e => e.SoHopDong)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SoKhachDk).HasColumnName("SoKhachDK");

                entity.Property(e => e.SoKhachTt).HasColumnName("SoKhachTT");

                entity.Property(e => e.TenKh)
                    .IsRequired()
                    .HasColumnName("TenKH")
                    .HasMaxLength(50);

                entity.Property(e => e.TrangThai)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.TuyenTq)
                    .IsRequired()
                    .HasColumnName("TuyenTQ")
                    .HasMaxLength(250);

                entity.Property(e => e.TyGia).HasColumnType("decimal(18, 2)");
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
