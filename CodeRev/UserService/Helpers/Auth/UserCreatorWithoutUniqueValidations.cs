using System;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers.Auth.Invitations;
using UserService.Helpers.Interviews;
using UserService.Models.Auth;

namespace UserService.Helpers.Auth
{
    public class UserCreatorWithoutUniqueValidations : IUserCreator
    {
        private readonly IDbRepository dbRepository;
        private readonly IInterviewCreator interviewCreator;
        private readonly IInvitationValidator invitationValidator;

        public UserCreatorWithoutUniqueValidations(IDbRepository dbRepository, IInterviewCreator interviewCreator, IInvitationValidator invitationValidator)
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

            var fullName = userRegistration.FirstName + ' ' + userRegistration.Surname;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Role = invitation.Role,
                Email = userRegistration.Email,
                PasswordHash = userRegistration.PasswordHash,
                FullName = fullName,
                PhoneNumber = userRegistration.PhoneNumber
            };
            
            dbRepository.Add(user).Wait();

            if (invitation.Role != Role.Candidate)
                dbRepository.Remove(invitation).Wait();

            if (!invitation.InterviewId.Equals(Guid.Empty))
                interviewCreator.CreateSolution(user.Id, invitation.InterviewId, invitation.CreatedBy, invitation.IsSynchronous);

            dbRepository.SaveChangesAsync().Wait();

            errorString = null;
            return user;
        }

        public User Create(UserVkRegistration userVkRegistration, string invitationId, out string errorString)
        {
            var userRegistration = new UserRegistration // todo Пока так. Нужно подумать над внедрением этого в БД
                                                        // todo Мб переделать email на contact, а phone опциональным (или удалить к чертям)?
                                                        // todo Или вообще сделать две сущности?
            {
                FirstName = userVkRegistration.FirstName,
                Surname = userVkRegistration.Surname,
                Email = userVkRegistration.VkId,
                PasswordHash = TokenHelper.VkMockPassHash,
                PhoneNumber = userVkRegistration.VkDomainLink
            };
            
            return Create(userRegistration, invitationId, out errorString);
        }
    }
}