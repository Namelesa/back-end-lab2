using System.ComponentModel.DataAnnotations;

namespace RESTAPI.Models.JWTUser;

public class LoginRequestModel
{
    public string? UserName { get; set; }
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}