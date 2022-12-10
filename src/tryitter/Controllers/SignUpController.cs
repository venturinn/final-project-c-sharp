using Microsoft.AspNetCore.Mvc;
using tryitter.Repository;
using tryitter.Models;
using tryitter.Services;
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
        return Created("", _repository.AddUser(user));
    }

}