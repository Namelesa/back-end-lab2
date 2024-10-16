using Microsoft.AspNetCore.Mvc;
using RESTAPI.Models;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    
    private static List<Category> categories = new List<Category>
    {
        new Category { Id = 1, CategoryName = "CategoryTest1"},
        new Category { Id = 2, CategoryName = "CategoryTest2"}
    };

    
    [HttpGet("/category")]
    public ActionResult<Category> GetCategory(string categoryName)
    {
        var category = categories.FirstOrDefault(p => p.CategoryName == categoryName);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }
    
    [HttpPost("/category")]
    public IActionResult AddCategory(string categoryName)
    {
        var currentCategory = categories.FirstOrDefault(p => p.CategoryName == categoryName);
        if (currentCategory == null)
        {
            Category category = new Category()
            {
                CategoryName = categoryName,
            };
            category.Id = categories.Max(p => p.Id) + 1;
            categories.Add(category);
            return Ok(category);
        }
        
        return BadRequest("You already have this category");
    }
    
    [HttpDelete("/category")]
    public IActionResult DeleteCategory(string categoryName)
    {
        var category = categories.FirstOrDefault(p => p.CategoryName == categoryName);
        if (category == null)
        {
            return NotFound();
        }
        categories.Remove(category);
        return Ok("You delete this category");
    }
    
    [HttpGet("/categories")]
    public ActionResult<Category> GetAllCategory()
    {
        var category = categories.ToList();
        return Ok(category);
    }
}