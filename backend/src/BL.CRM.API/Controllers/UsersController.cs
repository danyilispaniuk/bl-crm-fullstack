using BL.CRM.Application.Users.DTOs;
using BL.CRM.Application.Users.Interfaces;
using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BL.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService, UserManager<Person> userManager) : ControllerBase
{
    [HttpGet("~/api/admin/[controller]/clients")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetClients()
    {
        var clients = await userService.GetAllClientsAsync();
        return Ok(clients);
    }

    [HttpGet("~/api/[controller]/clients/{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<UserDto>> GetClientById(Guid id)
    {
        var client = await userService.GetClientByIdAsync(id);
        if (client == null) return NotFound();
        return Ok(client);
    }

    [HttpPost("~/api/[controller]/clients")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientDto request)
    {
        // Check if PersonalId exists
        if (!string.IsNullOrWhiteSpace(request.PersonalId))
        {
            var isPersonalIdTaken = userManager.Users.Any(u => u.PersonalId == request.PersonalId);
            if (isPersonalIdTaken)
            {
                return BadRequest(new { Message = "This Personal ID is already registered to another user." });
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var isEmailTaken = userManager.Users.Any(u => u.Email == request.Email);
            if (isEmailTaken)
            {
                return BadRequest(new { Message = "This Email is already registered to another user." });
            }
        }

        var client = new Client
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PersonalId = request.PersonalId,
            BirthDate = request.BirthDate
        };

        var result = await userManager.CreateAsync(client);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, new UserDto 
        {
            Id = client.Id,
            Email = client.Email,
            FirstName = client.FirstName,
            LastName = client.LastName,
            PersonalId = client.PersonalId,
            BirthDate = client.BirthDate,
            Role = "Client"
        });
    }

    [HttpGet("~/api/admin/[controller]/advisors")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAdvisors()
    {
        var advisors = await userService.GetAllAdvisorsAsync();
        return Ok(advisors);
    }

    [HttpGet("~/api/[controller]/clients/lookup")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<IEnumerable<UserLookupDto>>> GetClientsLookup()
    {
        var lookups = await userService.GetClientsLookupAsync();
        return Ok(lookups);
    }

    [HttpGet("~/api/[controller]/advisors/lookup")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<IEnumerable<UserLookupDto>>> GetAdvisorsLookup()
    {
        var lookups = await userService.GetAdvisorsLookupAsync();
        return Ok(lookups);
    }

    [HttpGet("~/api/[controller]/advisors/{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<UserDto>> GetAdvisorById(Guid id)
    {
        var advisor = await userService.GetAdvisorByIdAsync(id);
        if (advisor == null) return NotFound();
        return Ok(advisor);
    }

    [HttpPut("~/api/[controller]/clients/{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<IActionResult> UpdateClient(Guid id, [FromBody] UpdateUserDto request)
    {
        var client = await userManager.FindByIdAsync(id.ToString());
        if (client == null || client is not Client)
        {
            return NotFound(new { Message = "Client not found." });
        }

        return await UpdateUserAsync(client, request);
    }

    [HttpPut("~/api/admin/[controller]/advisors/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAdvisor(Guid id, [FromBody] UpdateUserDto request)
    {
        var advisor = await userManager.FindByIdAsync(id.ToString());
        if (advisor == null || advisor is not Advisor)
        {
            return NotFound(new { Message = "Advisor not found." });
        }

        return await UpdateUserAsync(advisor, request);
    }

    private async Task<IActionResult> UpdateUserAsync(Person user, UpdateUserDto request)
    {
        // Check if PersonalId exists
        if (!string.IsNullOrWhiteSpace(request.PersonalId))
        {
            var isPersonalIdTaken = userManager.Users.Any(u => u.PersonalId == request.PersonalId && u.Id != user.Id);
            if (isPersonalIdTaken)
            {
                return BadRequest(new { Message = "This Personal ID is already registered to another user." });
            }
        }

        // Email update
        if (user.Email != request.Email)
        {
            var existingEmail = await userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null && existingEmail.Id != user.Id)
            {
                return BadRequest(new { Message = "Email is already in use by another user." });
            }
            user.Email = request.Email;
            user.UserName = request.Email;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PersonalId = request.PersonalId;
        user.BirthDate = request.BirthDate;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        return NoContent();
    }

    [HttpDelete("~/api/admin/[controller]/clients/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        var client = await userManager.FindByIdAsync(id.ToString());
        if (client == null || client is not Client)
        {
            return NotFound(new { Message = "Client not found." });
        }

        var result = await userManager.DeleteAsync(client);
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Failed to delete client." });
        }

        return NoContent();
    }

    [HttpDelete("~/api/admin/[controller]/advisors/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAdvisor(Guid id)
    {
        var advisor = await userManager.FindByIdAsync(id.ToString());
        if (advisor == null || advisor is not Advisor)
        {
            return NotFound(new { Message = "Advisor not found." });
        }

        var result = await userManager.DeleteAsync(advisor);
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Failed to delete advisor." });
        }

        return NoContent();
    }
}
