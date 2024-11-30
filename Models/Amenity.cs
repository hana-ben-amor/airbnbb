namespace airbnbb.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; } // Example: "WiFi", "Air Conditioning"
       
        public string IconUrl { get; set; } // Optional


        public ICollection<Property> Properties { get; set; } = new List<Property>();

    }

}
