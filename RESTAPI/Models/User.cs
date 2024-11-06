using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models;

public class User : BaseModel
{
    [Required]
    public string Name { get; set; }
    
    public string Currency { get; set; }
}