using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Tasks;

public class TestsCodeDto
{
    [Required]
    public string TestsCode { get; set; }
}