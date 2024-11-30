using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using airbnbb.Models; // Import the User model and any other models you need
using airbnbb.Data;
using Microsoft.AspNetCore.Authorization;
using airbnbb.Services; // Import the DB context or services

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




 

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registerRequest); // Return the form with validation errors
            }

            // Check if email already exists
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == registerRequest.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(registerRequest);
            }

            // Create a new User instance
            var newUser = new User
            {
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                Password = registerRequest.Password, // In a real-world app, hash the password before storing
                PhoneNumber = registerRequest.PhoneNumber,
                ProfilePictureUrl = registerRequest.ProfilePictureUrl,
                Role = registerRequest.Role
            };

            // Save the new user to the database
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            // Redirect to login after successful registration
            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToAction("Login", "Users"); // Redirect to home or login
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
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View(loginRequest);
            }

            // Generate JWT token

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signingCredentials
            );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("Token", tokenValue);
            return RedirectToAction("Index");
        }
    }
}