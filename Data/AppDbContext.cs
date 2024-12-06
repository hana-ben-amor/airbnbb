using airbnbb.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace airbnbb.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        // Configuring relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Review>()
            .HasOne(r => r.Property)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: User -> Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
       

            ////// Many-to-Many relationship between Property and Amenity

            // One-to-Many: User -> Property
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Host)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.HostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Wishlist>()
          .HasKey(w => new { w.UserId, w.PropertyId });  // Composite key

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)  // Assuming a User can have multiple wishlists
                .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Property)
                .WithMany(p => p.Wishlists)  // Assuming a Property can be in many wishlists
                .HasForeignKey(w => w.PropertyId);

            // **Booking relationships**
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Guest)  // Linking Booking to User (Guest)
                .WithMany(u => u.Bookings)  // A user can have multiple bookings
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Property)  // Linking Booking to Property
                .WithMany(p => p.Bookings)  // A property can have multiple bookings
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
            // Seed default roles for Users (e.g., Guest and Host)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FullName = "Admin User", Email = "admin@airbnb.com",Password="admin123", Role = UserRole.Host, PhoneNumber = "1234567890",ProfilePictureUrl= "https://th.bing.com/th/id/OIP.g5nSTvKkjrlJL-nvP1LxEwHaHa?w=600&h=600&rs=1&pid=ImgDetMain" }
            );


        }
    }


}

