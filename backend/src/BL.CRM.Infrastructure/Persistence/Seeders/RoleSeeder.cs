using Microsoft.AspNetCore.Identity;

namespace BL.CRM.Infrastructure.Persistence.Seeders;

public class RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager)
{
    public async Task SeedAsync()
    {
        string[] roles = ["Admin", "Advisor", "Client"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
