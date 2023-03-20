using CookieAuth.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookieAuth.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    [HttpGet]
    public string GetOne()
    {
        return "you are admin";
    }

    [HttpGet("AdultAdmin")]
    [Authorize(Policy = Policies.Min18)]
    public string AdultAdmin()
    {
        return "adultadmin";
    }
}