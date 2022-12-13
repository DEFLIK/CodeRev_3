using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;

namespace UserService.Helpers.Auth.Invitations
{
    public interface IInvitationValidator
    {
        Invitation Validate(string invitationId, out string errorString);
    }

    public class InvitationValidator : IInvitationValidator
    {
        private readonly IDbRepository dbRepository;

        public InvitationValidator(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public Invitation Validate(string invitationId, out string errorString)
        {
            (var invitationGuid, errorString) = GuidParser.TryParse(invitationId, nameof(invitationId));
            if (errorString != null)
                return null;
            
            var invitation = dbRepository
                .Get<Invitation>(i => i.Id == invitationGuid)
                .FirstOrDefault();

            if (invitation == null)
            {
                errorString = $"invitation {invitationGuid} doesn't exist";
                return null;
            }
            
            if (invitation.ExpiredAt < DateTimeOffset.Now.ToUnixTimeMilliseconds())
            {
                dbRepository.Remove(invitation).Wait();
                dbRepository.SaveChangesAsync().Wait();
                
                errorString = $"invitation {invitationGuid} is expired";
                return null;
            }

            return invitation;
        }
    }
}