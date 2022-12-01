using System;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Draft;
using UserService.DAL.Models.Interfaces;

namespace UserService.Helpers
{
    public interface IDraftHelper
    {
        ReviewerDraft GetReviewerDraft(Guid draftId);
        Draft GetDraft(Guid draftId);
        void PutDraft(Guid draftId, Draft draft);
    }
    
    public class DraftHelper : IDraftHelper
    {
        private readonly IDbRepository dbRepository;

        public DraftHelper(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public ReviewerDraft GetReviewerDraft(Guid draftId)
            => dbRepository
                .Get<ReviewerDraft>(draft => draft.Id == draftId)
                .FirstOrDefault();
        
        public Draft GetDraft(Guid draftId)
            => GetReviewerDraft(draftId)?.Draft;

        public void PutDraft(Guid draftId, Draft draft)
        {
            var reviewerDraft = GetReviewerDraft(draftId);

            reviewerDraft.Draft = draft;
            dbRepository.SaveChangesAsync().Wait();
        }
    }
}