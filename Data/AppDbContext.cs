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
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<UserProperty> UserProperties { get; set; } // Add this for the join entity

        // Configuring relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many relationship between Property and Amenity
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Amenities)
                .WithMany(a => a.Properties)
                .UsingEntity<Dictionary<string, object>>(
                    "PropertyAmenity",
                    j => j.HasOne<Amenity>().WithMany().HasForeignKey("AmenityId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Property>().WithMany().HasForeignKey("PropertyId").OnDelete(DeleteBehavior.Cascade)
                );

            // Many-to-Many relationship between User and Property through UserProperty
            modelBuilder.Entity<UserProperty>()
                .HasKey(up => new { up.UserId, up.PropertyId });

            modelBuilder.Entity<UserProperty>()
                .HasOne(up => up.User)
                .WithMany(u => u.Wishlists) // Wishlist relationship with User
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserProperty>()
                .HasOne(up => up.Property)
                .WithMany(p => p.Wishlists) // Wishlist relationship with Property
                .HasForeignKey(up => up.PropertyId);

            // Seed default roles for Users (e.g., Guest and Host)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FullName = "Admin User", Email = "admin@airbnb.com",Password="admin123", Role = UserRole.Host, PhoneNumber = "1234567890",ProfilePictureUrl= "https://th.bing.com/th/id/OIP.g5nSTvKkjrlJL-nvP1LxEwHaHa?w=600&h=600&rs=1&pid=ImgDetMain" }
            );
        }
    }


}

