using System.ComponentModel.DataAnnotations;

namespace BL.CRM.Application.Contracts.DTOs;

public class CreateContractDto
{
    [Required(ErrorMessage = "Registration number is required.")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Registration number must be between 5 and 50 characters.")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Institution is required.")]
    public string Institution { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly ValidityDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid ContractManagerId { get; set; }

    public List<Guid> ParticipantIds { get; set; } = new();
}
