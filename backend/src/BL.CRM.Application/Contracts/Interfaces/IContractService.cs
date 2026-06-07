using BL.CRM.Application.Contracts.DTOs;

namespace BL.CRM.Application.Contracts.Interfaces;

public interface IContractService
{
    Task<IEnumerable<ContractsDto>> GetAllContractsAsync();
    Task<ContractDto?> GetContractByIdAsync(Guid id);
    Task<ContractDto> CreateContractAsync(CreateContractDto request);
    Task<ContractDto?> UpdateContractAsync(Guid id, UpdateContractDto request);
    Task<bool> DeleteContractAsync(Guid id);
    Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, Guid? excludeContractId = null);
    Task<AdvisorContractsDto> GetContractsByAdvisorIdAsync(Guid advisorId);
    Task<IEnumerable<ContractsDto>> GetContractsByClientIdAsync(Guid clientId);
}
