using System.ComponentModel.DataAnnotations;

namespace SSO.Models
{
    public class RegisterUserModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Full name required")]
        [MaxLength(255, ErrorMessage = "Ful name cannot more than 255 characters")]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        [MaxLength(255, ErrorMessage = "Email cannot more than 255 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MaxLength(255, ErrorMessage = "Password cannot more than 255 characters")]
        [MinLength(8, ErrorMessage = "Mininum 8 characters for password required")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [MaxLength(255, ErrorMessage = "Password cannot more than 255 characters")]
        [MinLength(8, ErrorMessage = "Mininum 8 characters for password required")]
        public string ComfirmPassword { get; set; }
    }
}
