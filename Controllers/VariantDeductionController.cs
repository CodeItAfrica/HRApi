using System.Security.Claims;
using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api")]
public class VariantDeductionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PayrollService _payrollService;

    public VariantDeductionController(AppDbContext context, PayrollService payrollService)
    {
        _context = context;
        _payrollService = payrollService;
    }

    [HttpGet("variantDeduction/get")]
    public async Task<IActionResult> GetVariantDeductions()
    {
        var variantDeductions = await _context.VariantDeductions.ToListAsync();
        return Ok(variantDeductions);
    }

    [HttpGet("variantDeduction/get-actual")]
    public async Task<IActionResult> GetActualVariantDeductions()
    {
        var variantDeductions = await _context
            .VariantDeductions.Where(static d => d.Amount > 0)
            .ToListAsync();

        return Ok(variantDeductions);
    }

    [HttpPost("variantDeduction/create")]
    public async Task<IActionResult> CreateVariantDeduction([FromBody] CreateVariantRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Variant Deduction name cannot be empty.");
        }

        var normalizedVariantDeductionName = request.Name.Trim().ToLower();

        var existingVariantDeduction = await _context.VariantDeductions.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedVariantDeductionName
        );

        if (existingVariantDeduction != null)
        {
            return Conflict($"The Variant Deduction '{request.Name}' already exists.");
        }

        var variantDeduction = new VariantDeduction
        {
            Name = request.Name,
            Amount = request.Amount,
        };

        _context.VariantDeductions.Add(variantDeduction);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetVariantDeductions),
            new { id = variantDeduction.Id },
            variantDeduction
        );
    }

    [HttpPut("variantDeduction/update/{id}")]
    public async Task<IActionResult> VariantUpdateDeduction(
        int id,
        [FromBody] CreateVariantRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("VariantDeduction name cannot be empty.");
        }

        var variantDeduction = await _context.VariantDeductions.FindAsync(id);
        if (variantDeduction == null)
        {
            return NotFound("VariantDeduction not found.");
        }

        var normalizedVariantDeductionName = request.Name.Trim().ToLower();
        var existingVariantDeduction = await _context.VariantDeductions.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedVariantDeductionName && r.Id != id
        );

        if (existingVariantDeduction != null)
        {
            return Conflict($"The Variant Deduction '{request.Name}' already exists.");
        }

        variantDeduction.Name = request.Name;
        variantDeduction.Amount = request.Amount;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Variant Deduction has been updated successfully." });
    }

    [HttpDelete("variantDeduction/delete/{id}")]
    public async Task<IActionResult> DeleteVariantDeduction(int id)
    {
        var variantDeduction = await _context.VariantDeductions.FindAsync(id);
        if (variantDeduction == null)
        {
            return NotFound("Variant Deduction not found.");
        }

        _context.VariantDeductions.Remove(variantDeduction);
        await _context.SaveChangesAsync();

        return Ok(new { message = "VariantDeduction has been deleted successfully." });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("variantDeduction/assign")]
    public async Task<IActionResult> AssignVariantDeduction([FromBody] AssignVariantRequest request)
    {
        var variantDeduction = await _context.VariantDeductions.FindAsync(request.Id);

        if (variantDeduction == null)
        {
            return NotFound("Variant Deduction not found.");
        }

        var payroll = await _context.Payrolls.FirstOrDefaultAsync(p =>
            p.EmployeeId == request.EmployeeId
        );

        if (payroll == null)
        {
            return NotFound($"Payroll for employee not found.");
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        var payrollDeduction = new PayrollDeduction
        {
            EmployeeId = request.EmployeeId,
            VariantDeductionId = variantDeduction.Id,
            Amount = variantDeduction.Amount,
            Description = $"Auto-created for new variant Deduction type: {variantDeduction.Name}",
            LastDeductedBy = email ?? "System",
            LastDeductedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow,
        };

        _context.PayrollDeductions.Add(payrollDeduction);
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

            return Ok("Variant Deduction assigned successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the amount: {ex.Message}");
        }
    }
}
