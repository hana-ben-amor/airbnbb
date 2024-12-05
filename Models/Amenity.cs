namespace airbnbb.Models
{
    public class Amenity
    {
        public int Id { get; set; }
    
        public required string Name { get; set; }
        public required string IconUrl { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();

    }

}
