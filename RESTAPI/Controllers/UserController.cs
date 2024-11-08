using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Data;
using RESTAPI.Models;
using RESTAPI.Validators;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(AppDbContext db) : ControllerBase
{
    private async Task<IActionResult?> ValidateUserNameAsync(string userName)
    {
        var validator = new UserValidator();
        var result = await validator.ValidateAsync(new User { Name = userName });

        if (!result.IsValid)
        {
            return BadRequest(result.Errors.Select(e => e.ErrorMessage));
        }

        return null;
    }
    private async Task<Currency?> GetCurrencyAsync(int? currencyId)
    {
        if (currencyId.HasValue)
        {
            var currency = await db.Currencies.FindAsync(currencyId.Value);
            if (currency == null)
            {
                return null;
            }
            return currency;
        }

        return await db.Currencies.FirstOrDefaultAsync(c => c.Id == 1);
    }
    
    [HttpGet("/user/{id}")]
    public async Task<ActionResult<User>> GetUserByIdAsync(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }
        return Ok(user);
    }
    
    [HttpDelete("/user/{id}")]
    public async Task<IActionResult> DeleteUserByIdAsync(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("/user")]
    public async Task<IActionResult> AddUserAsync(string userName, int? currencyId)
    {
        var validationResult = await ValidateUserNameAsync(userName);
        if (validationResult != null) return validationResult;

        var currency = await GetCurrencyAsync(currencyId);
        if (currency == null)
        {
            return BadRequest("Invalid currency ID or default currency not available.");
        }

        var user = new User
        {
            Name = userName,
            CurrencyId = currency.Id
        };

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        return Ok("User successfully added.");
    }
    
    [HttpGet("/users")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync()
    {
        var users = await db.Users.ToListAsync();
        return Ok(users);
    }
}