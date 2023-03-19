using System.Security.Claims;
using CookieAuth.Authorization;
using CookieAuth.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var persons = new List<Person>()
{
    new Person()
    {
        Id = 1,
        Email = "mail1@m.com",
        Password = "123",
        Age = 15,
        City = "SPB",
        Company = "Com",
        Role = new Role(1,"Admin")
    },
    new Person()
    {
        Id = 2,
        Email = "mail2@m.com",
        Password = "123",
        Age = 20,
        City = "SPB",
        Company = "Com",
        Role = new Role(1,"Admin")
    }
};


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/accessdenied";
    });

builder.Services.AddAuthorization(opts =>
{
    // opts.DefaultPolicy =
    //     new AuthorizationPolicyBuilder(AuthenticationSchemes.Ldap).RequireAuthenticatedUser()
    //         .Build();
    
    // Create policy checking that user authenticated
    opts.AddPolicy(Policies.AuthorizedOnly, policy => policy.RequireAuthenticatedUser());
    
    // Create policy for specific city
    opts.AddPolicy(Policies.OnlySpb, policy =>
    {
        policy.RequireClaim(ClaimTypes.Locality, "SPB", "СПБ");
    });
    
    opts.AddPolicy(Policies.Min18, policy =>
    {
        policy.Requirements.Add(new MinAgeRequirement(18));
    });
});

builder.Services.AddDbContext<PersonDb>(opt => opt.UseInMemoryDatabase("PersonDb"));
builder.Services.AddControllers();

builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// seed on start
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<PersonDb>();
context.AddRange(persons);

await context.SaveChangesAsync();

app.Run();