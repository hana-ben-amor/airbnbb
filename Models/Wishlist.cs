namespace airbnbb.Models;

public class Wishlist
{
    public int Id { get; set; }

    // Relationships
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<UserProperty> UserProperties { get; set; }
}
