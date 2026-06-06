using System.ComponentModel.DataAnnotations;

namespace BL.CRM.Application.Users.DTOs;

public class RegisterAdvisorDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [RegularExpression(@"^(?:\d{5,6}\/\d{4}|\d{9,10})$", ErrorMessage = "Personal ID must be a 9 or 10 digit number, or in the format XXXXXX/XXXX or XXXXX/XXXX.")]
    public string? PersonalId { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "Phone number must be in a valid format (e.g. +420123456789 or 123456789).")]
    public string PhoneNumber { get; set; } = string.Empty;
}
