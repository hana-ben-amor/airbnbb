namespace airbnbb.Models;

public class UserProperty
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int PropertyId { get; set; }
    public Property Property { get; set; }
}
