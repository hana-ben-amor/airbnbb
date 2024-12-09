using Microsoft.AspNetCore.Mvc;

namespace airbnbb.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            return View(); // This will render the Admin Dashboard view
        }
    }
}
