using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Application.Contracts.DTOs;
using BL.CRM.Application.Contracts.Interfaces;
using BL.CRM.Application.Users.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Contracts.Services;

public class ContractService(IApplicationDbContext dbContext) : IContractService
{
    public async Task<IEnumerable<ContractsDto>> GetAllContractsAsync()
    {
        return await dbContext.Contracts
            .Include(c => c.Client)
            .Include(c => c.ContractManager)
            .Select(c => new ContractsDto
            {
                Id = c.Id,
                RegistrationNumber = c.RegistrationNumber,
                Institution = c.Institution,
                StartDate = c.StartDate,
                ValidityDate = c.ValidityDate,
                EndDate = c.EndDate,
                ClientId = c.ClientId,
                ClientName = $"{c.Client.FirstName} {c.Client.LastName}",
                ContractManagerId = c.ContractManagerId,
                ContractManagerName = $"{c.ContractManager.FirstName} {c.ContractManager.LastName}"
            }).ToListAsync();
    }

    public async Task<ContractDto?> GetContractByIdAsync(Guid id)
    {
        return await dbContext.Contracts
            .Include(c => c.Client)
            .Include(c => c.ContractManager)
            .Where(c => c.Id == id)
            .Select(c => new ContractDto
            {
                Id = c.Id,
                RegistrationNumber = c.RegistrationNumber,
                Institution = c.Institution,
                StartDate = c.StartDate,
                ValidityDate = c.ValidityDate,
                EndDate = c.EndDate,
                ClientId = c.ClientId,
                ClientName = $"{c.Client.FirstName} {c.Client.LastName}",
                ContractManagerId = c.ContractManagerId,
                ContractManagerName = $"{c.ContractManager.FirstName} {c.ContractManager.LastName}",
                Participants = c.Participants.Select(p => new UserDto
                {
                    Id = p.Id,
                    Email = p.Email ?? string.Empty,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PersonalId = p.PersonalId,
                    Age = p.Age,
                    Role = "Advisor"
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<ContractDto> CreateContractAsync(CreateContractDto request)
    {
        var participants = await dbContext.Advisors
            .Where(a => request.ParticipantIds.Contains(a.Id))
            .ToListAsync();

        var contract = new BL.CRM.Domain.Entities.Contract
        {
            RegistrationNumber = request.RegistrationNumber,
            Institution = request.Institution,
            StartDate = request.StartDate,
            ValidityDate = request.ValidityDate,
            EndDate = request.EndDate,
            ClientId = request.ClientId,
            ContractManagerId = request.ContractManagerId,
            Participants = participants
        };

        dbContext.Contracts.Add(contract);
        await dbContext.SaveChangesAsync();

        return await GetContractByIdAsync(contract.Id) 
            ?? throw new InvalidOperationException("Failed to retrieve created contract.");
    }

    public async Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber)
    {
        return !await dbContext.Contracts.AnyAsync(c => c.RegistrationNumber == registrationNumber);
    }
}
