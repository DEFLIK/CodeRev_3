using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        // [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}