using BL.CRM.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace BL.CRM.Domain.Entities;

/// <summary>
/// Abstract base class for all people in the system.
/// </summary>
public abstract class Person : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PersonalId { get; set; } // Czech Rodné číslo
    public DateOnly BirthDate { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
