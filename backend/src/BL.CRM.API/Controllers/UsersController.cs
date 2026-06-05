using BL.CRM.Application.Users.DTOs;
using BL.CRM.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BL.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("~/api/admin/[controller]/clients")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetClients()
    {
        var clients = await userService.GetAllClientsAsync();
        return Ok(clients);
    }

    [HttpGet("~/api/admin/[controller]/advisors")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAdvisors()
    {
        var advisors = await userService.GetAllAdvisorsAsync();
        return Ok(advisors);
    }
}
