using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Auth;

namespace UserService.Helpers.Auth
{
    public interface IUserHelper
    {
        User Get(LoginRequest request);
        User Get(Guid userId);
        User Get(string userId, out string errorString);
        string GetFullName(Guid userId);
        string GetFullNameByInterviewSolutionId(Guid interviewSolutionId);
        bool GetFirstNameAndSurname(User user, out string firstName, out string surname);
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
    }
}