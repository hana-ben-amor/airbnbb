﻿using Microsoft.EntityFrameworkCore;
using airbnbb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using airbnbb.Data;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Options;
using airbnbb.Services;
using NuGet.Packaging;

public class PropertyController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly StripeSettings _stripeSettings;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly AppDbContext _context;

    public PropertyController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IOptions<StripeSettings> stripeSettings)
    {
        _context = context;
        _configuration = configuration;
        _stripeSettings = stripeSettings.Value;
        _hostingEnvironment = hostingEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var properties = await _context.Properties.ToListAsync(); // Convert DbSet to List
        return View(properties); // Pass the list to the view
    }

    [HttpPost]
    public IActionResult Update(Property model)
    {
      
            var property = _context.Properties.FirstOrDefault(p => p.Id == model.Id);
            if (property != null)
            {
                property.Title = model.Title;
                property.PricePerNight = model.PricePerNight;
                property.Category = model.Category;
                property.Status = model.Status;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Property updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Property not found.";
            }
            return RedirectToAction("Index");
     // Adjust this if you want to return to a different view
    }

    [HttpGet]
    public IActionResult UploadImages(int propertyId)
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> UploadImages(int propertyId, IFormFile[] imageUpload, string imageUrls)
    {
        // Ensure the propertyId is received
        var property = await _context.Properties
    .Include(p => p.Images)  // Eagerly load the Images collection
    .FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property == null)
        {
            return NotFound(); // Handle case where property is not found
        }

        var imageList = new List<Image>();

        // Handle Image URL input (if any)
        if (!string.IsNullOrEmpty(imageUrls))
        {
            var urls = imageUrls.Split('\n');
            foreach (var url in urls)
            {
                if (Uri.IsWellFormedUriString(url.Trim(), UriKind.Absolute))
                {
                    imageList.Add(new Image { Url = url.Trim() });
                }
            }
        }

        // Handle Image File Uploads (Max 3 images)
        if (imageUpload != null && imageUpload.Length > 0)
        {
            foreach (var file in imageUpload)
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    // Save the file to a folder in wwwroot/uploads
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add image file URL to the list
                    imageList.Add(new Image { Url = "/uploads/" + file.FileName });
                }
            }
        }

        // Ensure the total number of images doesn't exceed 3
        if (imageList.Count > 3)
        {
            ModelState.AddModelError("", "You can only upload up to 3 images.");
            return View(); // Return to the view with an error message
        }

        // Save the images for the property (Assuming property.Images is a navigation property)
        property.Images.AddRange(imageList);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = propertyId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property == null)
        {
            return NotFound();
        }

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();

        return Json(new { success = true });  // Return a success response
    }




    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Property model, IFormFile[] imageUpload)
    {
        var imageList = new List<Image>();
        // Initialize a new property object
        var property = new Property
        {
            Title = model.Title,
            Description = model.Description,
            PricePerNight = model.PricePerNight,
            Address = model.Address,
            ImageUrl = model.ImageUrl,
            City = model.City,
            Country = model.Country,
            Capacity = model.Capacity,
            Category = model.Category,
            Status = PropertyStatus.Available,
            HostId = 2, // Replace with the logged-in host's ID
            Rating = 0,
            Images = new List<Image>() // Initialize the collection
        };

        try
        {
            // Add the property to the context and save changes to get the PropertyId
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();  // This saves the property and generates an Id

            //// Now that the property has an Id, we can associate the images
            //if (Images != null && Images.Length > 0)
            //{
            //    foreach (var imageUrl in Images)
            //    {
            //        if (!string.IsNullOrWhiteSpace(imageUrl))
            //        {
            //            var image = new Image
            //            {
            //                Url = imageUrl,
            //                PropertyId = property.Id // Set the PropertyId after the property is saved
            //            };
            //            _context.Images.Add(image);  // Add the image to the context
            //        }
            //    }
            //}
            if (imageUpload != null && imageUpload.Length > 0)
            {
                foreach (var file in imageUpload)
                {
                    if (file.ContentType.StartsWith("image/"))
                    {
                        // Save the file to a folder in wwwroot/uploads
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Add image file URL to the list
                        imageList.Add(new Image { Url = "/uploads/" + file.FileName });
                    }
                }
            }

            // Ensure the total number of images doesn't exceed 3
            if (imageList.Count > 3)
            {
                ModelState.AddModelError("", "You can only upload up to 3 images.");
                return View(); // Return to the view with an error message
            }

            // Save the images for the property (Assuming property.Images is a navigation property)
            property.Images.AddRange(imageList);

            // Save images to the database
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Property created successfully!";
            return RedirectToAction("Index", "Home");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            TempData["ErrorMessage"] = "There was an error creating the property.";
            return View(model);
        }
    }



    // GET: Property/Details/{id}
    public IActionResult Details(int id)
    {
        var property = _context.Properties
            .Include(p => p.Images)  // Eager load images
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
    public async Task<IActionResult> BookProperty(int propertyId, DateTime checkinDate, DateTime checkoutDate)
    {
        // Retrieve user ID from cookies
        if (!Request.Cookies.TryGetValue("userId", out string userIdString) || !int.TryParse(userIdString, out int guestId))
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

        // Create Stripe payment intent
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(totalPrice * 100),  // Stripe expects the amount in cents
            Currency = "eur",  // Set the currency to Euro (EUR)
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() }
            }
        };
        var service = new PaymentIntentService();
        PaymentIntent intent = await service.CreateAsync(options);

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
                return RedirectToAction("Checkout", "Property", new { checkinDate, checkoutDate, propertyId, bookingId = booking.Id });
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

    public IActionResult CreateCheckoutSession(int propertyId,string amount, int bookingId)
    {
        var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);
        var currency = "eur"; // Set the currency to Euro (EUR)
        var successUrl = "https://localhost:7175/Property/Success?session_id={CHECKOUT_SESSION_ID}";

        var cancelUrl = "https://localhost:7196/Home/cancel";
        // Stripe expects the amount as long (in cents)
        var amountInDecimal = decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture); // Parse as decimal
        var amountInCents = (long)(amountInDecimal*1000);
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;


        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string>
                {
                    "card"
                },
            LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount =amountInCents,  // Convert the amount to cents (smallest unit)
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name =property.Title,  // Replace with actual product name
                                Description = property.Description // Replace with actual product description
                            }
                        },
                        Quantity = 1
                    }
                },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>
                {
                    { "bookingId", bookingId.ToString() }  // Ensure this is added here
                }
        };


        var service = new SessionService();
        var session = service.Create(options);

        return Redirect(session.Url); // Redirect to Stripe Checkout
    }



    public async Task<IActionResult> Success(string session_id)
    {
        // Retrieve the session from Stripe to confirm the payment
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

        var service = new SessionService();
        var session = await service.GetAsync(session_id);

        // Extract the bookingId from the session metadata
        var bookingId = int.Parse(session.Metadata["bookingId"]);

        // Retrieve the booking from the database
        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            TempData["ErrorMessage"] = "Booking not found.";
            return RedirectToAction("Index", "Home");
        }

        // Update the booking status to Confirmed
        booking.Status = BookingStatus.Confirmed;

        // Save the updated booking
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();

        // Record the payment in the Payments table
        var payment = new Payment
        {
            Method = PaymentMethod.CreditCard, // Assuming CreditCard for now
            Amount = (decimal)session.AmountTotal / 100, // Amount in the smallest unit (cents)
            PaymentDate = DateTime.Now,
            Status = PaymentStatus.Completed,
            BookingId = booking.Id
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Optionally, you can add any additional logic here (e.g., sending confirmation emails)

        // Redirect to a confirmation page or return a success view
        TempData["SuccessMessage"] = "Your booking has been confirmed!";
        return RedirectToAction("Details", "Property", new { id = booking.PropertyId });
    }

    [HttpGet]
    public IActionResult CheckIfLiked(int propertyId)
    {
        try
        {
            // Retrieve user ID from cookies
            if (!Request.Cookies.TryGetValue("UserId", out string userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User not logged in.");
            }

            // Check if the property is in the user's wishlist
            var isLiked = _context.Wishlists
                .Any(w => w.UserId == userId && w.PropertyId == propertyId);

            return Json(new { isLiked });
        }
        catch (Exception ex)
        {
            // Log the error and return a failure response
            return StatusCode(500, "Internal server error"+ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleWishlist([FromBody] WishlistToggleRequest request)
    {
        // Retrieve user ID from cookies
        if (!Request.Cookies.TryGetValue("UserId", out string userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized("User not logged in.");
        }

        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == request.PropertyId);
        if (property == null)
        {
            return NotFound("Property not found.");
        }

        // Check if the property is already in the user's wishlist
        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.PropertyId == request.PropertyId);

        if (request.IsLiked)
        {
            if (wishlistItem == null)
            {
                // Add the property to the wishlist
                wishlistItem = new Wishlist
                {
                    UserId = userId,
                    PropertyId = request.PropertyId
                };
                _context.Wishlists.Add(wishlistItem);
            }
        }
        else
        {
            if (wishlistItem != null)
            {
                // Remove the property from the wishlist
                _context.Wishlists.Remove(wishlistItem);
            }
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    public IActionResult Cancel()
    {
        return View("Index");
    }

    // Checkout Page (for displaying the booking details)
    public IActionResult Checkout(DateTime checkinDate, DateTime checkoutDate, int propertyId,int bookingId)
    {
        var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);
        var booking = _context.Bookings.FirstOrDefault(p => p.Id == bookingId);
        if (property == null)
        {
            return NotFound();
        }

        ViewBag.CheckinDate = checkinDate;
        ViewBag.CheckoutDate = checkoutDate;
        ViewBag.Property = property;
        ViewBag.Booking = booking;


        return View();
    }
}
