using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models;

public class Category : BaseModel
{
    [Required]
    public string CategoryName { get; set; }
}