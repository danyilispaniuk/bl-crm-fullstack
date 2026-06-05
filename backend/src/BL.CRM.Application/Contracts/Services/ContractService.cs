using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Application.Contracts.DTOs;
using BL.CRM.Application.Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Contracts.Services;

public class ContractService(IApplicationDbContext dbContext) : IContractService
{
    public async Task<IEnumerable<ContractDto>> GetAllContractsAsync()
    {
        return await dbContext.Contracts
            .Include(c => c.Client)
            .Include(c => c.ContractManager)
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
                ContractManagerName = $"{c.ContractManager.FirstName} {c.ContractManager.LastName}"
            }).ToListAsync();
    }
}
