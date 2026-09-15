#if (IndividualAuth)
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
#endif
using Microsoft.EntityFrameworkCore;
using Company.App.Data.Entities;

namespace Company.App.Data;

#if (IndividualAuth)
/// <summary>
/// Database context inheriting from IdentityDbContext for ASP.NET Core Identity authentication tables.
/// </summary>
public class AppDbContext : IdentityDbContext<IdentityUser>
#else
/// <summary>
/// Standard application database context.
/// </summary>
public class AppDbContext : DbContext
#endif
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Items table set.
    /// </summary>
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Item entity schema constraints
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.CreatedAtUtc);
        });
    }
}