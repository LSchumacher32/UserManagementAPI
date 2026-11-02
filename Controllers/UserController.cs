using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private static readonly List<User> _users = new();
    private static int _nextId = 1;

    [HttpGet]
    public ActionResult<List<User>> GetAll()
    {
        return Ok(_users);
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create(User newUser)
    {
        newUser.Id = _nextId++;
        _users.Add(newUser);

        return Created($"/api/users/{newUser.Id}", newUser);
    }

    [HttpPut("{id:int}")]
    public ActionResult<User> Update(int id, User updatedUser)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Email = updatedUser.Email;

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        _users.Remove(user);

        return NoContent();
    }

    [HttpGet("test-error")]
    public IActionResult TestError()
    {
        throw new InvalidOperationException(
            "Absichtlicher Fehler zum Testen der Middleware.");
    }
}