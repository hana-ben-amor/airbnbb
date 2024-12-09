using airbnbb.Data;
using airbnbb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace airbnbb.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> DashboardAsync()
        { // Fetch data from the database
            var totalRevenue = _context.Bookings.Sum(b => b.TotalPrice);
      
            //var latestReviews = _context.Reviews
            //                .Include(r => r.Reviewer) // Ensure Reviewer is loaded
            //                .Where(r => r.Reviewer != null) // Exclude reviews with no Reviewer
            //                .OrderByDescending(r => r.Date)
            //                .Take(5)
            //                .ToList();
            var latestReviews = _context.Reviews
                    .Include(r => r.Reviewer) // Eager loading
                    .Where(r => r.Reviewer != null) // Ensure reviewer is not null
                    .OrderByDescending(r => r.Date)
                    .Take(5)
                    .ToList();

            var totalBookings = _context.Bookings.Count();
            var confirmedBookings = _context.Bookings.Count(b => b.Status == BookingStatus.Confirmed); // Use the enum value
            var cancelledBookings = _context.Bookings.Count(b => b.Status == BookingStatus.Cancelled); // Use the enum value
            var pendingBookings = _context.Bookings.Count(b => b.Status == BookingStatus.Pending);
            var stats = new BookingStatistic
            {
                TotalRevenue = totalRevenue,
                TotalBookings = totalBookings,
                PendingBookings = pendingBookings,
                ConfirmedBookings = confirmedBookings,
                CancelledBookings = cancelledBookings,
                LatestReviews = latestReviews,
                PropertiesByCategory = new Dictionary<string, int>
        {
            { "Luxury", 10 },
            { "Standard", 25 },
            { "Budget", 40 }
        }

            };
         



            return View(stats);
        }
    }


}
