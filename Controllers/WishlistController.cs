using Microsoft.AspNetCore.Mvc;
using airbnbb.Models;
using airbnbb.Data;
using Microsoft.EntityFrameworkCore;

namespace airbnbb.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Retrieve the UserId from the cookies
            var userIdString = HttpContext.Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect if userId is not valid
            }

            // Check if the user exists in the database
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(); // Return 404 if user not found
            }

            // Retrieve the wishlist for the current user with properties
            var wishlistItems = _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Property)  // Eagerly load the related Property
                .ToList();

            // Ensure that the wishlistItems are populated with the Property data
            if (wishlistItems == null || !wishlistItems.Any())
            {
                return View(new List<Wishlist>());
            }

            // Pass the wishlistItems to the view
            return View(wishlistItems);
        }
        [HttpPost]
        public IActionResult AddToWishlist([FromBody] WishlistRequest request)
        {
            // Get the UserId from the cookies
            var userIdString = HttpContext.Request.Cookies["userId"];
            Console.WriteLine($"UserId: {userIdString}");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect if userId is not valid
            }

            // Add the property to the wishlist
            var success = AddPropertyToWishlist(userId, request.PropertyId);
            string message = success ? "Property added to wishlist!" : "Failed to add property to wishlist.";

            // Optionally, set a flash message for the user or handle errors
            TempData["Message"] = message;

            return RedirectToAction("Index"); // Redirect to the wishlist page after the action
        }

        private bool AddPropertyToWishlist(int userId, int propertyId)
        {
            // Check if the property already exists in the wishlist
            var existingWishlistItem = _context.Wishlists
                .FirstOrDefault(w => w.UserId == userId && w.PropertyId == propertyId);

            if (existingWishlistItem != null)
            {
                Console.WriteLine("Property already in wishlist.");

                // The property is already in the wishlist, no need to add it again
                return false;
            }

            // Create a new wishlist item
            var wishlistItem = new Wishlist
            {
                UserId = userId,
                PropertyId = propertyId
            };

            _context.Wishlists.Add(wishlistItem);
            try
            {
                _context.SaveChanges();
                Console.WriteLine("Wishlist item added successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding wishlist item: {ex.Message}");
                return false;
            }


            _context.SaveChanges();  // Commit the changes to the database

            return true;
        }
        public class WishlistRequest
        {
            public int PropertyId { get; set; }
        }

        public IActionResult GetWishlist(int userId)
        {
            // Ensure proper relationship setup in OnModelCreating
            var wishlistItems = _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Property)  // Eagerly load the related Property
                .ToList();

            // Optionally check if the query result is empty or null
            if (!wishlistItems.Any())
            {
                // Handle the case where no wishlist items were found
                return NotFound(); // Or return a suitable view with a message
            }

            return View(wishlistItems);
        }
       

    

    }



}
