using Microsoft.AspNetCore.Mvc;
using tryitter.Repository;
using tryitter.Models;
using Microsoft.AspNetCore.Authorization;

namespace tryitter.Controllers;

[Route("[controller]")]
[AllowAnonymous]
public class SignUpController : ControllerBase
{
    private readonly TryitterRepository _repository;
    public SignUpController(TryitterRepository repository)
    {
        _repository = repository;
    }

    // Rota usada para o usuário criar um nova conta na plataforma
    [HttpPost]
    public IActionResult AddUser([FromBody] User user)
    {
        var userEmailAlreadyExists = _repository.GetUserByEmail(user.Email);
        if (userEmailAlreadyExists != null) return BadRequest(new { message = $"O email {user.Email} já existe." });

        var userNameAlreadyExists = _repository.GetUserByName(user.Name);
        if (userNameAlreadyExists != null) return BadRequest(new { message = $"O nome {user.Name} já existe, escolha outro nome." });

        return Created("", _repository.AddUser(user));
    }

}