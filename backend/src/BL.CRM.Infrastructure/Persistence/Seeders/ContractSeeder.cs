using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BL.CRM.Infrastructure.Persistence.Seeders;

public class ContractSeeder(ApplicationDbContext dbContext, UserManager<Person> userManager)
{
    public async Task SeedAsync()
    {
        var advisors = userManager.Users.OfType<Advisor>().Where(a => a.Email != "admin@test.com").ToList();
        var clients = userManager.Users.OfType<Client>().ToList();

        if (advisors.Any() && clients.Any())
        {
            string[] institutions = ["ČSOB", "AEGON", "Axa", "Generali", "Kooperativa"];
            
            for (int i = 1; i <= 5; i++)
            {
                var regNumber = $"CNTR-2026-{i:000}";
                if (!dbContext.Contracts.Any(c => c.RegistrationNumber == regNumber))
                {
                    var manager = advisors[i % advisors.Count];
                    var client = clients[i % clients.Count];
                    
                    var contract = new Contract
                    {
                        RegistrationNumber = regNumber,
                        Institution = institutions[i % institutions.Length],
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-i * 10)),
                        ValidityDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1).AddDays(i * 10)),
                        ClientId = client.Id,
                        ContractManagerId = manager.Id
                    };
                    
                    contract.Participants.Add(manager);
                    
                    // Add an extra participant just for variety on even contracts
                    if (i % 2 == 0 && advisors.Count > 1) 
                    {
                        var extraParticipant = advisors[(i + 1) % advisors.Count];
                        contract.Participants.Add(extraParticipant);
                    }

                    dbContext.Contracts.Add(contract);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
