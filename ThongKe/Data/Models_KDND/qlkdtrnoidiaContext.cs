using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ThongKe.Data.Models_KDND
{
    public partial class qlkdtrnoidiaContext : DbContext
    {
        public qlkdtrnoidiaContext()
        {
        }

        public qlkdtrnoidiaContext(DbContextOptions<qlkdtrnoidiaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tour> Tour { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=118.68.170.128;database=qlkdtrnoidia;Trusted_Connection=true;User Id=vanhong;Password=Hong@2019;Integrated security=false;MultipleActiveResultSets=true");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(e => e.Idtour);

                entity.ToTable("tour");

                entity.Property(e => e.Idtour)
                    .HasColumnName("idtour")
                    .HasColumnType("decimal(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Batdau)
                    .HasColumnName("batdau")
                    .HasColumnType("datetime");

                entity.Property(e => e.ChiNhanhDh)
                    .HasColumnName("ChiNhanhDH")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3);

                entity.Property(e => e.Chudetour)
                    .HasColumnName("chudetour")
                    .HasMaxLength(150);

                entity.Property(e => e.Chuongtrinhtour).HasColumnName("chuongtrinhtour");

                entity.Property(e => e.Daily)
                    .HasColumnName("daily")
                    .HasMaxLength(25);

                entity.Property(e => e.Diachi)
                    .HasColumnName("diachi")
                    .HasMaxLength(150);

                entity.Property(e => e.Dichvu).HasColumnName("dichvu");

                entity.Property(e => e.Diemtq)
                    .HasColumnName("diemtq")
                    .HasMaxLength(150);

                entity.Property(e => e.Dienthoai)
                    .HasColumnName("dienthoai")
                    .HasMaxLength(50);

                entity.Property(e => e.Doanhthudk)
                    .HasColumnName("doanhthudk")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doanhthutt)
                    .HasColumnName("doanhthutt")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Doitacnuocngoai)
                    .HasColumnName("doitacnuocngoai")
                    .HasMaxLength(150);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50);

                entity.Property(e => e.Filebiennhan).HasColumnName("filebiennhan");

                entity.Property(e => e.Filekhachditour).HasColumnName("filekhachditour");

                entity.Property(e => e.Filevemaybay).HasColumnName("filevemaybay");

                entity.Property(e => e.Hanxuatvmb)
                    .HasColumnName("hanxuatvmb")
                    .HasColumnType("date");

                entity.Property(e => e.Hinhthucgiaodich)
                    .HasColumnName("hinhthucgiaodich")
                    .HasMaxLength(150);

                entity.Property(e => e.Ketthuc)
                    .HasColumnName("ketthuc")
                    .HasColumnType("datetime");

                entity.Property(e => e.Laichuave)
                    .HasColumnName("laichuave")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Laigomve)
                    .HasColumnName("laigomve")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Laithuctegomve)
                    .HasColumnName("laithuctegomve")
                    .HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Lidonhandu).HasColumnName("lidonhandu");

                entity.Property(e => e.Loaitourid)
                    .HasColumnName("loaitourid")
                    .HasMaxLength(50);

                entity.Property(e => e.Makh)
                    .HasColumnName("makh")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Ngaydamphan)
                    .HasColumnName("ngaydamphan")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ngayhuytour)
                    .HasColumnName("ngayhuytour")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ngaykyhopdong)
                    .HasColumnName("ngaykyhopdong")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ngaynhandutien)
                    .HasColumnName("ngaynhandutien")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ngaysua)
                    .HasColumnName("ngaysua")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ngaytao)
                    .HasColumnName("ngaytao")
                    .HasColumnType("datetime");

                entity.Property(e => e.Ngaythanhlyhd)
                    .HasColumnName("ngaythanhlyhd")
                    .HasColumnType("datetime");

                entity.Property(e => e.Nguoidaidien)
                    .HasColumnName("nguoidaidien")
                    .HasMaxLength(150);

                entity.Property(e => e.Nguoikyhopdong)
                    .HasColumnName("nguoikyhopdong")
                    .HasMaxLength(150);

                entity.Property(e => e.Nguoisua)
                    .HasColumnName("nguoisua")
                    .HasMaxLength(50);

                entity.Property(e => e.Nguoitao)
                    .HasColumnName("nguoitao")
                    .HasMaxLength(50);

                entity.Property(e => e.Nguontour)
                    .HasColumnName("nguontour")
                    .HasMaxLength(150);

                entity.Property(e => e.Nguyennhanhuythau).HasColumnName("nguyennhanhuythau");

                entity.Property(e => e.Noidungthanhlyhd).HasColumnName("noidungthanhlyhd");

                entity.Property(e => e.Sgtcode)
                    .HasColumnName("sgtcode")
                    .HasMaxLength(17)
                    .IsUnicode(false);

                entity.Property(e => e.Sohopdong)
                    .HasColumnName("sohopdong")
                    .HasMaxLength(50);

                entity.Property(e => e.Sokhachdk).HasColumnName("sokhachdk");

                entity.Property(e => e.Sokhachtt).HasColumnName("sokhachtt");

                entity.Property(e => e.Tenkh)
                    .HasColumnName("tenkh")
                    .HasMaxLength(150);

                entity.Property(e => e.Trangthai)
                    .HasColumnName("trangthai")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Tuyentq)
                    .HasColumnName("tuyentq")
                    .HasMaxLength(150);
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
