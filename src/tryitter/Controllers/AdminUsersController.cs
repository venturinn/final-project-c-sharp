using Microsoft.AspNetCore.Mvc;
using tryitter.Repository;
using tryitter.Models;
using Microsoft.AspNetCore.Authorization;

namespace tryitter.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AdmLogin")]
public class AdminUsersController : ControllerBase
{
    private readonly TryitterRepository _repository;
    public AdminUsersController(TryitterRepository repository)
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
