using BL.CRM.Domain.Common;

namespace BL.CRM.Domain.Entities;

/// <summary>
/// Represents a Contract.
/// </summary>
public class Contract : Entity
{
    public string RegistrationNumber { get; set; } = string.Empty; // Registration number
    public string Institution { get; set; } = string.Empty; // Institution (ČSOB, AEGON, etc.)
    
    public DateOnly StartDate { get; set; } // Start date
    public DateOnly ValidityDate { get; set; } // Validity date
    public DateOnly? EndDate { get; set; } // End date

    // Relationships

    // Client
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    // Contract manager (must be one of the advisors)
    public Guid ContractManagerId { get; set; }
    public Advisor ContractManager { get; set; } = null!;

    // Contract participants (advisors)
    public ICollection<Advisor> Participants { get; set; } = [];
}
