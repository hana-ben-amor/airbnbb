namespace airbnbb.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } // Example: 1-5 stars
        public DateTime Date { get; set; }

        // Relationships
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public int ReviewerId { get; set; }
        public User Reviewer { get; set; }

    }

}
