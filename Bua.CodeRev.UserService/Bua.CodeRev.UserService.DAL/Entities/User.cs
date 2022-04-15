using Bua.CodeRev.UserService.DAL.Models;

namespace Bua.CodeRev.UserService.DAL.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public RoleEnum Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}