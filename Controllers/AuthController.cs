using Azure.Core;
using HRApi.Data;
using HRApi.Models;
using HRApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly AuthRepository _authRepository;

    public AuthController(AppDbContext context, IConfiguration config, AuthRepository authRepository)
    {
        _context = context;
        _config = config;
        _authRepository = authRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(HRApi.Models.LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null || user.PasswordHash != request.Password)
            return Unauthorized("Invalid email or password");

        var roles = await _context.UserRoles
            .Where(r => r.UserId == user.Id)
            .Select(r => r.RoleName)
            .ToListAsync();

        var token = _authRepository.GenerateJwt(user.Email, user.Id, roles);

        var response = new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Name = user.EmployeeName,
            EmployeeId = user.EmployeeId,
            Roles = roles
        };
        return Ok(response);
    }


    [HttpPost("registerlink")]
    public async Task<IActionResult> SendRegisterLink([FromBody] EmailRequest request)
    {
        var email = request.Email;
        var result = await _authRepository.CreateRegisterLinkAsync(email);
        return result ? Ok("Registration link sent.") : BadRequest("Failed to send link.");
    }

    [HttpPost("verifyregister")]
    public async Task<IActionResult> VerifyRegisterCode([FromBody] CodeVerificationRequest model)
    {
        var token = await _authRepository.VerifyRegisterCodeAsync(model.Email, model.Code);
        return token == null ? Unauthorized("Invalid or expired code.") : Ok(new { token });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> SendForgottenPassword([FromBody] EmailRequest request)
    {
        var email = request.Email;
        var result = await _authRepository.CreateForgottenPasswordAsync(email);
        return result ? Ok("Reset code sent.") : BadRequest("Failed to send code.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
    {
        var result = await _authRepository.ResetPasswordAsync(model.Email, model.ResetCode, model.NewPassword);
        return result ? Ok("Password reset successful.") : BadRequest("Failed to reset password.");
    }

    //[Authorize(Roles = "Admin")]
    //[HttpGet("admin-data")]
    //public IActionResult GetAdminData()
    //{
    //    return Ok("This is protected for Admins");
    //}
}


