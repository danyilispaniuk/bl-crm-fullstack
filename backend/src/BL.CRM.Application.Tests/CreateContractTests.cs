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

public class CreateContractTests : TestBase
{
    [Fact]
    public async Task CreateContractAsync_ShouldCreateContractAndReturnDto_WhenDataIsValid()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        
        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PersonalId = "880808/1234",
            BirthDate = new DateOnly(1988, 8, 8)
        };

        var manager = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice.smith@example.com",
            PersonalId = "900101/5678",
            BirthDate = new DateOnly(1990, 1, 1)
        };

        var participant = new Advisor
        {
            Id = Guid.NewGuid(),
            FirstName = "Bob",
            LastName = "Jones",
            Email = "bob.jones@example.com",
            PersonalId = "920202/9012",
            BirthDate = new DateOnly(1992, 2, 2)
        };

        await dbContext.Clients.AddAsync(client);
        await dbContext.Advisors.AddRangeAsync(manager, participant);
        await dbContext.SaveChangesAsync();

        var service = new ContractService(dbContext);

        var request = new CreateContractDto
        {
            RegistrationNumber = "CON-2026-TEST",
            Institution = "Test Bank",
            StartDate = new DateOnly(2026, 6, 7),
            ValidityDate = new DateOnly(2027, 6, 7),
            EndDate = null,
            ClientId = client.Id,
            ContractManagerId = manager.Id,
            ParticipantIds = new List<Guid> { participant.Id }
        };

        // Act
        var result = await service.CreateContractAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.RegistrationNumber, result.RegistrationNumber);
        Assert.Equal(request.Institution, result.Institution);
        Assert.Equal(request.StartDate, result.StartDate);
        Assert.Equal(request.ValidityDate, result.ValidityDate);
        Assert.Null(result.EndDate);
        Assert.Equal(client.Id, result.ClientId);
        Assert.Equal("John Doe", result.ClientName);
        Assert.Equal(manager.Id, result.ContractManagerId);
        Assert.Equal("Alice Smith", result.ContractManagerName);
        
        // Assert database persistence
        var contractInDb = await dbContext.Contracts
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == result.Id);

        Assert.NotNull(contractInDb);
        Assert.Equal(request.RegistrationNumber, contractInDb.RegistrationNumber);
        Assert.Equal(request.Institution, contractInDb.Institution);
        Assert.Equal(client.Id, contractInDb.ClientId);
        Assert.Equal(manager.Id, contractInDb.ContractManagerId);
        Assert.Single(contractInDb.Participants);
        Assert.Equal(participant.Id, contractInDb.Participants.First().Id);
    }
}
