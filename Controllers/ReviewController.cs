using Microsoft.AspNetCore.Mvc;
using airbnbb.Models; // Adjust namespace
using System.Linq;
using airbnbb.Data;

namespace airbnbb.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SubmitReview(int propertyId, int rating, string comment)
        {
            // Retrieve user ID from cookies
            if (!Request.Cookies.TryGetValue("UserId", out string userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not logged in.");
            }

            // Validate the property exists
            var property = _context.Properties.SingleOrDefault(p => p.Id == propertyId);
            if (property == null)
            {
                return NotFound("Property not found.");
            }

            // Create a new review
            var review = new Review
            {
                PropertyId = propertyId,
                ReviewerId = userId,
                Rating = rating,
                Comment = comment,
                Date = DateTime.Now
            };

            // Add and save the review
            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Details", "Property", new { id = propertyId });
        }
    }
}
