using System.ComponentModel.DataAnnotations;

namespace BL.CRM.Application.Users.DTOs;

public class CreateClientDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [RegularExpression(@"^\d{5,6}\/\d{4}$", ErrorMessage = "Personal ID must be in the format XXXXXX/XXXX or XXXXX/XXXX.")]
    public string? PersonalId { get; set; }

    [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
    public int Age { get; set; }
}
