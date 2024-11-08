using Microsoft.AspNetCore.Mvc;
using RESTAPI.Data;
using RESTAPI.Models;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Validators;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CurrencyController(AppDbContext db, StringParamValidator stringParamValidator) : ControllerBase
{
    [HttpGet("/currencies")]
    public async Task<ActionResult<IEnumerable<Currency>>> GetAllCurrenciesAsync()
    {
        var currencies = await db.Currencies.ToListAsync();
        return Ok(currencies);
    }

    [HttpPost("/currency")]
    public async Task<IActionResult> AddCurrencyAsync(string name)
    {
        var validationResult = await stringParamValidator.ValidateAsync(name);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        if (await db.Currencies.AnyAsync(c => c.Name == name))
        {
            return BadRequest("Currency with this name already exists.");
        }

        var currency = new Currency { Name = name };
        db.Currencies.Add(currency);
        await db.SaveChangesAsync();
        
        return Ok("Currency added successfully.");
    }
    
    [HttpDelete("/currency")]
    public async Task<IActionResult> DeleteCurrencyAsync(string name)
    {
        var validationResult = await stringParamValidator.ValidateAsync(name);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var currency = await db.Currencies.FirstOrDefaultAsync(c => c.Name == name);
        if (currency == null)
        {
            return NotFound("Currency not found.");
        }

        db.Currencies.Remove(currency);
        await db.SaveChangesAsync();
        return Ok("Currency deleted successfully.");
    }
}