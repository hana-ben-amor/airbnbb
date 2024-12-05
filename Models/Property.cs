using System.ComponentModel.DataAnnotations;

namespace airbnbb.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
        public PropertyStatus Status { get; set; } // Enum: Available, Booked

        // New Category Property
        public string Category { get; set; }

        // Relationship: Property belongs to one User (Host)
        public int HostId { get; set; } // Foreign Key to User
        public User Host { get; set; } // Navigation property
        public double Rating { get; set; }
        public ICollection<Amenity> Amenities { get; set; }
        public ICollection<Review> Reviews { get; set; }
        // Relationship with Wishlist (Many-to-Many)

        public ICollection<Wishlist> Wishlists { get; set; }



    }
}
