namespace airbnbb.Models
{
    public class BookingStatistic
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int PendingBookings { get; set; }
        public Dictionary<string, int> PropertiesByCategory { get; set; }
        public List<Review> LatestReviews { get; set; } // Add this property


    }
}
