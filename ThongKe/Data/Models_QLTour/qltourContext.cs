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

        public virtual DbSet<Phongban> Phongban { get; set; }

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

            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
