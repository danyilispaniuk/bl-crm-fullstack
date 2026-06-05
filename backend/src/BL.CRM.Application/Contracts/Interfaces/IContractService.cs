using BL.CRM.Application.Contracts.DTOs;

namespace BL.CRM.Application.Contracts.Interfaces;

public interface IContractService
{
    Task<IEnumerable<ContractsDto>> GetAllContractsAsync();
    Task<ContractDto?> GetContractByIdAsync(Guid id);
}
