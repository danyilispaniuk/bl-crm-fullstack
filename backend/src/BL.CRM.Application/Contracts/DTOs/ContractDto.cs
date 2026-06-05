namespace BL.CRM.Application.Contracts.DTOs;

public class ContractDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ValidityDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;

    public Guid ContractManagerId { get; set; }
    public string ContractManagerName { get; set; } = string.Empty;
}
