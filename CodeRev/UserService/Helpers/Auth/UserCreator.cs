﻿using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers.Auth.Invitations;
using UserService.Helpers.Interviews;
using UserService.Models.Auth;

namespace UserService.Helpers.Auth
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
                interviewCreator.CreateSolution(user.Id, invitation.InterviewId, invitation.CreatedBy);

            dbRepository.SaveChangesAsync().Wait();

            errorString = null;
            return user;
        }
    }
}