namespace airbnbb.Models
{
 
    public class StripePaymentProcessor : IPaymentProcessor
    {
        public Payment ProcessPayment(Payment payment)
        {
            // Simulate Stripe processing
            Console.WriteLine($"Processing Stripe payment for Booking ID: {payment.BookingId}, Amount: {payment.Amount:C}");

            // Update payment status
            payment.Status = PaymentStatus.Completed;
            payment.PaymentDate = DateTime.Now;

            return payment;
        }
    }
}
