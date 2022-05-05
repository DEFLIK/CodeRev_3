using System.ComponentModel.DataAnnotations;

namespace Bua.CodeRev.UserService.Core.Models
{
    public class InvitationParams
    {
        [Required]
        public string Role { get; set; }
        public string InterviewId { get; set; }
    }
}