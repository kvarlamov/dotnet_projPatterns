using Microsoft.AspNetCore.Mvc;

namespace WebApiEx.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : Controller
{
    [HttpGet(Name = "Get Hello")]
    public string GreetingController()
    {
        return "Hello from main controller";
    }
}