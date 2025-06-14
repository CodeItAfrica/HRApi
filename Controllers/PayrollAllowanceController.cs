using System.Security.Claims;
using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PayrollAllowanceController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PayrollService _payrollService;

    public PayrollAllowanceController(AppDbContext context, PayrollService payrollService)
    {
        _context = context;
        _payrollService = payrollService;
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetAllowance(int id)
    {
        var allowance = await _context
            .PayrollAllowances.Include(p => p.Employee)
            .Include(p => p.AllowanceList)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (allowance == null)
        {
            return NotFound($"PayrollAllowance with ID {id} not found.");
        }

        var result = new
        {
            allowance.Id,
            allowance.Amount,
            Employee = new
            {
                allowance.Employee.FullName,
                allowance.Employee.OtherNames,
                allowance.Employee.Email,
            },
            allowance.AllowanceList,
            allowance.Description,
            allowance.LastGrantedBy,
            allowance.LastGrantedOn,
            allowance.CreatedAt,
        };

        return Ok(result);
    }

    [HttpGet("employee-allowance")]
    public async Task<IActionResult> GetAllowances([FromQuery] string? employeeId = null)
    {
        var query = _context.PayrollAllowances.Include(p => p.AllowanceList).AsQueryable();

        if (!string.IsNullOrEmpty(employeeId))
        {
            query = query.Where(p => p.EmployeeId == employeeId);
        }

        var allowances = await query.ToListAsync();
        return Ok(allowances);
    }

    [Authorize]
    [HttpPut("{id}/amount")]
    public async Task<IActionResult> UpdateAllowanceAmount(
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

        var allowance = await _context.PayrollAllowances.FirstOrDefaultAsync(p => p.Id == id);

        if (allowance == null)
        {
            return NotFound($"PayrollAllowance with ID {id} not found.");
        }

        var oldAmount = allowance.Amount;
        var newAmount = request.NewAmount;

        allowance.Amount = newAmount;

        string descriptionUpdate = _payrollService.BuildDescriptionUpdate(oldAmount, newAmount);

        if (string.IsNullOrEmpty(allowance.Description))
        {
            allowance.Description = descriptionUpdate;
        }
        else
        {
            allowance.Description += Environment.NewLine + descriptionUpdate;
        }

        allowance.LastGrantedBy = userEmail;
        allowance.LastGrantedOn = DateOnly.FromDateTime(DateTime.Now);

        try
        {
            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    OldAmount = oldAmount,
                    NewAmount = newAmount,
                    allowance.Description,
                    allowance.LastGrantedBy,
                    allowance.LastGrantedOn,
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
        ActionResult<IEnumerable<PayrollAllowanceHistory>>
    > GetEmployeeAllowanceHistoryByMonthYear(string employeeId, int month, int year)
    {
        if (month < 1 || month > 12)
            return BadRequest("Month must be between 1 and 12");

        if (year < 2020 || year > 2100)
            return BadRequest("Year must be between 2020 and 2100");

        var allowanceHistory = await _context
            .PayrollAllowanceHistories.Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId && a.Month == month && a.Year == year)
            .OrderBy(a => a.AllowanceName)
            .ToListAsync();

        if (!allowanceHistory.Any())
            return NotFound(
                $"No allowance history found for employee {employeeId} in {month}/{year}"
            );

        return Ok(allowanceHistory);
    }

    [HttpGet("history/employee/{employeeId}")]
    public async Task<
        ActionResult<IEnumerable<PayrollAllowanceHistory>>
    > GetAllEmployeeAllowanceHistory(string employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
            return NotFound($"Employee with ID {employeeId} not found");

        var allowanceHistory = await _context
            .PayrollAllowanceHistories.Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.Year)
            .ThenByDescending(a => a.Month)
            .ThenBy(a => a.AllowanceName)
            .ToListAsync();

        return Ok(allowanceHistory);
    }

    [HttpGet("history/allowance/{allowanceName}")]
    public async Task<ActionResult<IEnumerable<PayrollAllowanceHistory>>> GetAllowanceHistoryByName(
        string allowanceName
    )
    {
        var allowanceHistory = await _context
            .PayrollAllowanceHistories.Include(a => a.Employee)
            .Where(a => a.AllowanceName.ToLower() == allowanceName.ToLower())
            .OrderByDescending(a => a.Year)
            .ThenByDescending(a => a.Month)
            .ThenBy(a => a.Employee.Surname)
            .ToListAsync();

        return Ok(allowanceHistory);
    }

    [HttpGet("history/month/{month}/year/{year}")]
    public async Task<
        ActionResult<IEnumerable<PayrollAllowanceHistory>>
    > GetAllowanceHistoryByMonthYear(int month, int year)
    {
        if (month < 1 || month > 12)
            return BadRequest("Month must be between 1 and 12");

        if (year < 2020 || year > 2100)
            return BadRequest("Year must be between 2020 and 2100");

        var allowanceHistory = await _context
            .PayrollAllowanceHistories.Include(a => a.Employee)
            .Where(a => a.Month == month && a.Year == year)
            .OrderBy(a => a.Employee.Surname)
            .ThenBy(a => a.AllowanceName)
            .ToListAsync();

        return Ok(allowanceHistory);
    }

    [HttpGet("history/{id}")]
    public async Task<ActionResult<PayrollAllowanceHistory>> GetPayrollAllowanceHistory(int id)
    {
        var allowanceHistory = await _context
            .PayrollAllowanceHistories.Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (allowanceHistory == null)
            return NotFound();

        return Ok(allowanceHistory);
    }
}
