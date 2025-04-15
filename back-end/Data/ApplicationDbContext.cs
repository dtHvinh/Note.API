using back_end.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace back_end.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(cf =>
        {
            cf.Ignore(e => e.EmailConfirmed);
            cf.Ignore(e => e.PhoneNumberConfirmed);
            cf.Ignore(e => e.TwoFactorEnabled);
            cf.Ignore(e => e.LockoutEnabled);
            cf.Ignore(e => e.AccessFailedCount);
            cf.Ignore(e => e.NormalizedEmail);
            cf.Ignore(e => e.ConcurrencyStamp);
        });

        base.OnModelCreating(builder);
    }
}
