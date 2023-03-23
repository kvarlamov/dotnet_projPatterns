using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi1.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    
    [AllowAnonymous]
    [Route("open-method")]
    public string Open()
    {
        return "hello from anonymus method";
    }

    [Route("auth-method")]
    public IActionResult AuthRequiredMethod()
    {
        return new JsonResult(User.Claims.Select(x => new {x.Type, x.Value}));
    }
}