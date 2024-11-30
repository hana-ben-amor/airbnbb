namespace airbnbb.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public PaymentMethod Method { get; set; } // Enum: CreditCard, PayPal, etc.
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; } // Enum: Pending, Completed, Failed

        // Relationships
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }

}
