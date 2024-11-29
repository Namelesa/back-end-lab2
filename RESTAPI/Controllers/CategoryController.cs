using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTAPI.Data;
using RESTAPI.Models;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Validators;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class CategoryController(AppDbContext db) : ControllerBase
{
    private async Task<IActionResult?> ValidateCategoryNameAsync(string categoryName)
    {
        var validator = new StringParamValidator();
        var validationResult = await validator.ValidateAsync(categoryName);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        return null;
    }

    private async Task<Category?> FindCategoryByNameAsync(string categoryName)
    {
        return await db.Categories.FirstOrDefaultAsync(p => p.CategoryName == categoryName);
    }
    
    [HttpGet("/category")]
    public async Task<IActionResult> GetCategoryAsync(string categoryName)
    {
        IActionResult? validationResult = await ValidateCategoryNameAsync(categoryName);
        if (validationResult != null) return validationResult;

        var category = await FindCategoryByNameAsync(categoryName);
        if (category == null)
        {
            return NotFound($"Category '{categoryName}' not found.");
        }

        return Ok(category);
    }
    
    [HttpPost("/category")]
    public async Task<IActionResult> AddCategoryAsync(string categoryName)
    {
        var validationResult = await ValidateCategoryNameAsync(categoryName);
        if (validationResult != null) return validationResult;

        var existingCategory = await FindCategoryByNameAsync(categoryName);
        if (existingCategory != null)
        {
            return BadRequest($"Category '{categoryName}' already exists.");
        }

        var category = new Category { CategoryName = categoryName };
        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync();

        return Ok("Add a new Category");
    }
    
    [HttpDelete("/category")]
    public async Task<IActionResult> DeleteCategoryAsync(string categoryName)
    {
        var validationResult = await ValidateCategoryNameAsync(categoryName);
        if (validationResult != null) return validationResult;

        var category = await FindCategoryByNameAsync(categoryName);
        if (category == null)
        {
            return NotFound($"Category '{categoryName}' not found.");
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return Ok($"Category '{categoryName}' has been deleted.");
    }
    
    [HttpGet("/categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetAllCategoriesAsync()
    {
        var categories = await db.Categories.ToListAsync();
        return Ok(categories);
    }
}
