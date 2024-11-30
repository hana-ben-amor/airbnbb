namespace airbnbb.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } // Enum: Pending, Confirmed, Canceled

        // Relationships
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public int GuestId { get; set; }
        public User Guest { get; set; }
        public Payment Payment { get; set; }
    }

}
