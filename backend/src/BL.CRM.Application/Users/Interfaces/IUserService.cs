using BL.CRM.Application.Users.DTOs;

namespace BL.CRM.Application.Users.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IEnumerable<UserDto>> GetAllClientsAsync();
    Task<IEnumerable<UserDto>> GetAllAdvisorsAsync();
    Task<UserDto?> GetClientByIdAsync(Guid id);
    Task<UserDto?> GetAdvisorByIdAsync(Guid id);
    
    Task<IEnumerable<UserLookupDto>> GetClientsLookupAsync();
    Task<IEnumerable<UserLookupDto>> GetAdvisorsLookupAsync();
}
