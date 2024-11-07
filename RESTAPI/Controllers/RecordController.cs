using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Data;
using RESTAPI.Models;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class RecordController : ControllerBase
{   
    private readonly AppDbContext _db;

    public RecordController(AppDbContext db)
    {
        _db = db;
    }
    
    [HttpGet("/record/{recordId}")]
    public async Task<IActionResult> GetRecordByIdAsync(int recordId)
    {
        var record = await _db.Records
            .Include(r => r.User)
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == recordId);
        
        if (record == null)
        {
            return NotFound($"Record with ID {recordId} not found.");
        }

        return Ok(record);
    }
    
    [HttpDelete("/record/{recordId}")]
    public async Task<IActionResult> DeleteRecordByIdAsync(int recordId)
    {
        var record = await _db.Records
            .Include(r => r.User)
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == recordId);

        if (record == null)
        {
            return NotFound($"Record with ID {recordId} not found.");
        }
        
        _db.Records.Remove(record);
        await _db.SaveChangesAsync();

        return Ok($"Record with ID {recordId} has been successfully deleted.");
    }
    
    [HttpPost("/record")]
    public async Task<IActionResult> AddRecordAsync(int userId, int categoryId, decimal total, int? currencyId = null)
    {
        var user = await _db.Users
            .Include(u => u.Currency)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return BadRequest("Invalid user ID.");
        }

        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == categoryId);
        if (!categoryExists)
        {
            return BadRequest("Invalid category ID.");
        }
        
        Currency? currency;
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
            currency = user.Currency;
        }

        var record = new Record
        {
            UserId = userId,
            CategoryId = categoryId,
            Date = DateTime.UtcNow,
            TotalPrice = total,
            CurrencyId = currency.Id
        };
    
        await _db.Records.AddAsync(record);
        await _db.SaveChangesAsync();

        return Ok("Add a new Record");
    }

    
    [HttpGet("/record")]
    public async Task<IActionResult> GetRecordAsync(int? userId, int? categoryId)
    {
        if (userId == null && categoryId == null)
        {
            return BadRequest("You must provide at least one filter parameter.");
        }

        var filteredRecords = _db.Records
            .Include(r => r.User)
            .Include(r => r.Category)
            .Include(r => r.Currency)
            .AsQueryable();

        if (userId.HasValue)
        {
            filteredRecords = filteredRecords.Where(p => p.UserId == userId);
        }

        if (categoryId.HasValue)
        {
            filteredRecords = filteredRecords.Where(p => p.CategoryId == categoryId);
        }
        
        var results = await filteredRecords.ToListAsync();

        if (!results.Any())
        {
            return NotFound("No records found with the specified criteria.");
        }

        return Ok(results);
    }
}
