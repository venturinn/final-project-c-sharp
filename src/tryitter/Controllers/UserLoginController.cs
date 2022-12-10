using Microsoft.AspNetCore.Mvc;
using tryitter.Repository;
using tryitter.Models;
using tryitter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace tryitter.Controllers;

[Route("[controller]")]
[Authorize(Policy = "userLogin")]
public class UserLoginController : ControllerBase
{
    private readonly TryitterRepository _repository;
    public UserLoginController(TryitterRepository repository)
    {
        _repository = repository;
    }

    // Lista todos os posts do usuário logado
    [HttpGet("allmyposts")]
    public async Task<IActionResult> GetPostsByUserLogin()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = Int32.Parse(new TokenDecode().GetUserIdFromToken(token));

        var result = _repository.GetPostsByUserId(userId);

        if (!result.Any()) return NotFound(new { message = "Você ainda não possui posts!" });
        return Ok(result);
    }

    // Lista o último post do usuário logado
    [HttpGet("mylastpost")]
    public async Task<IActionResult> GetLastPostByUserLogin()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = Int32.Parse(new TokenDecode().GetUserIdFromToken(token));

        var result = _repository.GetPostByUserId(userId);

        if (result == null) return NotFound(new { message = "Você ainda não possui posts!" });

        return Ok(result);
    }

    // Publica um post na conta do usuário logado
    [HttpPost]
    public async Task<IActionResult> AddPost([FromBody] string content)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = Int32.Parse(new TokenDecode().GetUserIdFromToken(token));

        var post = new PostDTO { Content = content, UserId = userId };
        var result = _repository.AddPost(post);

        return Created("", result);
    }

    // Altera os dados cadastrais da conta do usuário logado
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var userId = Int32.Parse(new TokenDecode().GetUserIdFromToken(token));
        return Ok(_repository.UpdateUser(user, userId));
    }

    // Pesquisa os posts de outras contas 
    [HttpGet("{userName}/{allOrLast}")]
    public IActionResult GetPostOrPostsByUserName(string userName, string allOrLast)
    {
        var result = _repository.GetPostOrPostsByUserName(userName, allOrLast);

        if (result == null) return NotFound(new { message = $"O usuário {userName} não existe." });

        return Ok(result);
    }
}


