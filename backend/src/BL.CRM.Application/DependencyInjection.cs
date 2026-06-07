using Microsoft.Extensions.DependencyInjection;

namespace BL.CRM.Application;

/// <summary>
/// DI registration for the Application layer.
/// Call <c>services.AddApplication()</c> in <c>Program.cs</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services here as they are created
        services.AddScoped<BL.CRM.Application.Users.Interfaces.IUserService, BL.CRM.Application.Users.Services.UserService>();
        services.AddScoped<BL.CRM.Application.Contracts.Interfaces.IContractService, BL.CRM.Application.Contracts.Services.ContractService>();
        services.AddScoped<BL.CRM.Application.Users.Interfaces.IUserExportService, BL.CRM.Application.Users.Services.UserExportService>();
        services.AddScoped<BL.CRM.Application.Contracts.Interfaces.IContractExportService, BL.CRM.Application.Contracts.Services.ContractExportService>();

        return services;
    }
}
