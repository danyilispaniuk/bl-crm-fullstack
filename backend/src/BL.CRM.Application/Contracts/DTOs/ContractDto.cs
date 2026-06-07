using BL.CRM.Application.Users.DTOs;

namespace BL.CRM.Application.Contracts.DTOs;

public class ContractDto
{
    public Guid Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly ValidityDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;

    public Guid ContractManagerId { get; set; }
    public string ContractManagerName { get; set; } = string.Empty;

    public IEnumerable<UserDto> Participants { get; set; } = [];
}
