using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Users
{
    public class InvitationParams
    {
        [Required]
        public string Role { get; set; }
        public string InterviewId { get; set; }
        public bool IsSynchronous { get; set; }
    }
}