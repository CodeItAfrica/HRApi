using System.Security.Claims;
using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api")]
public class VariantAllowanceController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PayrollService _payrollService;

    public VariantAllowanceController(AppDbContext context, PayrollService payrollService)
    {
        _context = context;
        _payrollService = payrollService;
    }

    [HttpGet("variantAllowance/get")]
    public async Task<IActionResult> GetVariantAllowances()
    {
        var variantAllowances = await _context.VariantAllowances.ToListAsync();
        return Ok(variantAllowances);
    }

    [HttpGet("variantAllowance/get-actual")]
    public async Task<IActionResult> GetActualVariantAllowances()
    {
        var variantAllowances = await _context
            .VariantAllowances.Where(static d => d.Amount > 0)
            .ToListAsync();

        return Ok(variantAllowances);
    }

    [HttpPost("variantAllowance/create")]
    public async Task<IActionResult> CreateVariantAllowance([FromBody] CreateVariantRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Variant allowance name cannot be empty.");
        }

        var normalizedVariantAllowanceName = request.Name.Trim().ToLower();

        var existingVariantAllowance = await _context.VariantAllowances.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedVariantAllowanceName
        );

        if (existingVariantAllowance != null)
        {
            return Conflict($"The Variant allowance '{request.Name}' already exists.");
        }

        var variantAllowance = new VariantAllowance
        {
            Name = request.Name,
            Amount = request.Amount,
        };

        _context.VariantAllowances.Add(variantAllowance);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetVariantAllowances),
            new { id = variantAllowance.Id },
            variantAllowance
        );
    }

    [HttpPut("variantAllowance/update/{id}")]
    public async Task<IActionResult> VariantUpdateAllowance(
        int id,
        [FromBody] CreateVariantRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("VariantAllowance name cannot be empty.");
        }

        var variantAllowance = await _context.VariantAllowances.FindAsync(id);
        if (variantAllowance == null)
        {
            return NotFound("VariantAllowance not found.");
        }

        var normalizedVariantAllowanceName = request.Name.Trim().ToLower();
        var existingVariantAllowance = await _context.VariantAllowances.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedVariantAllowanceName && r.Id != id
        );

        if (existingVariantAllowance != null)
        {
            return Conflict($"The Variant allowance '{request.Name}' already exists.");
        }

        variantAllowance.Name = request.Name;
        variantAllowance.Amount = request.Amount;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Variant Allowance has been updated successfully." });
    }

    [HttpDelete("variantAllowance/delete/{id}")]
    public async Task<IActionResult> DeleteVariantAllowance(int id)
    {
        var variantAllowance = await _context.VariantAllowances.FindAsync(id);
        if (variantAllowance == null)
        {
            return NotFound("Variant Allowance not found.");
        }

        _context.VariantAllowances.Remove(variantAllowance);
        await _context.SaveChangesAsync();

        return Ok(new { message = "VariantAllowance has been deleted successfully." });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("variantAllowance/assign")]
    public async Task<IActionResult> AssignVariantAllowance([FromBody] AssignVariantRequest request)
    {
        var variantAllowance = await _context.VariantAllowances.FindAsync(request.Id);

        if (variantAllowance == null)
        {
            return NotFound("Variant Allowance not found.");
        }

        var payroll = await _context.Payrolls.FirstOrDefaultAsync(p =>
            p.EmployeeId == request.EmployeeId
        );

        if (payroll == null)
        {
            return NotFound($"Payroll for employee not found.");
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        var payrollAllowance = new PayrollAllowance
        {
            EmployeeId = request.EmployeeId,
            VariantAllowanceId = variantAllowance.Id,
            Amount = variantAllowance.Amount,
            Description = $"Auto-created for new variant allowance type: {variantAllowance.Name}",
            LastGrantedBy = email ?? "System",
            LastGrantedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow,
        };

        _context.PayrollAllowances.Add(payrollAllowance);
        await _context.SaveChangesAsync();

        try
        {
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
            payroll.LastModifiedBy = email ?? "System";
            payroll.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Variant Allowance assigned successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the amount: {ex.Message}");
        }
    }
}
