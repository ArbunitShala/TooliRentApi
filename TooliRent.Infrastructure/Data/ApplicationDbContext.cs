using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TooliRent.Core.Models.Bookings;
using TooliRent.Core.Models.Catalog;
using TooliRent.Infrastructure.Auth;

namespace TooliRent.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tool> Tools => Set<Tool>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingTool> BookingTools => Set<BookingTool>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Category>()
                .HasIndex(x => x.Name).IsUnique();

            b.Entity<Tool>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tools)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<BookingTool>()
                .HasKey(bt => new { bt.BookingId, bt.ToolId });

            b.Entity<BookingTool>()
                .HasOne(bt => bt.Booking)
                .WithMany(bk => bk.BookingTools)
                .HasForeignKey(bt => bt.BookingId);

            b.Entity<BookingTool>()
                .HasOne(bt => bt.Tool)
                .WithMany(t => t.BookingTools)
                .HasForeignKey(bt => bt.ToolId);
        }
    }
}
