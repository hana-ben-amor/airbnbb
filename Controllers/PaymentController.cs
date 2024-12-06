using Microsoft.AspNetCore.Mvc;
using Stripe;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Stripe.Checkout;

public class PaymentController : Controller
{
    private readonly string _stripeSecretKey;

    public PaymentController(IConfiguration configuration)
    {
        // Get Stripe secret key from appsettings
        _stripeSecretKey = configuration["Stripe:SecretKey"];
    }

    public IActionResult PaymentSuccess()
    {
        return View();
    }

}
