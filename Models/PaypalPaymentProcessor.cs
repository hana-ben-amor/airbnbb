namespace airbnbb.Models
{
    public class PaypalPaymentProcessor:IPaymentProcessor
    {
        public Payment ProcessPayment(Payment payment)
        {
            // Simulate PayPal processing
            Console.WriteLine($"Processing PayPal payment for Booking ID: {payment.BookingId}, Amount: {payment.Amount:C}");

            // Update payment status
            payment.Status = PaymentStatus.Completed;
            payment.PaymentDate = DateTime.Now;

            return payment;
        }
    }


}
