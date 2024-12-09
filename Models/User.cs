using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace airbnbb.Models
{
    public class User
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // For storing encrypted passwords
        public UserRole Role { get; set; } // "Guest" or "Host" or "Admin"
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; } // Optional

        public ICollection<Booking> Bookings { get; set; } // As Guest
        public ICollection<Wishlist> Wishlists { get; set; }
    
        public ICollection<Property> Properties { get; set; } = new List<Property>(); // One-to-Many with Properties

        public ICollection<Review> Reviews { get; set; } = new List<Review>(); // A user can write many reviews


    }

}
