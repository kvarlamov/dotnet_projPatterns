using System.Security.Claims;
using CookieAuth.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookieAuth.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly PersonDb _db;

    public AuthController(PersonDb db)
    {
        _db = db;
    }

    [HttpGet("login")]
    public async Task Login()
    {
        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        // html-форма для ввода логина/пароля
        string loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>Test Auth Proj</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <form method='post'>
            <p>
                <label>Email</label><br />
                <input name='email' />
            </p>
            <p>
                <label>Password</label><br />
                <input type='password' name='password' />
            </p>
            <input type='submit' value='Login' />
        </form>
    </body>
    </html>";
        await HttpContext.Response.WriteAsync(loginForm);
    }
    
    [HttpGet("accessdenied")]
    public async Task AccessDenies()
    {
        HttpContext.Response.StatusCode = 403;
        await HttpContext.Response.WriteAsync("Access Denied");
    }

    [HttpPost("login")]
    public async Task<IResult> Login(string? returnUrl, [FromForm]LoginData data)
    {
        // получаем из формы email и пароль
        var form = HttpContext.Request.Form;
        // если email и/или пароль не установлены, посылаем статусный код ошибки 400
        if (!form.ContainsKey("email") || !form.ContainsKey("password"))
            return Results.BadRequest("Email и/или пароль не установлены");
        
        string email = form["email"];
        string password = form["password"];
        
        var person = await _db.Persons
            .Where(p => p.Email == data.Email && p.Password == data.Password)
            .Include(p => p.Role).FirstOrDefaultAsync();
        if (person is null) 
            return Results.Redirect("/accessdenied");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, person.Email),
            new Claim(ClaimTypes.Locality, person.City),
            new Claim("company", person.Company),
            new Claim("age", person.Age.ToString()),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role?.Name)
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimsPrincipal);
        return Results.Redirect("/weatherforecast");
    }
    
    
}