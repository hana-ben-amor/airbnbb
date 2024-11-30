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
        public ICollection<Property> Properties { get; set; } // As Host
        public ICollection<UserProperty> Wishlists { get; set; } 
    }

}
