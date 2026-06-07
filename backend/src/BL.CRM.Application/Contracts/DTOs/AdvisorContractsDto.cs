namespace BL.CRM.Application.Contracts.DTOs;

public class AdvisorContractsDto
{
    public IEnumerable<ContractsDto> ManagedContracts { get; set; } = [];
    public IEnumerable<ContractsDto> ParticipatingContracts { get; set; } = [];
}
