using IDP.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IDP.Data;

public class OpenIddictAppDbContext : IdentityDbContext<ApplicationUser>
{
    public OpenIddictAppDbContext(DbContextOptions<OpenIddictAppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
     
        builder.UseOpenIddict();

        base.OnModelCreating(builder);
    }
}
