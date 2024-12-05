namespace airbnbb.Models
{

    public class PaymentProcessorFactory
    {
        public static IPaymentProcessor GetPaymentProcessor(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.PayPal => new PaypalPaymentProcessor(),
                PaymentMethod.CreditCard => new CreditCardPaymentProcessor(),
                PaymentMethod.Stripe => new StripePaymentProcessor(),
                _ => throw new ArgumentException("Invalid payment method"),
            };
        }
    }

}
