using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.CRM.Application.Contracts.DTOs;
using BL.CRM.Application.Contracts.Services;
using BL.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BL.CRM.Application.Tests;

public class UpdateContractTests : TestBase
{
    [Fact]
    public async Task UpdateContractAsync_ShouldUpdateContractAndReturnDto_WhenDataIsValid()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();

        // 1. Initial entities
        var client1 = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PersonalId = "880808/1234",
            BirthDate = new DateOnly(1988, 8, 8)
        };

        var manager1 = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice.smith@example.com",
            PersonalId = "900101/5678",
            BirthDate = new DateOnly(1990, 1, 1)
        };

        var participant1 = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Bob",
            LastName = "Jones",
            Email = "bob.jones@example.com",
            PersonalId = "920202/9012",
            BirthDate = new DateOnly(1992, 2, 2)
        };

        // 2. Updated entities to transition to
        var client2 = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Miller",
            Email = "jane.miller@example.com",
            PersonalId = "890909/4321",
            BirthDate = new DateOnly(1989, 9, 9)
        };

        var manager2 = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "David",
            LastName = "Brown",
            Email = "david.brown@example.com",
            PersonalId = "910101/8765",
            BirthDate = new DateOnly(1991, 1, 1)
        };

        var participant2 = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Charlie",
            LastName = "Green",
            Email = "charlie.green@example.com",
            PersonalId = "930303/3456",
            BirthDate = new DateOnly(1993, 3, 3)
        };

        // Save all of them to db
        await dbContext.Clients.AddRangeAsync(client1, client2);
        await dbContext.Advisors.AddRangeAsync(manager1, manager2, participant1, participant2);
        
        // Add the contract with initial details
        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            RegistrationNumber = "CON-OLD-123",
            Institution = "Old Bank",
            StartDate = new DateOnly(2025, 1, 1),
            ValidityDate = new DateOnly(2026, 1, 1),
            EndDate = null,
            ClientId = client1.Id,
            ContractManagerId = manager1.Id,
            Participants = new List<Advisor> { participant1 }
        };
        await dbContext.Contracts.AddAsync(contract);
        await dbContext.SaveChangesAsync();

        var service = new ContractService(dbContext);

        // Define the update request
        var updateRequest = new UpdateContractDto
        {
            RegistrationNumber = "CON-NEW-999",
            Institution = "New Bank",
            StartDate = new DateOnly(2026, 1, 1),
            ValidityDate = new DateOnly(2028, 1, 1),
            EndDate = new DateOnly(2027, 12, 31),
            ClientId = client2.Id,
            ContractManagerId = manager2.Id,
            ParticipantIds = new List<Guid> { participant2.Id }
        };

        // Act
        var result = await service.UpdateContractAsync(contract.Id, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contract.Id, result.Id);
        Assert.Equal(updateRequest.RegistrationNumber, result.RegistrationNumber);
        Assert.Equal(updateRequest.Institution, result.Institution);
        Assert.Equal(updateRequest.StartDate, result.StartDate);
        Assert.Equal(updateRequest.ValidityDate, result.ValidityDate);
        Assert.Equal(updateRequest.EndDate, result.EndDate);
        Assert.Equal(client2.Id, result.ClientId);
        Assert.Equal("Jane Miller", result.ClientName);
        Assert.Equal(manager2.Id, result.ContractManagerId);
        Assert.Equal("David Brown", result.ContractManagerName);
        
        // Assert database values
        var updatedContractInDb = await dbContext.Contracts
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == contract.Id);

        Assert.NotNull(updatedContractInDb);
        Assert.Equal(updateRequest.RegistrationNumber, updatedContractInDb.RegistrationNumber);
        Assert.Equal(updateRequest.Institution, updatedContractInDb.Institution);
        Assert.Equal(client2.Id, updatedContractInDb.ClientId);
        Assert.Equal(manager2.Id, updatedContractInDb.ContractManagerId);
        Assert.Single(updatedContractInDb.Participants);
        Assert.Equal(participant2.Id, updatedContractInDb.Participants.First().Id);
    }
}
