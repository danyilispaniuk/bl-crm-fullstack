using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the application with Identity support.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<Person, IdentityRole<Guid>, Guid>(options), IApplicationDbContext
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Advisor> Advisors => Set<Advisor>();
    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all IEntityTypeConfiguration<T> classes in this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically stamp UpdatedAt on modified entities
        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is BL.CRM.Domain.Common.Entity entity)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Person person)
            {
                person.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
