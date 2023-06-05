using System.ComponentModel.DataAnnotations;

namespace SchoolInfoMVC.Models
{
    public class UserVM
    {
        [Required(ErrorMessage = "The username field is required!")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The username can be between 3 and 20 characters long.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "The password field is required!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The password can be between 6 and 100 characters long.")]
        public required string Password { get; set; }
    }
}
