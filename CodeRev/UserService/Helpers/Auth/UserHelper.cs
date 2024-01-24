using System;
using System.Linq;
using System.Threading.Tasks;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Auth;
using Task = System.Threading.Tasks.Task;

namespace UserService.Helpers.Auth
{
    public interface IUserHelper
    {
        User Get(LoginRequest request);
        User Get(Guid userId);
        User Get(string userId, out string errorString);
        User[] GetAll();
        string GetFullName(Guid userId);
        string GetFullNameByInterviewSolutionId(Guid interviewSolutionId);
        bool GetFirstNameAndSurname(User user, out string firstName, out string surname);
        Task EditUser(string userEmail, string? userName, Role? userRole);
        System.Threading.Tasks.Task<Guid> DeleteUser(string userEmail);
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

        public User Get(Guid userId)
            => dbRepository
                .Get<User>(u => u.Id == userId)
                .FirstOrDefault();

        public User Get(string userId, out string errorString)
        {
            (var userGuid, errorString) = GuidParser.TryParse(userId, nameof(userId));
            return errorString == null ? Get(userGuid) : null;
        }

        public User[] GetAll() 
            => dbRepository
              .GetAll<User>()
              .ToArray();

        public string GetFullName(Guid userId)
            => Get(userId)?.FullName;

        public string GetFullNameByInterviewSolutionId(Guid interviewSolutionId)
        {
            var interviewSolution = dbRepository
                .Get<InterviewSolution>(i => i.Id == interviewSolutionId)
                .FirstOrDefault(); // чтобы не было циклической зависимости при создании interviewHelper, пришлось так доставать interviewSolution
            return interviewSolution == null ? null : GetFullName(interviewSolution.UserId);
        }

        public bool GetFirstNameAndSurname(User user, out string firstName, out string surname)
        {
            var splitFullName = GetFullName(user.Id).Split(' ');
            firstName = splitFullName.FirstOrDefault();
            surname = splitFullName.ElementAtOrDefault(1);
            
            return true;
        }

        public async Task EditUser(string userEmail, string? userName, Role? userRole)
        {
            var user = dbRepository.Get<User>(user => user.Email == userEmail).FirstOrDefault();
            if (user == null) return;
            
            if (userRole != null) 
                user.Role = userRole.Value;
            if (userName != null)
                user.FullName = userName;
            
            await dbRepository.Update(user);
            await dbRepository.SaveChangesAsync();
        }

        public async Task<Guid> DeleteUser(string userEmail)
        {
            var user = dbRepository.Get<User>(user => user.Email == userEmail).FirstOrDefault();
            if (user == null) return Guid.Empty;

            await dbRepository.Remove(user);
            await dbRepository.SaveChangesAsync();
            return user.Id;
        }
    }
}