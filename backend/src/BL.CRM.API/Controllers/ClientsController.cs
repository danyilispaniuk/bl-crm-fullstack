using BL.CRM.Application.Users.DTOs;
using BL.CRM.Application.Users.Interfaces;
using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BL.CRM.API.Controllers;

[ApiController]
[Route("api/client")]
public class ClientsController(IUserService userService, UserManager<Person> userManager) : ControllerBase
{
    [HttpGet("~/api/admin/client")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetClients()
    {
        var clients = await userService.GetAllClientsAsync();
        return Ok(clients);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<UserDto>> GetClientById(Guid id)
    {
        var client = await userService.GetClientByIdAsync(id);
        if (client == null) return NotFound();
        return Ok(client);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientDto request)
    {
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
            PhoneNumber = request.PhoneNumber,
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
            PersonalId = client.PersonalId ?? string.Empty,
            BirthDate = client.BirthDate,
            PhoneNumber = client.PhoneNumber ?? string.Empty,
            Role = "Client"
        });
    }

    [HttpGet("lookup")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<IEnumerable<UserLookupDto>>> GetClientsLookup()
    {
        var lookups = await userService.GetClientsLookupAsync();
        return Ok(lookups);
    }

    [HttpPut("{id}")]
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

    [HttpDelete("~/api/admin/client/{id}")]
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

    private async Task<IActionResult> UpdateUserAsync(Person user, UpdateUserDto request)
    {
        if (!string.IsNullOrWhiteSpace(request.PersonalId))
        {
            var isPersonalIdTaken = userManager.Users.Any(u => u.PersonalId == request.PersonalId && u.Id != user.Id);
            if (isPersonalIdTaken)
            {
                return BadRequest(new { Message = "This Personal ID is already registered to another user." });
            }
        }

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
}
