using Microsoft.EntityFrameworkCore;
using Warehouse.Model;

namespace Warehouse.Db
{
    public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<Pallet> Pallets { get; set; }
        public DbSet<Pallet_Location> Pallet_Locations { get; set; }
        public DbSet<Robot> Robots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Pallet_Location entity
            modelBuilder.Entity<Pallet_Location>(entity =>
            {
                entity.HasKey(pl => new { pl.Pallet_ID, pl.Time_In });

                entity.Property(pl => pl.Pallet_ID)
                      .HasMaxLength(10)
                      .IsRequired()
                      .HasColumnType("nvarchar(10)");

                entity.Property(pl => pl.Time_In)
                      .IsRequired()
                      .HasColumnType("datetime2");

                entity.Property(pl => pl.Location_ID)
                      .HasMaxLength(50) // Tăng độ dài để phù hợp với Location_ID mới
                      .HasColumnType("nvarchar(50)");

                entity.Property(pl => pl.Time_Out)
                      .HasColumnType("datetime2");

                entity.HasOne(pl => pl.Pallet)
                      .WithMany(p => p.Pallet_Locations)
                      .HasForeignKey(pl => pl.Pallet_ID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pl => pl.Location)
                      .WithMany(l => l.Pallet_Locations)
                      .HasForeignKey(pl => pl.Location_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Pallet entity
            modelBuilder.Entity<Pallet>(entity =>
            {
                entity.HasKey(p => p.Pallet_ID);

                entity.Property(p => p.Pallet_ID)
                      .HasMaxLength(10)
                      .IsRequired()
                      .HasColumnType("nvarchar(10)");

                entity.Property(p => p.Status)
                      .HasMaxLength(20)
                      .IsRequired()
                      .HasColumnType("nvarchar(20)");

                entity.Property(p => p.Type)
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(p => p.Size)
                      .HasMaxLength(50)
                      .HasColumnType("nvarchar(50)");

                entity.Property(p => p.Creation_Date)
                      .HasColumnType("datetime2");

                entity.HasOne(p => p.Robot)
                      .WithMany(r => r.Pallets)
                      .HasForeignKey(p => p.Robot_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Location entity
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.Location_ID);

                entity.Property(l => l.Location_ID)
                      .HasMaxLength(50) // Tăng độ dài lên 50
                      .IsRequired()
                      .HasColumnType("nvarchar(50)");

                entity.Property(l => l.Name)
                      .HasMaxLength(100)
                      .IsRequired()
                      .HasColumnType("nvarchar(100)");

                entity.Property(l => l.Parent_Location_ID)
                      .HasMaxLength(50) // Tăng độ dài lên 50
                      .HasColumnType("nvarchar(50)");

                entity.HasOne(l => l.Parent_Location)
                      .WithMany(l => l.Child_Locations)
                      .HasForeignKey(l => l.Parent_Location_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Robot entity
            modelBuilder.Entity<Robot>(entity =>
            {
                entity.HasKey(r => r.Robot_ID);

                entity.Property(r => r.Robot_ID)
                      .HasMaxLength(10)
                      .IsRequired()
                      .HasColumnType("nvarchar(10)");
            });
        }
    }
}