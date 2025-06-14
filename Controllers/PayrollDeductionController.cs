using System.Security.Claims;
using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PayrollDeductionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PayrollService _payrollService;

    public PayrollDeductionController(AppDbContext context, PayrollService payrollService)
    {
        _context = context;
        _payrollService = payrollService;
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetDeduction(int id)
    {
        var deduction = await _context
            .PayrollDeductions.Include(p => p.Employee)
            .Include(p => p.DeductionList)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (deduction == null)
        {
            return NotFound($"PayrollDeduction with ID {id} not found.");
        }

        var result = new
        {
            deduction.Id,
            deduction.Amount,
            Employee = new
            {
                deduction.Employee.FullName,
                deduction.Employee.OtherNames,
                deduction.Employee.Email,
            },
            deduction.DeductionList,
            deduction.Description,
            deduction.LastDeductedBy,
            deduction.LastDeductedOn,
            deduction.CreatedAt,
        };

        return Ok(result);
    }

    [HttpGet("employee-deduction")]
    public async Task<IActionResult> GetDeductions([FromQuery] string? employeeId = null)
    {
        var query = _context.PayrollDeductions.Include(p => p.DeductionList).AsQueryable();

        if (!string.IsNullOrEmpty(employeeId))
        {
            query = query.Where(p => p.EmployeeId == employeeId);
        }

        var deductions = await query.ToListAsync();
        return Ok(deductions);
    }

    [Authorize]
    [HttpPut("{id}/amount")]
    public async Task<IActionResult> UpdateDeductionAmount(
        int id,
        [FromBody] UpdateAmountRequest request
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized("Unable to identify user from token.");
        }

        var deduction = await _context.PayrollDeductions.FirstOrDefaultAsync(p => p.Id == id);

        if (deduction == null)
        {
            return NotFound($"PayrollDeduction with ID {id} not found.");
        }

        var oldAmount = deduction.Amount;
        var newAmount = request.NewAmount;

        deduction.Amount = newAmount;

        string descriptionUpdate = _payrollService.BuildDescriptionUpdate(oldAmount, newAmount);

        if (string.IsNullOrEmpty(deduction.Description))
        {
            deduction.Description = descriptionUpdate;
        }
        else
        {
            deduction.Description += Environment.NewLine + descriptionUpdate;
        }

        deduction.LastDeductedBy = userEmail;
        deduction.LastDeductedOn = DateOnly.FromDateTime(DateTime.Now);

        try
        {
            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    OldAmount = oldAmount,
                    NewAmount = newAmount,
                    deduction.Description,
                    deduction.LastDeductedBy,
                    deduction.LastDeductedOn,
                    Message = "Amount updated successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the amount: {ex.Message}");
        }
    }

    [HttpGet("history/employee/{employeeId}/month/{month}/year/{year}")]
    public async Task<
        ActionResult<IEnumerable<PayrollDeductionHistory>>
    > GetEmployeeDeductionHistoryByMonthYear(string employeeId, int month, int year)
    {
        if (month < 1 || month > 12)
            return BadRequest("Month must be between 1 and 12");

        if (year < 2020 || year > 2100)
            return BadRequest("Year must be between 2020 and 2100");

        var deductionHistory = await _context
            .PayrollDeductionHistories.Include(d => d.Employee)
            .Where(d => d.EmployeeId == employeeId && d.Month == month && d.Year == year)
            .OrderBy(d => d.DeductionName)
            .ToListAsync();

        if (!deductionHistory.Any())
            return NotFound(
                $"No deduction history found for employee {employeeId} in {month}/{year}"
            );

        return Ok(deductionHistory);
    }

    [HttpGet("history/employee/{employeeId}")]
    public async Task<
        ActionResult<IEnumerable<PayrollDeductionHistory>>
    > GetAllEmployeeDeductionHistory(string employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
            return NotFound($"Employee with ID {employeeId} not found");

        var deductionHistory = await _context
            .PayrollDeductionHistories.Include(d => d.Employee)
            .Where(d => d.EmployeeId == employeeId)
            .OrderByDescending(d => d.Year)
            .ThenByDescending(d => d.Month)
            .ThenBy(d => d.DeductionName)
            .ToListAsync();

        return Ok(deductionHistory);
    }

    [HttpGet("history/deduction/{deductionName}")]
    public async Task<ActionResult<IEnumerable<PayrollDeductionHistory>>> GetDeductionHistoryByName(
        string deductionName
    )
    {
        var deductionHistory = await _context
            .PayrollDeductionHistories.Include(d => d.Employee)
            .Where(d => d.DeductionName.ToLower() == deductionName.ToLower())
            .OrderByDescending(d => d.Year)
            .ThenByDescending(d => d.Month)
            .ThenBy(d => d.Employee.Surname)
            .ToListAsync();

        return Ok(deductionHistory);
    }

    [HttpGet("history/month/{month}/year/{year}")]
    public async Task<
        ActionResult<IEnumerable<PayrollDeductionHistory>>
    > GetDeductionHistoryByMonthYear(int month, int year)
    {
        if (month < 1 || month > 12)
            return BadRequest("Month must be between 1 and 12");

        if (year < 2020 || year > 2100)
            return BadRequest("Year must be between 2020 and 2100");

        var deductionHistory = await _context
            .PayrollDeductionHistories.Include(d => d.Employee)
            .Where(d => d.Month == month && d.Year == year)
            .OrderBy(d => d.Employee.Surname)
            .ThenBy(d => d.DeductionName)
            .ToListAsync();

        return Ok(deductionHistory);
    }

    [HttpGet("history/{id}")]
    public async Task<ActionResult<PayrollDeductionHistory>> GetPayrollDeductionHistory(int id)
    {
        var deductionHistory = await _context
            .PayrollDeductionHistories.Include(d => d.Employee)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deductionHistory == null)
            return NotFound();

        return Ok(deductionHistory);
    }
}
