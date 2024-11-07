using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTAPI.Models;

public class User : BaseModel
{
    [Required]
    public string Name { get; set; }
    
    [Display(Name ="Currency")]
    public int? CurrencyId { get; set; }
    [ForeignKey("CurrencyId")]
    public virtual Currency Currency { get; set; }
}