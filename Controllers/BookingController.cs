using airbnbb.Data;
using airbnbb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace airbnbb.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // Existing BookProperty method
        [HttpPost]
        public async Task<IActionResult> BookProperty(int propertyId, DateTime checkinDate, DateTime checkoutDate)
        {
            // Retrieve user ID from cookies
            if (!Request.Cookies.TryGetValue("UserId", out string userIdString) || !int.TryParse(userIdString, out int guestId))
            {
                return Unauthorized("User not logged in.");
            }

            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);
            if (property == null)
            {
                TempData["ErrorMessage"] = "The property is no longer available.";
                return RedirectToAction("Index", "Home");
            }

            var existingBooking = _context.Bookings
                .Where(b => b.PropertyId == propertyId &&
                            ((checkinDate >= b.CheckInDate && checkinDate < b.CheckOutDate) ||
                             (checkoutDate > b.CheckInDate && checkoutDate <= b.CheckOutDate)))
                .Any();

            if (existingBooking)
            {
                TempData["ErrorMessage"] = "The property is already booked for the selected dates.";
                return RedirectToAction("Index", "Home");
            }

            var pricePerNight = property.PricePerNight;
            var totalPrice = (checkoutDate - checkinDate).Days * pricePerNight;
            var booking = new Booking
            {
                PropertyId = propertyId,
                GuestId = guestId,  // Use the guestId obtained from the cookies
                CheckInDate = checkinDate,
                CheckOutDate = checkoutDate,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending
            };

            if (booking == null)
            {
                return RedirectToAction("Index", "Home");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    property.Status = PropertyStatus.Booked;
                    _context.Update(property);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();  // Commit transaction
                    TempData["SuccessMessage"] = "Your booking has been successfully placed!";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "There was an error processing your booking: " + ex.Message;
                    Console.WriteLine(ex);  // Log the exception to the console for debugging
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        // New Index method to display the user's bookings
        public async Task<IActionResult> Index()
        {
            // Retrieve user ID from cookies
            if (!Request.Cookies.TryGetValue("UserId", out string userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "User not logged in.";
                return RedirectToAction("Login", "Account"); // Redirect to login if userId is not found in cookies
            }

            // Fetch the bookings for the current user
            var bookings = await _context.Bookings
                .Where(b => b.GuestId == userId)
                .Include(b => b.Property)  // Include the related Property details
                .ToListAsync();

            // Pass the bookings to the view
            return View(bookings);
        }
    }
}
