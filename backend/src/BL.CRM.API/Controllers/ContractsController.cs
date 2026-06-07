using BL.CRM.Application.Contracts.DTOs;
using BL.CRM.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BL.CRM.API.Controllers;

[ApiController]
[Route("api/contract")]
public class ContractsController(IContractService contractService) : ControllerBase
{
    [HttpGet("~/api/admin/contract")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ContractsDto>>> GetAll()
    {
        var contracts = await contractService.GetAllContractsAsync();
        return Ok(contracts);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<ActionResult<ContractDto>> GetById(Guid id)
    {
        var contract = await contractService.GetContractByIdAsync(id);
        if (contract == null) return NotFound();
        return Ok(contract);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractDto request)
    {
        try
        {
            var isUnique = await contractService.IsRegistrationNumberUniqueAsync(request.RegistrationNumber);
            if (!isUnique)
            {
                return BadRequest(new { Message = "A contract with this Registration Number already exists." });
            }

            var createdContract = await contractService.CreateContractAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdContract.Id }, createdContract);
        }
        catch (Exception ex)
        {
            // Foreign key violation if ClientId or ContractManagerId is invalid
            return BadRequest(new { Message = "Failed to create contract. Ensure Client and Manager exist.", Error = ex.Message });
        }
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Advisor")]
    public async Task<IActionResult> UpdateContract(Guid id, [FromBody] UpdateContractDto request)
    {
        try
        {
            if (User.IsInRole("Advisor"))
            {
                var advisorIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(advisorIdClaim, out var advisorId))
                {
                    return Forbid();
                }

                var existingContract = await contractService.GetContractByIdAsync(id);
                if (existingContract == null)
                {
                    return NotFound(new { Message = "Contract not found." });
                }

                var isManager = existingContract.ContractManagerId == advisorId;
                if (!isManager)
                {
                    return StatusCode(403, new { Message = "You are not authorized to edit this contract." });
                }
            }

            var isUnique = await contractService.IsRegistrationNumberUniqueAsync(request.RegistrationNumber, id);
            if (!isUnique)
            {
                return BadRequest(new { Message = "A contract with this Registration Number already exists." });
            }

            var updatedContract = await contractService.UpdateContractAsync(id, request);
            
            if (updatedContract == null)
            {
                return NotFound(new { Message = "Contract not found." });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            // Foreign key violation if ClientId or ContractManagerId is invalid
            return BadRequest(new { Message = "Failed to update contract. Ensure Client and Manager exist.", Error = ex.Message });
        }
    }

    [HttpDelete("~/api/admin/contract/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteContract(Guid id)
    {
        var deleted = await contractService.DeleteContractAsync(id);
        if (!deleted)
        {
            return NotFound(new { Message = "Contract not found." });
        }

        return NoContent();
    }
}
