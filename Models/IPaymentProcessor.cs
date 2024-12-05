namespace airbnbb.Models
{

    public interface IPaymentProcessor
    {
        Payment ProcessPayment(Payment payment);
    }

}
