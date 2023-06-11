namespace UserService.Models.Auth;

public class VkSession
{
    public int Expire { get; set; }
    public string Mid { get; set; }
    public string Secret { get; set; }
    public string Sid { get; set; }
    public string Sig { get; set; }
}