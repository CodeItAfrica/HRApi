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

        var payrollHistory = await _context.PayrollHistories.FindAsync(request.PayrollHistoryId);

        if (payrollHistory == null)
        {
            return NotFound($"Payroll History not found.");
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        var variantPayrollDeduction = new VariantPayrollDeduction
        {
            PayrollHistoryId = request.PayrollHistoryId,
            VariantDeductionId = variantDeduction.Id,
            Amount = variantDeduction.Amount,
            GrantedBy = email ?? "System",
            CreatedAt = DateTime.UtcNow,
        };

        _context.VariantPayrollDeductions.Add(variantPayrollDeduction);
        await _context.SaveChangesAsync();

        try
        {
            var newTotalVariantAllowance =
                await _payrollService.CalculateTotalVariantAllowanceAsync(request.PayrollHistoryId);

            var newTotalVariantDeduction =
                await _payrollService.CalculateTotalVariantDeductionAsync(request.PayrollHistoryId);

            decimal monthlyTax = payrollHistory?.AnnualTax / 12 ?? 0;

            decimal allDeductions =
                payrollHistory?.TotalDeductions + monthlyTax + newTotalVariantDeduction ?? 0;

            payrollHistory.TotalVariantAllowances = newTotalVariantAllowance;
            payrollHistory.TotalVariantDeductions = newTotalVariantDeduction;

            payrollHistory.GrossSalary =
                payrollHistory.BaseSalary
                + payrollHistory.HousingAllowance
                + payrollHistory.TransportAllowance
                + payrollHistory.TotalAllowances
                + newTotalVariantAllowance;

            payrollHistory.NetSalary = payrollHistory.GrossSalary - allDeductions;

            await _context.SaveChangesAsync();

            return Ok("Variant Deduction assigned successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the amount: {ex.Message}");
        }
    }
}
