using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RESTAPI.Data;
using RESTAPI.Models.JWTUser;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace RESTAPI.Service;

public class JWTService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    
    public JWTService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }
    public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return null;
        var userAccount = _db.Users.FirstOrDefault(u => u.Name == request.UserName);

        if (userAccount is null && userAccount?.Password == request.Password)
            return null;

        var issuer = _config["JWTConfig:Issuer"];
        var audience = _config["JWTConfig:Audience"];
        var key = _config["JWTConfig:Key"];
        var tokenMin = _config.GetValue<int>("JWTConfig:TokenValidityMins");
        var tokenExpiryTimeStap = DateTime.UtcNow.AddMinutes(tokenMin);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, request.UserName)
            }),
            Expires = tokenExpiryTimeStap,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return new LoginResponseModel
        {
            AccessToken = accessToken,
            UserName = request.UserName,
            ExpiresIn = (int)tokenExpiryTimeStap.Subtract(DateTime.UtcNow).TotalSeconds
        };
    }
}