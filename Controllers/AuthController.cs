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
    private readonly IDService _idService;

    public AuthController(AppDbContext context, IConfiguration config, AuthRepository authRepository, IDService idService)
    {
        _context = context;
        _config = config;
        _authRepository = authRepository;
        _idService = idService;
    }

    protected ActionResult ExceptionResult(Exception ex)
    {
        if (ex is null)
            return StatusCode(500, "SecureControllerBase.ExceptionResult() ex parameter cannot be null");

        if (ex is ArgumentException || ex is ArgumentNullException || ex is KeyNotFoundException)
            return BadRequest(ex.Message);

        if (ex.InnerException != null)
            return StatusCode(500, ex.Message + "\n\n\n --- inner exception --- " + ex.InnerException.ToString());

        return StatusCode(500, ex.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        // if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        //     return Unauthorized("Invalid email or password");

        if (user == null || user.PasswordHash != request.Password)
            return Unauthorized("Invalid email or password");

        // var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == user.EmployeeId);

        var roles = await _context.UserRoles
            .Where(r => r.UserId == user.Id)
            .Select(r => r.Role.RoleName)
            .ToListAsync();

        var token = _authRepository.GenerateJwt(user.Email, user.Id, roles);

        var response = new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Name = user?.Employee?.Surname + " " + user?.Employee?.OtherNames,
            Surname = user?.Employee?.Surname,
            EmployeeId = user?.EmployeeId,
            Roles = roles
        };
        return Ok(response);
    }


    [HttpPost("register-link")]
    public async Task<IActionResult> SendRegisterLink([FromBody] EmailRequest request)
    {
        var email = request.Email;
        var result = await _authRepository.CreateRegisterLinkAsync(email);
        return result ? Ok("Registration link sent.") : BadRequest("Failed to send link.");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // This ensures that the email and password fields are not empty, i needed to be sure because i was getting an error yesterday
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        // This is to ensure that a user with the same email does not exist so that each user will have a unique email address
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest("User with this email already exists.");
        }

        // This is where the system will check if the user is verified, it collects the email and the code and returns true or false depending on if the user is verified or not
        // bool isVerified = await _authRepository.VerifyRegisterCodeAsync(request.Email, request.code);
        // if (!isVerified)
        // {
        //     return BadRequest("Invalid or expired code. Please confirm your OTP.");
        // }

        using var transaction = await _context.Database.BeginTransactionAsync();

        // This is where the auto generated id will be set, both for the employeeId and the staffId ----- we might need to add a a foreign key constraint linking the employee table to the user table
        var staffIdNo = await _idService.GenerateUniqueStaffIdAsync();
        var employeeIdNo = await _idService.GenerateUniqueEmployeeIdAsync();

        try
        {
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
                AcctNo1 = request.AcctNo1,
                AcctName1 = request.AcctName1,
                AcctNo2 = request.AcctNo2,
                AcctName2 = request.AcctName2,
                BranchId = request.BranchId,
                // Branch = request.Branch,
                DeptId = request.DeptId,
                // Dept = request.Dept,
                UnitId = request.UnitId,
                // Unit = request.Unit,
                GradeId = request.GradeId,
                // Grade = request.Grade,
                BirthDate = request.BirthDate,
                HireDate = request.HireDate,
                NextKin = request.NextKin,
                KinAddress = request.KinAddress,
                KinPhone = request.KinPhone,
                KinRelationship = request.KinRelationship,
                Height = request.Height,
                Weight = request.Weight,
                Smoker = request.Smoker,
                DisableType = request.DisableType,
                Remarks = request.Remarks,
                Tag = request.Tag,
                Photo = request.Photo,
                PayFirstMonth = request.PayFirstMonth,
                SheetId2 = request.SheetId2,
                ConfirmStatus = request.ConfirmStatus,
                ConfirmDuration = request.ConfirmDuration,
                ConfirmationDate = request.ConfirmationDate,
                RetiredDate = request.RetiredDate,
                // Deleted = request.Deleted,
                Active = request.Active,
                SubmitBy = request.Email,
                SubmitOn = DateTime.UtcNow,
                // ModifiedBy = request.ModifiedBy,
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
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex);
        }
    }

    [HttpPost("verify-register-link")]
    public async Task<IActionResult> VerifyRegisterCode([FromBody] CodeVerificationRequest model)
    {
        var token = await _authRepository.VerifyRegisterCodeAsync(model.Email, model.Code);
        return token == null ? Unauthorized("Invalid or expired code.") : Ok(new { message = "OTP verification successful", token });
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

    // [Authorize(Roles = "Admin")]
    [HttpGet("admin-data")]
    public IActionResult GetAdminData()
    {
        return Ok("This is protected for Admins");
    }
}


