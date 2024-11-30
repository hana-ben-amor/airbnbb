using airbnbb.Models;
using Microsoft.AspNetCore.Mvc;

namespace airbnbb.Controllers
{
    public class AmenityController : Controller
    {
        private readonly AmenityService _amenityService;


        public AmenityController(AmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        // GET: Amenity/Index
        public async Task<IActionResult> Index()
        {
            var amenities = await _amenityService.GetAllAmenitiesAsync();
            return View(amenities);
        }

        // GET: Amenity/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var amenity = await _amenityService.GetAmenityByIdAsync(id);
            if (amenity == null)
            {
                return NotFound();
            }
            return View(amenity);
        }

        // GET: Amenity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amenity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IconUrl")] Amenity amenity)
        {
            if (ModelState.IsValid)
            {
                await _amenityService.CreateAmenityAsync(amenity);
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        // GET: Amenity/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var amenity = await _amenityService.GetAmenityByIdAsync(id);
            if (amenity == null)
            {
                return NotFound();
            }
            return View(amenity);
        }

        // POST: Amenity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IconUrl")] Amenity amenity)
        {
            if (id != amenity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _amenityService.UpdateAmenityAsync(amenity);
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }


        // POST: Amenity/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _amenityService.DeleteAmenityAsync(id);
            TempData["SuccessMessage"] = "Amenity deleted successfully.";  // Success message
            return RedirectToAction(nameof(Index));  // Redirect to Index page
        }


        // GET: Amenity/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var amenity = await _amenityService.GetAmenityByIdAsync(id);
            if (amenity == null)
            {
                return NotFound();
            }
            return View(amenity);  // Display the confirmation page
        }



    }
}
