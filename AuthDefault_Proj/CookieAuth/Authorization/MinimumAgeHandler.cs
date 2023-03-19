using CookieAuth.Model;
using Microsoft.AspNetCore.Authorization;

namespace CookieAuth.Authorization;

public class MinimumAgeHandler : AuthorizationHandler<MinAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinAgeRequirement requirement)
    {
        var ageClaim = context.User.Claims.FirstOrDefault(c => c.Type == "age");

        if (!int.TryParse(ageClaim?.Value, out var age))
            throw new Exception("Age is not correct");
        
        if (age >= requirement.Age)
            context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}