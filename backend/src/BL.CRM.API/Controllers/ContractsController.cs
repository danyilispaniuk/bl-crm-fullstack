using BL.CRM.Application.Contracts.DTOs;
using BL.CRM.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BL.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController(IContractService contractService) : ControllerBase
{
    [HttpGet("~/api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll()
    {
        var contracts = await contractService.GetAllContractsAsync();
        return Ok(contracts);
    }
}
