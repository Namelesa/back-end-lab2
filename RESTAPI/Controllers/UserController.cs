using Microsoft.AspNetCore.Mvc;
using RESTAPI.Models;

namespace RESTAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    
    private static List<User> users = new List<User>
    {
        new User { Id = 1, Name = "Test1"},
        new User { Id = 2, Name = "Test2"}
    };

    
    [HttpGet("/user/{id}")]
    public ActionResult<User> GetUserById(int id)
    {
        var user = users.FirstOrDefault(p => p.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    
    [HttpDelete("/user/{id}")]
    public IActionResult DeleteUserById(int id)
    {
        var user = users.FirstOrDefault(p => p.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        users.Remove(user);
        return NoContent();
    }
    
    [HttpPost("/user")]
    public IActionResult AddUser(string userName)
    {
        User user = new User()
        {
            Name = userName,
        };
        user.Id = users.Max(p => p.Id) + 1;
        users.Add(user);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }
    
    [HttpGet("/users")]
    public ActionResult<IEnumerable<User>> GetAllUsers()
    {
        return Ok(users);
    }

    
}