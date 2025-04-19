using back_end.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace back_end.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    public DbSet<Block> Blocks { get; set; } = default!;
    public DbSet<Page> Pages { get; set; } = default!;

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
            cf.Ignore(e => e.LockoutEnd);
            cf.Ignore(e => e.UserName);
        });

        base.OnModelCreating(builder);
    }
}
