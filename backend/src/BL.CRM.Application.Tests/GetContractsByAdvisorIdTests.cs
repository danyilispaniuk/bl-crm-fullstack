using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.CRM.Application.Contracts.Services;
using BL.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BL.CRM.Application.Tests;

public class GetContractsByAdvisorIdTests : TestBase
{
    [Fact]
    public async Task GetContractsByAdvisorIdAsync_ShouldReturnContracts_WhenAdvisorIsManagerOrParticipant()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();

        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        var advisor1 = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Advisor",
            LastName = "One",
            Email = "adv1@example.com"
        };

        var advisor2 = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Advisor",
            LastName = "Two",
            Email = "adv2@example.com"
        };

        var advisor3 = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Advisor",
            LastName = "Three",
            Email = "adv3@example.com"
        };

        await dbContext.Clients.AddAsync(client);
        await dbContext.Advisors.AddRangeAsync(advisor1, advisor2, advisor3);

        // Contract 1: Managed by advisor1, participant advisor2
        var contract1 = new Contract
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = "CON-1",
            Institution = "Bank A",
            StartDate = new DateOnly(2026, 1, 1),
            ValidityDate = new DateOnly(2027, 1, 1),
            ClientId = client.Id,
            ContractManagerId = advisor1.Id,
            Participants = new List<Advisor> { advisor2 }
        };

        // Contract 2: Managed by advisor3, participant advisor1
        var contract2 = new Contract
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = "CON-2",
            Institution = "Bank B",
            StartDate = new DateOnly(2026, 1, 1),
            ValidityDate = new DateOnly(2027, 1, 1),
            ClientId = client.Id,
            ContractManagerId = advisor3.Id,
            Participants = new List<Advisor> { advisor1 }
        };

        // Contract 3: Managed by advisor3, participant advisor2
        var contract3 = new Contract
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = "CON-3",
            Institution = "Bank C",
            StartDate = new DateOnly(2026, 1, 1),
            ValidityDate = new DateOnly(2027, 1, 1),
            ClientId = client.Id,
            ContractManagerId = advisor3.Id,
            Participants = new List<Advisor> { advisor2 }
        };

        await dbContext.Contracts.AddRangeAsync(contract1, contract2, contract3);
        await dbContext.SaveChangesAsync();

        var service = new ContractService(dbContext);

        // Act
        var resultsForAdvisor1 = await service.GetContractsByAdvisorIdAsync(advisor1.Id);

        // Assert
        Assert.NotNull(resultsForAdvisor1);
        Assert.Single(resultsForAdvisor1.ManagedContracts);
        Assert.Contains(resultsForAdvisor1.ManagedContracts, c => c.Id == contract1.Id);

        Assert.Single(resultsForAdvisor1.ParticipatingContracts);
        Assert.Contains(resultsForAdvisor1.ParticipatingContracts, c => c.Id == contract2.Id);

        Assert.DoesNotContain(resultsForAdvisor1.ManagedContracts, c => c.Id == contract3.Id);
        Assert.DoesNotContain(resultsForAdvisor1.ParticipatingContracts, c => c.Id == contract3.Id);
    }
}
