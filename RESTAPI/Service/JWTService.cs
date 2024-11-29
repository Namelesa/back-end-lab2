using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RESTAPI.Data;
using RESTAPI.Models;
using RESTAPI.Models.JWTUser;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace RESTAPI.Service;

public class JWTService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _passwordHasher;

    public JWTService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return null;
        
        var userAccount = await _db.Users.FirstOrDefaultAsync(u => u.Name == request.UserName);
        if (userAccount == null || string.IsNullOrWhiteSpace(userAccount.Password))
            return null;
        
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(userAccount, userAccount.Password, request.Password);
        if (passwordVerificationResult != PasswordVerificationResult.Success)
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
    
    public async Task<string?> AddUserWithCurrencyAsync(RegisterRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            return "User name cannot be empty.";

        var existingUser = _db.Users.FirstOrDefault(u => u.Name == request.UserName);
        if (existingUser != null)
            return "A user with this name already exists.";

        var currency = await GetCurrencyAsync(request.CurrencyId);
        if (currency == null)
            return "Invalid currency ID or default currency not available.";
        
        var newUser = new User
        {
            Name = request.UserName,
            CurrencyId = currency.Id,
            Password = _passwordHasher.HashPassword(null, request.Password)
        };

        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();

        return null; 
    }

    private async Task<Currency?> GetCurrencyAsync(int? currencyId)
    {
        if (currencyId == 0)
            return await _db.Currencies.FirstOrDefaultAsync(c => c.Name == "USD");
            
        return await _db.Currencies.FindAsync(currencyId);
    }

}
