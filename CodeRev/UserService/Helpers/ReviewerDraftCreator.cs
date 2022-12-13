using System;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;

namespace UserService.Helpers
{
    public interface IReviewerDraftCreator
    {
        Guid Create(Guid interviewSolutionId);
    }

    public class ReviewerDraftCreator : IReviewerDraftCreator
    {
        private readonly IDbRepository dbRepository;

        public ReviewerDraftCreator(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }
        
        public Guid Create(Guid interviewSolutionId)
        {
            var reviewerDraftId = Guid.NewGuid();
            dbRepository.Add(new ReviewerDraft
            {
                Id = reviewerDraftId,
                InterviewSolutionId = interviewSolutionId,
                Draft = null
            }).Wait();

            dbRepository.SaveChangesAsync().Wait();
            
            return reviewerDraftId;
        }
    }
}