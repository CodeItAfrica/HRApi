using System.Security.Claims;
using HRApi.Data;
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
}
