using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BL.CRM.Infrastructure.Persistence;

public class DatabaseSeeder(UserManager<Person> userManager, RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext dbContext)
{
    public async Task SeedAsync()
    {
        // Add roles if they don't exist
        string[] roles = ["Admin", "Advisor", "Client"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        // Add a default Advisor if none exist
        if (!userManager.Users.OfType<Advisor>().Any())
        {
            var advisor = new Advisor
            {
                UserName = "advisor@test.com",
                Email = "advisor@test.com",
                FirstName = "Jan",
                LastName = "Novák",
                Age = 35,
                PersonalId = "850101/1234"
            };

            var result = await userManager.CreateAsync(advisor, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(advisor, "Advisor");
            }
        }

        // Add a default Client if none exist
        if (!userManager.Users.OfType<Client>().Any())
        {
            var client = new Client
            {
                UserName = "client@test.com",
                Email = "client@test.com",
                FirstName = "Petr",
                LastName = "Svoboda",
                Age = 40,
                PersonalId = "820202/5678"
            };

            var result = await userManager.CreateAsync(client, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(client, "Client");
            }
        }

        // Seed a default contract if none exist
        if (!dbContext.Contracts.Any())
        {
            var advisor = userManager.Users.OfType<Advisor>().FirstOrDefault(a => a.Email == "advisor@test.com");
            var client = userManager.Users.OfType<Client>().FirstOrDefault(c => c.Email == "client@test.com");

            if (advisor != null && client != null)
            {
                var contract = new Contract
                {
                    RegistrationNumber = "CNTR-2026-001",
                    Institution = "ČSOB",
                    StartDate = DateTime.UtcNow,
                    ValidityDate = DateTime.UtcNow.AddYears(1),
                    ClientId = client.Id,
                    ContractManagerId = advisor.Id
                };
                
                contract.Participants.Add(advisor);

                dbContext.Contracts.Add(contract);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
