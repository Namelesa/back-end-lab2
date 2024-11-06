using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models;

public class Currency : BaseModel
{
    [Required]
    public string Name { get; set; }
    
}