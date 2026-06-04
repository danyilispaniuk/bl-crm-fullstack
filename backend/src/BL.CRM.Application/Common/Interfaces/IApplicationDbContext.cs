using BL.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Common.Interfaces;

/// <summary>
/// Abstraction for database access exposed to the Application layer.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Client> Clients { get; }
    DbSet<Advisor> Advisors { get; }
    DbSet<Contract> Contracts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
