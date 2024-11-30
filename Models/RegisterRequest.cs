namespace airbnbb.Models
{
    public class RegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }

        // New Role Property for selecting Guest or Host
        public UserRole Role { get; set; }
    }
}
