using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SSO.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Email cannot more than 255 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Password cannot more than 255 characters")]
        [MinLength(8, ErrorMessage = "Mininum 8 characters for password required")]
        public string Password { get; set; }
    }
}
