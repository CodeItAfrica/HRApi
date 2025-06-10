using System.Security.Claims;
using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GradeController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PayrollService _payrollService;

    public GradeController(AppDbContext context, PayrollService payrollService)
    {
        _context = context;
        _payrollService = payrollService;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetGrades()
    {
        var grades = await _context
            .Grades.Select(u => new
            {
                u.Id,
                u.GradeName,
                u.BaseSalary,
                u.Description,
                u.HousingAllowance,
                u.TransportAllowance,
                u.AnnualTax,
                u.CreatedAt,
                u.ModifiedDate,
            })
            .ToListAsync();
        return Ok(grades);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeRequest request)
    {
        if (string.IsNullOrEmpty(request.GradeName))
        {
            return BadRequest("Grade name cannot be empty.");
        }

        var normalizedGradeName = request.GradeName.Trim().ToLower();

        var existingGrade = await _context.Grades.FirstOrDefaultAsync(g =>
            g.GradeName.ToLower() == normalizedGradeName
        );

        if (existingGrade != null)
        {
            return Conflict($"The grade '{request.GradeName}' already exists.");
        }

        var grade = new Grade
        {
            GradeName = request.GradeName,
            Description = request.Description,
            BaseSalary = request.BaseSalary,
            HousingAllowance = request.HousingAllowance,
            TransportAllowance = request.TransportAllowance,
            AnnualTax = request.AnnualTax,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGrades), new { id = grade.Id }, grade);
    }

    // [HttpPut("update/{id}")]
    // public async Task<IActionResult> UpdateGrade(int id, [FromBody] CreateGradeRequest request)
    // {
    //     if (string.IsNullOrEmpty(request.GradeName))
    //     {
    //         return BadRequest("Grade name cannot be empty.");
    //     }

    //     var normalizedGradeName = request.GradeName.Trim().ToLower();

    //     var existingGrade = await _context.Grades.FirstOrDefaultAsync(g =>
    //         g.Id != id && g.GradeName.ToLower() == normalizedGradeName
    //     );

    //     if (existingGrade != null)
    //     {
    //         return Conflict($"The grade '{request.GradeName}' already exists.");
    //     }

    //     var grade = await _context.Grades.FindAsync(id);
    //     if (grade == null)
    //     {
    //         return NotFound($"No grade found with ID {id}");
    //     }

    //     grade.GradeName = request.GradeName;
    //     grade.Description = request.Description;
    //     grade.BaseSalary = request.BaseSalary;

    //     await _context.SaveChangesAsync();

    //     return Ok(grade);
    // }
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeRequest request)
    {
        if (request == null)
        {
            return BadRequest("Request body cannot be null");
        }

        var existingGrade = await _context.Grades.FindAsync(id);
        if (existingGrade == null)
        {
            return NotFound($"Grade with ID {id} not found");
        }

        if (!string.IsNullOrEmpty(request.GradeName))
        {
            existingGrade.GradeName = request.GradeName;
        }

        if (request.Description != null)
        {
            existingGrade.Description = request.Description;
        }

        if (request.BaseSalary.HasValue)
        {
            if (request.BaseSalary.Value < 0)
            {
                return BadRequest("BaseSalary cannot be negative");
            }
            existingGrade.BaseSalary = request.BaseSalary.Value;
        }

        if (request.HousingAllowance.HasValue)
        {
            if (request.HousingAllowance.Value < 0)
            {
                return BadRequest("HousingAllowance cannot be negative");
            }
            existingGrade.HousingAllowance = request.HousingAllowance.Value;
        }

        if (request.TransportAllowance.HasValue)
        {
            if (request.TransportAllowance.Value < 0)
            {
                return BadRequest("TransportAllowance cannot be negative");
            }
            existingGrade.TransportAllowance = request.TransportAllowance.Value;
        }

        if (request.AnnualTax.HasValue)
        {
            if (request.AnnualTax.Value < 0)
            {
                return BadRequest("AnnualTax cannot be negative");
            }
            existingGrade.AnnualTax = request.AnnualTax.Value;
        }

        if (request.IsActive.HasValue)
        {
            existingGrade.IsActive = request.IsActive.Value;
        }

        existingGrade.ModifiedDate = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingGrade);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Error updating grade: {ex.Message}");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null)
        {
            return NotFound($"No grade found with ID {id}");
        }

        var employeesWithGrade = await _context.Employees.Where(e => e.GradeId == id).CountAsync();

        if (employeesWithGrade > 0)
        {
            return BadRequest(
                $"Cannot delete grade. {employeesWithGrade} employee(s) are currently assigned to this grade."
            );
        }

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("assign-grade")]
    public async Task<IActionResult> AssignGrade([FromBody] AssignGradeRequest request)
    {
        var employee = await _context
            .Employees.Where(e => e.Id == request.EmployeeId.ToString())
            .FirstOrDefaultAsync();
        if (employee == null)
        {
            return NotFound($"No employee found with ID {request.EmployeeId}");
        }

        var grade = await _context.Grades.FindAsync(request.GradeId);
        if (grade == null)
        {
            return NotFound($"No grade found with ID {request.GradeId}");
        }

        var payroll = await _context.Payrolls.FirstOrDefaultAsync(p =>
            p.EmployeeId == request.EmployeeId
        );
        if (payroll == null)
        {
            return NotFound($"No payroll found for employee with ID {request.EmployeeId}");
        }

        employee.GradeId = request.GradeId;

        payroll.GradeId = request.GradeId;
        payroll.BaseSalary = grade.BaseSalary;
        payroll.HousingAllowance = grade.HousingAllowance;
        payroll.TransportAllowance = grade.TransportAllowance;
        payroll.AnnualTax = grade.AnnualTax;

        var newTotalAllowance = await _payrollService.CalculateTotalAllowancesAsync(
            request.EmployeeId
        );
        var newTotalDeduction = await _payrollService.CalculateTotalDeductionsAsync(
            request.EmployeeId
        );

        payroll.TotalAllowances = newTotalAllowance;
        payroll.TotalDeductions = newTotalDeduction;

        payroll.GrossSalary = payroll.BaseSalary + newTotalAllowance;
        payroll.NetSalary = payroll.GrossSalary - newTotalDeduction;
        payroll.UpdatedAt = DateTime.UtcNow;

        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        payroll.LastModifiedBy = email ?? "Unknown";

        await _context.SaveChangesAsync();
        return Ok("Grade assigned and payroll updated successfully.");
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetGrade(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null)
        {
            return NotFound($"No grade found with ID {id}");
        }

        return Ok(grade);
    }
}
