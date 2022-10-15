using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Auth;

namespace UserService.Helpers
{
    public interface IUserHelper
    {
        User Get(LoginRequest request);
    }
    
    public class UserHelper : IUserHelper
    {
        private readonly IDbRepository dbRepository;

        public UserHelper(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public User Get(LoginRequest request)
        {
            return dbRepository
                .Get<User>(user => user.Email == request.Email && user.PasswordHash == request.PasswordHash)
                .FirstOrDefault();
        }
    }
}