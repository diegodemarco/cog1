using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/greetings")]
public sealed class GreetingsController : ControllerBase
{
    [HttpGet("{name}")]
    public IActionResult GetGreeting(string name)
    {
        return Ok(new { message = $"Hello, {name}" });
    }
}


