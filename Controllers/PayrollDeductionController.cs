using System.Security.Claims;
using HRApi.Data;
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
}
