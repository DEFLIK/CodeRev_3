using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Auth;

public class UserVkRegistration
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string Surname { get; set; }
        
    [Required]
    public string VkDomainLink { get; set; }
    [Required]
    public string VkId { get; set; }
    
    [Required]
    public VkSession VkSession { get; set; }
}