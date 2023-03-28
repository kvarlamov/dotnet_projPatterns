using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiEx.Models;

namespace WebApiEx.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;

    public MainController(ILogger<MainController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("Greetings")]
    public string GreetingController()
    {
        _logger.LogInformation("Greeting method in controller");
        return "Hello from main controller";
    }

    [HttpGet("Error")]
    public IActionResult ErrorGet()
    {
        try
        {
            throw new Exception("Test error");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "exception");
            return Ok("Exception");
        }
    }

    [HttpPost]
    public IActionResult LogPostTest([FromBody] User user)
    {
        // структурное логгирование - автоматически парсит в Json
        _logger.LogInformation("Full json user {@user}", user);
        //если без @ - запишет toString
        _logger.LogInformation("Incoming user {user} {Now} {$name}", user, DateTime.UtcNow, user.Name);
        return Ok();
    }
}