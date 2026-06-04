namespace BL.CRM.Domain.Entities;

/// <summary>
/// Represents an Advisor in the system.
/// Inherits from Person.
/// </summary>
public class Advisor : Person
{
    // Navigation properties
    public ICollection<Contract> ManagedContracts { get; set; } = [];
    public ICollection<Contract> ParticipatedContracts { get; set; } = [];
}
