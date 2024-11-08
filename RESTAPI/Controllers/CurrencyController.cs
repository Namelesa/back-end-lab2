using Microsoft.AspNetCore.Mvc;
using RESTAPI.Data;
using RESTAPI.Models;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Validators;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CurrencyController: ControllerBase
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
        var validatorString = new StringParamValidator();
        var validationResult = await validatorString.ValidateAsync(name);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        if (await _db.Currencies.AnyAsync(c => c.Name == name))
        {
            return BadRequest("Currency with this name already exists.");
        }

        var currency = new Currency { Name = name };
        _db.Currencies.Add(currency);
        await _db.SaveChangesAsync();
        
        return Ok("Currency added successfully.");
    }
    
    [HttpDelete("/currency")]
    public async Task<IActionResult> DeleteCurrencyAsync(string name)
    {
        var validatorString = new StringParamValidator();
        var validationResult = await validatorString.ValidateAsync(name);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var currency = await _db.Currencies.FirstOrDefaultAsync(c => c.Name == name);
        if (currency == null)
        {
            return NotFound("Currency not found.");
        }

        _db.Currencies.Remove(currency);
        await _db.SaveChangesAsync();
        return Ok("Currency deleted successfully.");
    }
}