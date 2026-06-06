using BL.CRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BL.CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController(UserManager<Person> userManager) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request, [FromServices] IConfiguration configuration)
    {
        // Find the user by email
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Verify password
        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Client";

        // Generate JWT Token
        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? ""),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)
        };

        var jwtSettings = configuration.GetSection("Jwt");
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var expireDays = Convert.ToInt32(jwtSettings["ExpireDays"]);

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expireDays),
            signingCredentials: creds
        );

        var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

        // Return token and user data
        return Ok(new
        {
            Token = tokenString,
            User = new
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role
            }
        });
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignupAdvisor([FromBody] BL.CRM.Application.Users.DTOs.RegisterAdvisorDto request)
    {
        // Check if email is registered
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { Message = "Email is already registered." });
        }

        // Check if PersonalId is registered
        if (!string.IsNullOrWhiteSpace(request.PersonalId))
        {
            var isPersonalIdTaken = userManager.Users.Any(u => u.PersonalId == request.PersonalId);
            if (isPersonalIdTaken)
            {
                return BadRequest(new { Message = "This Personal ID is already registered to another user." });
            }
        }

        var advisor = new Advisor
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PersonalId = request.PersonalId,
            BirthDate = request.BirthDate
        };

        var result = await userManager.CreateAsync(advisor, request.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        await userManager.AddToRoleAsync(advisor, "Advisor");

        return Ok(new { Message = "Advisor registered successfully." });
    }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
