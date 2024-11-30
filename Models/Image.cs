namespace airbnbb.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int PropertyId { get; set; }  // Foreign key to Property
        public Property Property { get; set; }  // Navigation property
    }
}
