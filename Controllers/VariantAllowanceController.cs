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

        var payrollHistory = await _context.PayrollHistories.FindAsync(request.PayrollHistoryId);

        if (payrollHistory == null)
        {
            return NotFound($"Payroll History not found.");
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        // Check if this variant allowance is already assigned to this payroll history
        var existingVariantPayrollAllowance =
            await _context.VariantPayrollAllowances.FirstOrDefaultAsync(vpa =>
                vpa.PayrollHistoryId == request.PayrollHistoryId
                && vpa.VariantAllowanceId == variantAllowance.Id
            );

        if (existingVariantPayrollAllowance != null)
        {
            // Update existing assignment
            existingVariantPayrollAllowance.Amount = request.Amount;
            existingVariantPayrollAllowance.GrantedBy = email ?? "System";
            // Note: CreatedAt remains unchanged, but you could add a LastModifiedAt field if needed
        }
        else
        {
            // Create new assignment
            var variantPayrollAllowance = new VariantPayrollAllowance
            {
                PayrollHistoryId = request.PayrollHistoryId,
                VariantAllowanceId = variantAllowance.Id,
                Amount = request.Amount,
                GrantedBy = email ?? "System",
                CreatedAt = DateTime.UtcNow,
            };

            _context.VariantPayrollAllowances.Add(variantPayrollAllowance);
        }

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

            string message =
                existingVariantPayrollAllowance != null
                    ? "Variant Allowance updated successfully"
                    : "Variant Allowance assigned successfully";

            return Ok(message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the amount: {ex.Message}");
        }
    }
}
