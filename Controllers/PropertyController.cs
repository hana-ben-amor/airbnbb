using Microsoft.EntityFrameworkCore;

using airbnbb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using airbnbb.Data;

public class PropertyController : Controller
{
    private readonly AppDbContext _context;

    public PropertyController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Property/Details/{id}
    public IActionResult Details(int id)
    {

        var property = _context.Properties
            .Include(p => p.Reviews)  // Eager load reviews
                .ThenInclude(r => r.Reviewer) // Optionally load Reviewer (User)
            .FirstOrDefault(p => p.Id == id);
        if (property == null)
        {
            return NotFound();
        }

        return View(property);
    }



    // Assuming you have a method for handling booking
    [HttpPost]
    public IActionResult BookProperty(DateTime checkinDate, DateTime checkoutDate, int propertyId)
    {
        // Logic for booking the property (e.g., save details to a database or session)

        // Redirect to the checkout page with the relevant data
        return RedirectToAction("Checkout", new { checkinDate = checkinDate, checkoutDate = checkoutDate, propertyId = propertyId });
    }

    // Checkout Page (for displaying the booking details)
    public IActionResult Checkout(DateTime checkinDate, DateTime checkoutDate, int propertyId)
    {
        // Fetch property details based on the propertyId
        var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);
        if (property == null)
        {
            return NotFound();
        }

        // Pass the booking details to the view
        ViewBag.CheckinDate = checkinDate;
        ViewBag.CheckoutDate = checkoutDate;
        ViewBag.Property = property;

        return View();
    }
}
