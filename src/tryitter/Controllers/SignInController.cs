using Microsoft.AspNetCore.Mvc;
using tryitter.Repository;
using tryitter.Models;
using tryitter.Services;
using Microsoft.AspNetCore.Authorization;

namespace tryitter.Controllers;

[Route("[controller]")]
[AllowAnonymous]
public class SignInController : ControllerBase
{
    private readonly TryitterRepository _repository;
    public SignInController(TryitterRepository repository)
    {
        _repository = repository;
    }

    // Retorna um JWT Token se as credenciais estiverem corretas
    [HttpPost]
    public IActionResult Authenticate([FromBody] UserLogin user)
    {
        var userDTO = _repository.GetUserByEmailAndPassword(user);

        if (userDTO == null)
            return Unauthorized(new { message = $"Email ou senha incorretas!" });

        var token = new TokenGenerator().Generate(userDTO);

        return Ok(token);
    }

}