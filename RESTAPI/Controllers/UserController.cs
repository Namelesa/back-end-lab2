using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Data;
using RESTAPI.Models;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserController(AppDbContext db)
    {
        _db = db;
    }
    
    [HttpGet("/user/{id}")]
    public async Task<ActionResult<User>> GetUserByIdAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }
        return Ok(user);
    }
    
    [HttpDelete("/user/{id}")]
    public async Task<IActionResult> DeleteUserByIdAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("/user")]
    public async Task<IActionResult> AddUserAsync(string userName, int? currencyId)
    {
        Currency? currency = null;
        if (currencyId.HasValue)
        {
            currency = await _db.Currencies.FindAsync(currencyId.Value);
            if (currency == null)
            {
                return BadRequest("Invalid currency ID.");
            }
        }
        else
        {
            currency = await _db.Currencies.FirstOrDefaultAsync(c => c.Name == "USD");
            if (currency == null)
            {
                return BadRequest("Default currency is not available. Please specify a currency.");
            }
        }

        var user = new User
        {
            Name = userName,
            CurrencyId = currency.Id
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return Ok("Add a new User");

    }

    
    [HttpGet("/users")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync()
    {
        var users = await _db.Users.ToListAsync();
        return Ok(users);
    }
}