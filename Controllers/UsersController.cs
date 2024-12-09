using airbnbb.Data;
using airbnbb.Models; // Import the User model and any other models you need
using airbnbb.Services; // Import the DB context or services
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace airbnbb.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public UsersController(AppDbContext dbContext, IConfiguration configuration,UserService userService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync(); // Fetch users
            return View(users); // Pass the users list to the view
        }




        [HttpPost]
        public IActionResult AddUser(User model)
        {
            if (ModelState.IsValid)
            {
                // Save the user to the database (assuming you have a DbContext called _context)
                _dbContext.Users.Add(model);
                _dbContext.SaveChanges();

                // Set success message for display in the view
                TempData["SuccessMessage"] = "User added successfully!";

                // You may return to the current view or return JSON response for a better user experience
                return RedirectToAction("Index"); // Replace with the correct action if necessary
            }

            // If model is not valid, return to the same view with error messages
            return View(model);
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest registerRequest)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(registerRequest); // Return the form with validation errors
            //}

            //// Check if email already exists
            //var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == registerRequest.Email);
            //if (existingUser != null)
            //{
            //    ModelState.AddModelError("Email", "Email is already registered.");
            //    return View(registerRequest);
            //}

            //// Create a new User instance
            //var newUser = new User
            //{
            //    FullName = registerRequest.FullName,
            //    Email = registerRequest.Email,
            //    Password = registerRequest.Password, // In a real-world app, hash the password before storing
            //    PhoneNumber = registerRequest.PhoneNumber,
            //    ProfilePictureUrl = registerRequest.ProfilePictureUrl,
            //    Role = registerRequest.Role
            //};

            //// Save the new user to the database
            //_dbContext.Users.Add(newUser);
            //_dbContext.SaveChanges();

            //// Redirect to login after successful registration
            //TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Index","Home");
        }
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Delete the token cookie by setting its expiration to a past date
            HttpContext.Response.Cookies.Delete("token");

            // Redirect to the home page
            return RedirectToAction("Index", "Home");
        }




        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

 

        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequest); // Return the form with errors
            }

            // Authenticate user
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == loginRequest.Email && x.Password == loginRequest.Password);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid login credentials.";

                return View(loginRequest);
            }

            // Generate JWT token

            var claims = new[]
           {

                new Claim(JwtRegisteredClaimNames.Sub, "JwtSubject"),

                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("Email", user.Email),
                new Claim("UserId",user.Id.ToString()),
                new Claim("Name", user.FullName),
                new Claim("Role", user.Role.ToString())
            };
            HttpContext.Response.Cookies.Append("Role", user.Role.ToString(),
                 new CookieOptions
                 {
                     Expires = DateTime.UtcNow.AddMinutes(60), // Set the expiration time
                     HttpOnly = true, // Optional: Makes the cookie inaccessible to JavaScript
                     Secure = true, // Optional: Makes the cookie sent over HTTPS only
                     SameSite = SameSiteMode.Strict // Optional: Prevents sending the cookie with cross-site requests
                 });
            HttpContext.Response.Cookies.Append(
     "userId",
     user.Id.ToString(),
     new CookieOptions
     {
         Expires = DateTime.UtcNow.AddMinutes(60), // Set the expiration time
         HttpOnly = true, // Optional: Makes the cookie inaccessible to JavaScript
         Secure = true, // Optional: Makes the cookie sent over HTTPS only
         SameSite = SameSiteMode.Strict // Optional: Prevents sending the cookie with cross-site requests
     });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signingCredentials
            );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
  
            HttpContext.Response.Cookies.Append("token", tokenValue,
    new Microsoft.AspNetCore.Http.CookieOptions
    {
        HttpOnly = true,
        Secure = true, // Enable in production with HTTPS
        Expires = DateTime.Now.AddMinutes(60)
    });

            TempData["SuccessMessage"] = "Login successful!";
            if (user.Role.HasFlag(UserRole.Admin))
            {
                return RedirectToAction("Dashboard", "Admin"); // Redirect to Admin dashboard
            }
            return RedirectToAction("Index","Home");
        }
    }
}