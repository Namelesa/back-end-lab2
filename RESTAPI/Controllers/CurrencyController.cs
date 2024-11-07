using Microsoft.AspNetCore.Mvc;
using RESTAPI.Data;
using RESTAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CurrencyController : ControllerBase
{
    private readonly AppDbContext _db;

    public CurrencyController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("/currencies")]
    public async Task<ActionResult<IEnumerable<Currency>>> GetAllCurrenciesAsync()
    {
        var currencies = await _db.Currencies.ToListAsync();
        return Ok(currencies);
    }

    [HttpPost("/currency")]
    public async Task<IActionResult> AddCurrencyAsync(string name)
    {
        if (await _db.Currencies.AnyAsync(c => c.Name == name))
        {
            return BadRequest("Currency with this name already exists.");
        }

        var currency = new Currency { Name = name };
        await _db.Currencies.AddAsync(currency);
        await _db.SaveChangesAsync();

        return Ok("Add a new Currency");
    }
    
    [HttpDelete("/currency")]
    public async Task<IActionResult> DeleteCurrencyAsync(string name)
    {
        var currency = _db.Currencies.FirstOrDefault(u => u.Name == name);
        if (currency == null)
        {
            return NotFound("Currency with this name is not found");
        }
        _db.Currencies.Remove(currency);
        await _db.SaveChangesAsync();
        return Ok("Delete is Ok");
    }
}