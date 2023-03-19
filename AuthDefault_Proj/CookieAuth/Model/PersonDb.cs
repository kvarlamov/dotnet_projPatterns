using Microsoft.EntityFrameworkCore;

namespace CookieAuth.Model;

public class PersonDb : DbContext
{
    public PersonDb(DbContextOptions<PersonDb> options) : base(options)
    {
    }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Role)
            .WithOne(r => r.Person)
            .HasForeignKey<Role>(r => r.UserId);
        
        base.OnModelCreating(modelBuilder);
    }
}