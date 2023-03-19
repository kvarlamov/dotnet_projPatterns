using Microsoft.AspNetCore.Authorization;

namespace CookieAuth.Model;

public class MinAgeRequirement : IAuthorizationRequirement
{
    public int Age { get; set; }

    public MinAgeRequirement(int age) => Age = age;
}