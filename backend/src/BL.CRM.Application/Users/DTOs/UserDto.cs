namespace BL.CRM.Application.Users.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalId { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
