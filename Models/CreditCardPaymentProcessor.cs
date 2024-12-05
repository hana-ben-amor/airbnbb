namespace airbnbb.Models
{

    public class CreditCardPaymentProcessor : IPaymentProcessor
    {
        public Payment ProcessPayment(Payment payment)
        {
            // Simulate Credit Card processing
            Console.WriteLine($"Processing Credit Card payment for Booking ID: {payment.BookingId}, Amount: {payment.Amount:C}");

            // Update payment status
            payment.Status = PaymentStatus.Completed;
            payment.PaymentDate = DateTime.Now;

            return payment;
        }
    }

}
