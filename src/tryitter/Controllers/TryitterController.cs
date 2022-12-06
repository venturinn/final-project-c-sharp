using Microsoft.AspNetCore.Mvc;
using tryitter.Repository;
using tryitter.Models;
namespace tryitter.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly TryitterRepository _repository;
    public UsersController(TryitterRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult GetUsers()
    {
        return Ok(_repository.GetUsers());
    }

    [HttpGet("{userId}")]
    public IActionResult GetUserById(int userId)
    {
        return Ok(_repository.GetUserById(userId));
    }

    [HttpPost]
    public IActionResult AddUser([FromBody] User user)
    {
        return Created("", _repository.AddUser(user));
    }

    [HttpPut("{userId}")]
    public IActionResult UpdateUser([FromBody] User user, int userId)
    {
        return Ok(_repository.UpdateUser(user, userId));
    }

    [HttpDelete("{userId}")]
    public IActionResult Delete(int userId)
    {
        _repository.DeleteUserById(userId);
        return Ok(new { message = $"Usuário {userId} removido com sucesso" });
    }
}
