using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.Core.Models
{
    public class UserRegistration
    {
        [Required]
        public string FullName { get; set; }
        public RoleEnum Role { get; set; } //todo how to make required
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
    }
}