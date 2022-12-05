using Microsoft.AspNetCore.Mvc;

namespace tryitter.Controllers;

[ApiController]
[Route("[controller]")]
public class TryitterController : ControllerBase
{

    public TryitterController()
    {

    }

    [HttpGet("init")]
    public ActionResult<string> Get()
    {
        return "Começando o projeto final da aceleração! VQV!";
    }
}
