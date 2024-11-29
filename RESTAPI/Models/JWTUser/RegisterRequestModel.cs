using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models.JWTUser;

public class RegisterRequestModel
{
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public int? CurrencyId { get; set; }
}