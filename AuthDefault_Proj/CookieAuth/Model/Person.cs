namespace CookieAuth.Model;

public class LoginData
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class Person
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
    public string? City { get; set; }
    public string? Company { get; set; }
    
    public Role? Role { get; set; }
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Role(int id, string name) => Name = name;

    public int UserId { get; set; }
    public Person? Person { get; set; } 
}