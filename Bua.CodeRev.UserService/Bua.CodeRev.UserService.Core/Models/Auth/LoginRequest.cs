using System.ComponentModel.DataAnnotations;

namespace Bua.CodeRev.UserService.Core.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}