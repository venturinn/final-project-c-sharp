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



[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly TryitterRepository _repository;
    public PostsController(TryitterRepository repository)
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

        if (!result.Any()) return NotFound(new { message = $"O usuário {userId} não possuí posts!" });
        return Ok(result);
    }

    // Requisito: Lista o último post de uma conta x

    [HttpGet("user/{userId}/last")]
    public IActionResult GetPostByUserId(int userId)
    {
        var result = _repository.GetPostByUserId(userId);

        if (result == null) return NotFound(new { message = $"O usuário {userId} não possuí posts!" });
        return Ok(result);
    }


}