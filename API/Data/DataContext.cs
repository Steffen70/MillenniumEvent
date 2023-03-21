using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Write Fluent API configurations here

            builder.Entity<Reservation>()
                .HasOne(t => t.AppUser)
                .WithMany(u => u.Reservations)
                .HasForeignKey(t => t.AppUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.Bike)
                .WithMany(b => b.Reservations)
                .HasForeignKey(r => r.BikeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.ApplyUtcDateTimeConverter();
        }
    }
}