using BL.CRM.Application.Common.Interfaces;
using BL.CRM.Application.Users.DTOs;
using BL.CRM.Application.Users.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BL.CRM.Application.Users.Services;

public class UserService(IApplicationDbContext dbContext) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var clients = await dbContext.Clients
            .Select(c => new UserDto
            {
                Id = c.Id,
                Email = c.Email ?? string.Empty,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PersonalId = c.PersonalId,
                Age = c.Age,
                Role = "Client"
            }).ToListAsync();

        var advisors = await dbContext.Advisors
            .Select(a => new UserDto
            {
                Id = a.Id,
                Email = a.Email ?? string.Empty,
                FirstName = a.FirstName,
                LastName = a.LastName,
                PersonalId = a.PersonalId,
                Age = a.Age,
                Role = "Advisor"
            }).ToListAsync();

        return clients.Concat(advisors);
    }

    public async Task<IEnumerable<UserDto>> GetAllClientsAsync()
    {
        return await dbContext.Clients
            .Select(c => new UserDto
            {
                Id = c.Id,
                Email = c.Email ?? string.Empty,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PersonalId = c.PersonalId,
                Age = c.Age,
                Role = "Client"
            }).ToListAsync();
    }

    public async Task<IEnumerable<UserDto>> GetAllAdvisorsAsync()
    {
        return await dbContext.Advisors
            .Select(a => new UserDto
            {
                Id = a.Id,
                Email = a.Email ?? string.Empty,
                FirstName = a.FirstName,
                LastName = a.LastName,
                PersonalId = a.PersonalId,
                Age = a.Age,
                Role = "Advisor"
            }).ToListAsync();
    }

    public async Task<UserDto?> GetClientByIdAsync(Guid id)
    {
        return await dbContext.Clients
            .Where(c => c.Id == id)
            .Select(c => new UserDto
            {
                Id = c.Id,
                Email = c.Email ?? string.Empty,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PersonalId = c.PersonalId,
                Age = c.Age,
                Role = "Client"
            }).FirstOrDefaultAsync();
    }

    public async Task<UserDto?> GetAdvisorByIdAsync(Guid id)
    {
        return await dbContext.Advisors
            .Where(a => a.Id == id)
            .Select(a => new UserDto
            {
                Id = a.Id,
                Email = a.Email ?? string.Empty,
                FirstName = a.FirstName,
                LastName = a.LastName,
                PersonalId = a.PersonalId,
                Age = a.Age,
                Role = "Advisor"
            }).FirstOrDefaultAsync();
    }
}
