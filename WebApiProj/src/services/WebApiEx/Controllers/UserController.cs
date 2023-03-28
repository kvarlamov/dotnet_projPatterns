using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using WebApiEx.Models;

namespace WebApiEx.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IDistributedCache _cache;

    public UserController(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    [HttpGet("GetByName/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var user = await _cache.GetStringAsync(name);
        if (string.IsNullOrEmpty(user))
            return NotFound($"user with name {name} not found");

        return Ok(user);
    }

    [HttpPost("CreateNew")]
    public async Task<IActionResult> Create(User user)
    {
        var userJson = JsonSerializer.Serialize(user);
        await _cache.SetAsync(user.Name, Encoding.UTF8.GetBytes(userJson));
        return Ok(userJson);
    }
}