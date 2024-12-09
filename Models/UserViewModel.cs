using System.ComponentModel.DataAnnotations;

namespace airbnbb.Models
{
    public class UserViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        [Url]
        public string ProfilePictureUrl { get; set; }

        [Required]
        public string Role { get; set; }
    }

}
