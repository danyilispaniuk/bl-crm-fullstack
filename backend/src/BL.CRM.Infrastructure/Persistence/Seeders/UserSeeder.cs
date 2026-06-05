using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BL.CRM.Infrastructure.Persistence.Seeders;

public class UserSeeder(UserManager<Person> userManager)
{
    public async Task SeedAsync()
    {
        // Seed Admin (Using Advisor class for system access)
        if (!userManager.Users.Any(u => u.Email == "admin@test.com"))
        {
            var admin = new Advisor
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FirstName = "Admin",
                LastName = "System",
                BirthDate = new DateOnly(1980, 1, 1),
                PersonalId = "800101/0000"
            };
            var result = await userManager.CreateAsync(admin, "Password123!");
            if (result.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed 10 Advisors
        for (int i = 1; i <= 10; i++)
        {
            var email = $"advisor{i}@test.com";
            if (!userManager.Users.Any(u => u.Email == email))
            {
                var advisor = new Advisor
                {
                    UserName = email,
                    Email = email,
                    FirstName = $"AdvisorName{i}",
                    LastName = $"AdvisorSurname{i}",
                    BirthDate = new DateOnly(1990, 1, i),
                    PersonalId = $"9001{i:00}/1234"
                };
                var result = await userManager.CreateAsync(advisor, "Password123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(advisor, "Advisor");
            }
        }

        // Seed 10 Clients (No password)
        for (int i = 1; i <= 10; i++)
        {
            var email = $"client{i}@test.com";
            if (!userManager.Users.Any(u => u.Email == email))
            {
                var client = new Client
                {
                    UserName = email,
                    Email = email,
                    FirstName = $"ClientName{i}",
                    LastName = $"ClientSurname{i}",
                    BirthDate = new DateOnly(1995, 1, i),
                    PersonalId = $"9501{i:00}/5678"
                };
                var result = await userManager.CreateAsync(client);
                if (result.Succeeded) await userManager.AddToRoleAsync(client, "Client");
            }
        }
    }
}
