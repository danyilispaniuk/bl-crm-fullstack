namespace BL.CRM.Domain.Entities;

/// <summary>
/// Represents a Client in the system. 
/// Inherits from Person.
/// </summary>
public class Client : Person
{
    // Navigation property
    public ICollection<Contract> Contracts { get; set; } = [];
}
