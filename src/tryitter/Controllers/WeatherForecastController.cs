using Microsoft.AspNetCore.Mvc;

namespace tryitter.Controllers;

[ApiController]
[Route("[controller]")]
public class tryitterController : ControllerBase
{

    public tryitterController()
    {

    }

    [HttpGet("init")]
    public ActionResult<string> Get()
    {
        return "Começando o projeto final da aceleração! VQV!";
    }
}
