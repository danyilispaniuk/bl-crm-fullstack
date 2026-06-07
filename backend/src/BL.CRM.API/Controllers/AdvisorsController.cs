using BL.CRM.Application.Users.DTOs;
using BL.CRM.Application.Users.Interfaces;
using BL.CRM.Application.Contracts.Interfaces;
using BL.CRM.Application.Contracts.DTOs;
using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BL.CRM.API.Controllers;

[ApiController]
[Route("api/advisor")]
public class AdvisorsController(
    IUserService userService,
    IUserExportService exportService,
    UserManager<Person> userManager,
    IContractService contractService) : ControllerBase
{
    [HttpGet("~/api/admin/advisor")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAdvisors()
    {
        var advisors = await userService.GetAllAdvisorsAsync();
        return Ok(advisors);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<UserDto>> GetAdvisorById(Guid id)
    {
        var advisor = await userService.GetAdvisorByIdAsync(id);
        if (advisor == null) return NotFound();
        return Ok(advisor);
    }


    [HttpGet("{id}/contracts")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<AdvisorContractsDto>> GetAdvisorContracts(Guid id)
    {
        var advisor = await userService.GetAdvisorByIdAsync(id);
        if (advisor == null) return NotFound(new { Message = "Advisor not found." });

        var contracts = await contractService.GetContractsByAdvisorIdAsync(id);
        return Ok(contracts);
    }

    [HttpGet("lookup")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<IEnumerable<UserLookupDto>>> GetAdvisorsLookup()
    {
        var lookups = await userService.GetAdvisorsLookupAsync();
        return Ok(lookups);
    }

    [HttpGet("~/api/admin/advisor/export/csv")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportAdvisorsToCsv()
    {
        var csvBytes = await exportService.ExportAdvisorsToCsvAsync();
        var fileName = $"advisors_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        return File(csvBytes, "text/csv; charset=utf-8", fileName);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<IActionResult> UpdateAdvisor(Guid id, [FromBody] UpdateUserDto request)
    {
        if (User.IsInRole("Advisor"))
        {
            var loggedInId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (loggedInId != id.ToString())
            {
                return StatusCode(403, new { Message = "You are not authorized to edit this advisor's profile." });
            }
        }

        var advisor = await userManager.FindByIdAsync(id.ToString());
        if (advisor == null || advisor is not Advisor)
        {
            return NotFound(new { Message = "Advisor not found." });
        }

        return await UpdateUserAsync(advisor, request);
    }

    [HttpDelete("~/api/admin/advisor/{id}")]
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
