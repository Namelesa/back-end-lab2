using Microsoft.AspNetCore.Mvc;
using RESTAPI.Data;
using RESTAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoryController(AppDbContext db)
    {
        _db = db;
    }
    
    [HttpGet("/category")]
    public async Task<ActionResult<Category>> GetCategoryAsync(string categoryName)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(p => p.CategoryName == categoryName);
        if (category == null)
        {
            return NotFound($"Category '{categoryName}' not found.");
        }
        return Ok(category);
    }
    
    [HttpPost("/category")]
    public async Task<IActionResult> AddCategoryAsync(string categoryName)
    {
        var existingCategory = await _db.Categories.AnyAsync(p => p.CategoryName == categoryName);
        if (existingCategory)
        {
            return BadRequest($"Category '{categoryName}' already exists.");
        }

        var category = new Category { CategoryName = categoryName };
        await _db.Categories.AddAsync(category);
        await _db.SaveChangesAsync();

        return Ok("Add a new Category");
    }
    
    [HttpDelete("/category")]
    public async Task<IActionResult> DeleteCategoryAsync(string categoryName)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(p => p.CategoryName == categoryName);
        if (category == null)
        {
            return NotFound($"Category '{categoryName}' not found.");
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return Ok($"Category '{categoryName}' has been deleted.");
    }
    
    
    [HttpGet("/categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetAllCategoriesAsync()
    {
        var categories = await _db.Categories.ToListAsync();
        return Ok(categories);
    }
}
