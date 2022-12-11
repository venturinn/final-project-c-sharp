using Microsoft.AspNetCore.Mvc;
using tryitter.Repository;
using tryitter.Models;
using Microsoft.AspNetCore.Authorization;

namespace tryitter.Controllers;

[Route("[controller]")]
[Authorize(Policy = "AdmLogin")]
public class AdminPostsController : ControllerBase
{
    private readonly TryitterRepository _repository;
    public AdminPostsController(TryitterRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult GetPosts()
    {
        return Ok(_repository.GetPosts());
    }

    [HttpGet("{postId}")]
    public IActionResult GetPostById(int postId)
    {
        return Ok(_repository.GetPostById(postId));
    }

    [HttpPost]
    public IActionResult AddPost([FromBody] PostDTO post)
    {
        if (post.Content.Length > 300) return BadRequest(new { message = "Post excedeu 300 caracteres." });

        var result = _repository.AddPost(post);

        if (result == null)
            return NotFound(new { message = $"Usuário {post.UserId} não existe!" });

        return Created("", result);
    }

    [HttpPut("{postId}")]
    public IActionResult UpdatePost([FromBody] Post post, int postId)
    {
        var result = _repository.UpdatePost(post, postId);

        if (result == null)
            return NotFound(new { message = $"Post {post.UserId} não existe!" });

        return Ok(result);
    }

    [HttpDelete("{postId}")]
    public IActionResult Delete(int postId)
    {
        var result = _repository.DeletePostById(postId);

        if (result == null)
            return NotFound(new { message = $"Post {postId} não existe!" });

        return Ok(new { message = $"Post {postId} removido com sucesso" });
    }


    // Requisito: Listar todos os posts de uma conta x

    [HttpGet("user/{userId}/all")]
    public IActionResult GetPostsByUserId(int userId)
    {
        var result = _repository.GetPostsByUserId(userId);

        if (result == null) return BadRequest(new { message = $"O usuário {userId} não existe!" });
        if (!result.Any()) return NotFound(new { message = $"O usuário {userId} não possuí posts!" });
        return Ok(result);
    }

    // Requisito: Lista o último post de uma conta x

    [HttpGet("user/{userId}/last")]
    public IActionResult GetPostByUserId(int userId)
    {
        var result = _repository.GetPostByUserId(userId);

        if (result == null) return NotFound(new { message = $"O usuário {userId} não existe ou não possuí posts" });

        return Ok(result);
    }
}