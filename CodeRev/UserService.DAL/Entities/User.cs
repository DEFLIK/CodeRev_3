using UserService.DAL.Models.Enums;

namespace UserService.DAL.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public Role Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}