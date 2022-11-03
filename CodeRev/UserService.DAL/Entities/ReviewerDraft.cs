using System;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.DAL.Models.Draft;

namespace UserService.DAL.Entities
{
    public class ReviewerDraft : BaseEntity
    {
        public Guid InterviewSolutionId { get; set; }
        public Draft Draft { get; set; }
    }
}