using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly AuthRepository _authRepository;
    private readonly IDService _idService;
    private readonly PayrollService _payrollService;

    public AuthController(
        AppDbContext context,
        IConfiguration config,
        AuthRepository authRepository,
        IDService idService,
        PayrollService payrollService
    )
    {
        _context = context;
        _config = config;
        _authRepository = authRepository;
        _idService = idService;
        _payrollService = payrollService;
    }

    protected ActionResult ExceptionResult(Exception ex)
    {
        if (ex is null)
            return StatusCode(
                500,
                "SecureControllerBase.ExceptionResult() ex parameter cannot be null"
            );

        if (ex is ArgumentException || ex is ArgumentNullException || ex is KeyNotFoundException)
            return BadRequest(ex.Message);

        if (ex.InnerException != null)
            return StatusCode(
                500,
                ex.Message + "\n\n\n --- inner exception --- " + ex.InnerException.ToString()
            );

        return StatusCode(500, ex.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context
            .Users.Include(u => u.Employee)
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        // if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        //     return Unauthorized("Invalid email or password");

        if (user == null || user.PasswordHash != request.Password)
            return Unauthorized("Invalid email or password");

        // var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == user.EmployeeId);

        var roles = await _context
            .UserRoles.Where(r => r.UserId == user.Id)
            .Select(r => r.Role.RoleName)
            .ToListAsync();

        var token = _authRepository.GenerateJwt(user.Email, user.Id, roles);

        var response = new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Photo = user.Employee?.Photo,
            Name = user?.Employee?.Surname + ", " + user?.Employee?.OtherNames,
            Surname = user?.Employee?.Surname,
            EmployeeId = user?.EmployeeId,
            Roles = roles,
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
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request, IFormFile? Photo)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");

        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest("User with this email already exists.");
        }

        var grade = await _context.Grades.FindAsync(request.GradeId);
        if (grade == null)
        {
            return NotFound($"No grade found with ID {request.GradeId}");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        var staffIdNo = await _idService.GenerateUniqueStaffIdAsync();

        try
        {
            string? savedFileName = null;

            if (Photo != null && Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }

                savedFileName = fileName;
            }

            var employee = new Employee
            {
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
                BankName1 = request.BankName1,
                AcctNo2 = request.AcctNo2,
                AcctName2 = request.AcctName2,
                BankName2 = request.BankName2,
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
                Photo = savedFileName,
                PayFirstMonth = request.PayFirstMonth,
                PaySheetId = request.PaySheetId,
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
                EmployeeId = employee.Id,
                Email = request.Email,
                // PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PasswordHash = request.Password,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = 1,
                AssignedAt = DateTime.UtcNow,
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            var payroll = new Payroll
            {
                EmployeeId = employee.Id,
                GradeId = request.GradeId,
                BaseSalary = grade.BaseSalary,
                HousingAllowance = grade.HousingAllowance,
                TransportAllowance = grade.TransportAllowance,
                AnnualTax = grade.AnnualTax,
                TotalAllowances = 0m,
                TotalDeductions = 0m,
                GrossSalary = grade.BaseSalary + grade.HousingAllowance + grade.TransportAllowance,
                NetSalary = grade.BaseSalary + grade.HousingAllowance + grade.TransportAllowance,
                AccountNumber = request.AcctNo1,
                BankName = request.BankName1,
                CreatedBy = request.Email,
                LastModifiedBy = request.Email,
            };

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            await _payrollService.CreatePayrollAllowancesForEmployee(employee.Id);
            await _payrollService.CreatePayrollDeductionsForEmployee(employee.Id);

            var newPayroll = await _context.Payrolls.FirstOrDefaultAsync(p =>
                p.EmployeeId == employee.Id
            );

            if (newPayroll == null)
            {
                return NotFound($"Created Payroll not found.");
            }

            try
            {
                var newTotalAllowance = await _payrollService.CalculateTotalAllowancesAsync(
                    employee.Id
                );
                var newTotalDeduction = await _payrollService.CalculateTotalDeductionsAsync(
                    employee.Id
                );

                newPayroll.TotalAllowances = newTotalAllowance;
                newPayroll.TotalDeductions = newTotalDeduction;

                newPayroll.GrossSalary = newPayroll.BaseSalary + newTotalAllowance;
                newPayroll.NetSalary = newPayroll.GrossSalary - newTotalDeduction;
                newPayroll.LastModifiedBy = "System";
                newPayroll.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok("Employee and user registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"An error occurred while updating the amount: {ex.Message}"
                );
            }
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
        return token == null
            ? Unauthorized("Invalid or expired code.")
            : Ok(new { message = "OTP verification successful", token });
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
        var result = await _authRepository.ResetPasswordAsync(
            model.Email,
            model.ResetCode,
            model.NewPassword
        );
        return result ? Ok("Password reset successful.") : BadRequest("Failed to reset password.");
    }
}

//    // [Authorize(Roles = "Admin")]
//    [HttpGet("admin-data")]
//    public IActionResult GetAdminData()
//    {
//        return Ok("This is protected for Admins");
//    }
//}
