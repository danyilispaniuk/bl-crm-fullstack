using BL.CRM.Infrastructure.Persistence.Seeders;

namespace BL.CRM.Infrastructure.Persistence;

public class DatabaseSeeder(RoleSeeder roleSeeder, UserSeeder userSeeder, ContractSeeder contractSeeder)
{
    public async Task SeedAsync()
    {
        await roleSeeder.SeedAsync();
        await userSeeder.SeedAsync();
        await contractSeeder.SeedAsync();
    }
}
