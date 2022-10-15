using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Auth;

namespace UserService.Helpers.Creators
{
    public interface IUserCreator
    {
        User Create(UserRegistration userRegistration, string invitationId, out string errorString);
    }
    
    public class UserCreator : IUserCreator
    {
        private readonly IDbRepository dbRepository;
        private readonly IInterviewCreator interviewCreator;
        private readonly IInvitationValidator invitationValidator;

        public UserCreator(IDbRepository dbRepository, IInterviewCreator interviewCreator, IInvitationValidator invitationValidator)
        {
            this.dbRepository = dbRepository;
            this.interviewCreator = interviewCreator;
            this.invitationValidator = invitationValidator;
        }

        public User Create(UserRegistration userRegistration, string invitationId, out string errorString)
        {
            var invitation = invitationValidator.Validate(invitationId, out errorString);
            if (errorString != null)
                return null;

            if (dbRepository.Get<User>(user => user.Email == userRegistration.Email || user.PhoneNumber == userRegistration.PhoneNumber).Any())
            {
                errorString = "email or phone number is already registered";
                return null;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Role = invitation.Role,
                Email = userRegistration.Email,
                PasswordHash = userRegistration.PasswordHash,
                FullName = userRegistration.FullName,
                PhoneNumber = userRegistration.PhoneNumber
            };
            
            dbRepository.Add(user).Wait();

            if (invitation.Role != RoleEnum.Candidate)
                dbRepository.Remove(invitation).Wait();

            if (!invitation.InterviewId.Equals(Guid.Empty))
                interviewCreator.CreateSolution(user.Id, invitation.InterviewId);

            dbRepository.SaveChangesAsync();

            errorString = null;
            return user;
        }
    }
}