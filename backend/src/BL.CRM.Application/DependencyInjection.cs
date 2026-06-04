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

        return services;
    }
}
