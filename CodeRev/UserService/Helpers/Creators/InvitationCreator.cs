using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Users;

namespace UserService.Helpers.Creators
{
    public interface IInvitationCreator
    {
        Invitation Create(InvitationParams invitationParams, out string errorString);
    }
    
    public class InvitationCreator : IInvitationCreator
    {
        private const long InvitationDurationMs = 604800000; // == 1 week //todo make config setting
        private readonly IDbRepository dbRepository;

        public InvitationCreator(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public Invitation Create(InvitationParams invitationParams, out string errorString)
        {
            if (!Enum.TryParse(invitationParams.Role, true, out Role roleEnum))
            {
                errorString = "role is invalid";
                return null;
            }
            
            var mustBeInterviewId = invitationParams.InterviewId != null || roleEnum == Role.Candidate;
            var interviewGuid = Guid.Empty;
            if (mustBeInterviewId)
            {
                (interviewGuid, errorString) = GuidParser.TryParse(invitationParams.InterviewId, nameof(invitationParams.InterviewId));
                if (errorString != null)
                    return null;
            }

            if (mustBeInterviewId && dbRepository
                    .Get<Interview>(i => i.Id == interviewGuid)
                    .FirstOrDefault() == null)
            {
                errorString = "no interview with such id";
                return null;
            }

            var invitation = dbRepository
                .Get<Invitation>(i => i.Role == roleEnum && i.InterviewId == interviewGuid)
                .FirstOrDefault();

            if (invitation == null)
            {
                invitation = new Invitation
                {
                    Id = Guid.NewGuid(),
                    Role = roleEnum,
                    InterviewId = interviewGuid,
                    ExpiredAt = DateTimeOffset.Now.ToUnixTimeMilliseconds() + InvitationDurationMs
                };
                dbRepository.Add(invitation).Wait();
            }
            else
            {
                invitation.ExpiredAt = DateTimeOffset.Now.ToUnixTimeMilliseconds() + InvitationDurationMs;
            }
            
            dbRepository.SaveChangesAsync().Wait();

            errorString = null;
            return invitation;
        }
    }
}