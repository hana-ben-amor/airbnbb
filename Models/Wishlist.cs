namespace airbnbb.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }    // Foreign key to User
        public int PropertyId { get; set; } // Foreign key to Property

        public User User { get; set; }      // Navigation property to User
        public Property Property { get; set; } // Navigation property to Property
    }


}
