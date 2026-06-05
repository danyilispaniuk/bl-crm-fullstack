using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Domain.Entities;
using BL.CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BL.CRM.Infrastructure;

/// <summary>
/// DI registration for the Infrastructure layer.
/// Call <c>services.AddInfrastructure(configuration)</c> in <c>Program.cs</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Expose the typed DbContext through the application interface
        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // Identity
        services.AddIdentityCore<Person>()
            .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Database Seeders
        services.AddScoped<BL.CRM.Infrastructure.Persistence.Seeders.RoleSeeder>();
        services.AddScoped<BL.CRM.Infrastructure.Persistence.Seeders.UserSeeder>();
        services.AddScoped<BL.CRM.Infrastructure.Persistence.Seeders.ContractSeeder>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
