using Azure.Core;
using HRApi.Data;
using HRApi.Models;
using HRApi.Repository;
using HRApi.Services;
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
    private readonly EmployeeService _employeeService;

    public AuthController(AppDbContext context, IConfiguration config, AuthRepository authRepository, EmployeeService employeeService)
    {
        _context = context;
        _config = config;
        _authRepository = authRepository;
        _employeeService = employeeService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(HRApi.Models.LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        // if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        //     return Unauthorized("Invalid email or password");

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] HRApi.Models.RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest("User with this email already exists.");
        }

        bool isVerified = await _authRepository.VerifyRegisterCodeAsync(request.Email, request.code);
        if (!isVerified)
        {
            return BadRequest("Invalid or expired code. Please confirm your OTP.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        var staffIdNo = await _employeeService.GenerateUniqueStaffIdAsync();
        var employeeIdNo = await _employeeService.GenerateUniqueEmployeeIdAsync();

        try {
            var employee = new Employee
            {
                Id = employeeIdNo,
                StaffIdNo = staffIdNo,
                Title = request.Title,
                Surname = request.Surname,
                OtherNames = request.OtherNames,
                Telephone = request.Phone,
                Email = request.Email,
                Email2 = request.Email2,
                Address = request.Address,
                State = request.State,
                Country = request.Country,
                Sex = request.Sex,
                MobilePhone = request.MobilePhone,
                MaritalStatus = request.MaritalStatus,
                StateOrigin = request.StateOrigin,
                NationalIdNo = request.NationalIdNo,
                AccountNo1 = request.AccountNo1,
                AccountName1 = request.AccountName1,
                AccountNo2 = request.AccountNo2,
                AccountName2 = request.AccountName2,
                BranchId = request.BranchId,
                Branch = request.Branch,
                DepartmentId = request.DepartmentId,
                Department = request.Department,
                UnitId = request.UnitId,
                Unit = request.Unit,
                GradeId = request.GradeId,
                Grade = request.Grade,
                BirthDate = request.BirthDate,
                HireDate = request.HireDate,
                NextOfKin = request.NextOfKin,
                KinAddress = request.KinAddress,
                KinPhone = request.KinPhone,
                KinRelationship = request.KinRelationship,
                Height = request.Height,
                Weight = request.Weight,
                Smoker = request.Smoker,
                DisabilityType = request.DisabilityType,
                Remarks = request.Remarks,
                Tag = request.Tag,
                Photo = request.Photo,
                PayFirstMonth = request.PayFirstMonth,
                SheetId2 = request.SheetId2,
                ConfirmStatus = request.ConfirmStatus,
                ConfirmDuration = request.ConfirmDuration,
                ConfirmationDate = request.ConfirmationDate,
                RetiredDate = request.RetiredDate,
                Deleted = request.Deleted,
                Active = request.Active,
                SubmittedBy = request.SubmittedBy,
                SubmittedOn = DateTime.UtcNow,
                ModifiedBy = request.ModifiedBy,
                ModifiedOn = DateTime.UtcNow,
                HmoName = request.HmoName,
                HmoId = request.HmoId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var user = new User
            {
                EmployeeId = employeeIdNo,
                EmployeeName = request.OtherNames + " " + request.Surname,
                Email = request.Email,
                // PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PasswordHash = request.Password,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return Ok("Employee and user registered successfully.");
        } catch {
            await transaction.RollbackAsync();
            return StatusCode(500, "An error occurred during registration.");
        }
    }

    // We would not need a verify register route as the code will be verified during the registration

    // [HttpPost("verifyregister")]
    // public async Task<IActionResult> VerifyRegisterCode([FromBody] CodeVerificationRequest model)
    // {
    //     var token = await _authRepository.VerifyRegisterCodeAsync(model.Email, model.Code);
    //     return token == null ? Unauthorized("Invalid or expired code.") : Ok(new { token });
    // }

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

    // [Authorize(Roles = "Admin")]
    [HttpGet("admin-data")]
    public IActionResult GetAdminData()
    {
        return Ok("This is protected for Admins");
    }
}


