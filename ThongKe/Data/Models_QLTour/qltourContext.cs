using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ThongKe.Data.Models_QLTour
{
    public partial class qltourContext : DbContext
    {
        public qltourContext()
        {
        }

        public qltourContext(DbContextOptions<qltourContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Dmchinhanh> Dmchinhanh { get; set; }
        public virtual DbSet<Phongban> Phongban { get; set; }
        public virtual DbSet<Tourkind> Tourkind { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=118.68.170.128;database=qltour;Trusted_Connection=true;User Id=vanhong;Password=Hong@2019;Integrated security=false;MultipleActiveResultSets=true");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.Property(e => e.CompanyId)
                    .HasColumnName("companyId")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(250);

                entity.Property(e => e.Chinhanh)
                    .HasColumnName("chinhanh")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Contact)
                    .HasColumnName("contact")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50);

                entity.Property(e => e.Fullname)
                    .HasColumnName("fullname")
                    .HasMaxLength(80);

                entity.Property(e => e.Headoffice)
                    .HasColumnName("headoffice")
                    .HasMaxLength(50);

                entity.Property(e => e.Msthue)
                    .HasColumnName("msthue")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(80);

                entity.Property(e => e.Nation)
                    .HasColumnName("nation")
                    .HasMaxLength(50);

                entity.Property(e => e.Natione)
                    .HasColumnName("natione")
                    .HasMaxLength(50);

                entity.Property(e => e.Nguoidaidien)
                    .HasColumnName("nguoidaidien")
                    .HasMaxLength(50);

                entity.Property(e => e.Nguoilienhe)
                    .HasColumnName("nguoilienhe")
                    .HasMaxLength(50);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Dmchinhanh>(entity =>
            {
                entity.Property(e => e.Diachi).HasMaxLength(100);

                entity.Property(e => e.Dienthoai).HasMaxLength(50);

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.Macn)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.Masothue).HasMaxLength(20);

                entity.Property(e => e.Tencn).HasMaxLength(50);

                entity.Property(e => e.Thanhpho).HasMaxLength(50);
            });

            modelBuilder.Entity<Phongban>(entity =>
            {
                entity.HasKey(e => e.Maphong);

                entity.Property(e => e.Maphong)
                    .HasColumnName("maphong")
                    .HasMaxLength(5);

                entity.Property(e => e.Macode)
                    .HasColumnName("macode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tenphong)
                    .HasColumnName("tenphong")
                    .HasMaxLength(100);

                entity.Property(e => e.Trangthai).HasColumnName("trangthai");
            });

            modelBuilder.Entity<Tourkind>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.TourkindInf).HasMaxLength(50);
            });

            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
