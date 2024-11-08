using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Data;
using RESTAPI.Models;

namespace RESTAPI.Controllers;

[Route("record")]
[ApiController]
public class RecordController(AppDbContext db) : ControllerBase
{
    private async Task<Record?> GetRecordById(int recordId)
    {
        return await db.Records
            .Include(r => r.User)
            .Include(r => r.Category)
            .Include(r => r.Currency)
            .FirstOrDefaultAsync(r => r.Id == recordId);
    }

    [HttpGet("/record/{recordId}")]
    public async Task<IActionResult> GetRecordByIdAsync(int recordId)
    {
        var record = await GetRecordById(recordId);

        if (record == null)
        {
            return NotFound($"Record with ID {recordId} not found.");
        }

        return Ok(record);
    }

    [HttpDelete("/record/{recordId}")]
    public async Task<IActionResult> DeleteRecordByIdAsync(int recordId)
    {
        var record = await GetRecordById(recordId);

        if (record == null)
        {
            return NotFound($"Record with ID {recordId} not found.");
        }

        db.Records.Remove(record);
        await db.SaveChangesAsync();

        return Ok($"Record with ID {recordId} has been successfully deleted.");
    }

    [HttpPost("/record")]
    public async Task<IActionResult> AddRecordAsync(int userId, int categoryId, decimal total, int? currencyId = null)
    {
        var user = await db.Users
            .Include(u => u.Currency)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return BadRequest("Invalid user ID.");
        }

        var category = await db.Categories.FindAsync(categoryId);
        if (category == null)
        {
            return BadRequest("Invalid category ID.");
        }

        Currency? currency;
        if (currencyId.HasValue)
        {
            currency = await db.Currencies.FindAsync(currencyId.Value);
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

        await db.Records.AddAsync(record);
        await db.SaveChangesAsync();

        return Ok("Record added successfully.");
    }

    [HttpGet("/record")]
    public async Task<IActionResult> GetRecordAsync(int? userId, int? categoryId)
    {
        if (!userId.HasValue && !categoryId.HasValue)
        {
            return BadRequest("You must provide at least one filter parameter.");
        }

        var query = db.Records
            .Include(r => r.User)
            .Include(r => r.Category)
            .Include(r => r.Currency)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(r => r.UserId == userId);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(r => r.CategoryId == categoryId);
        }

        var results = await query.ToListAsync();

        if (!results.Any())
        {
            return NotFound("No records found with the specified criteria.");
        }

        return Ok(results);
    }
}
