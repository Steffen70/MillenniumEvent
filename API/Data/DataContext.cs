using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Write Fluent API configurations here

            builder.Entity<Ticket>()
                .HasOne(t => t.AppUser)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.AppUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.ApplyUtcDateTimeConverter();
        }
    }
}