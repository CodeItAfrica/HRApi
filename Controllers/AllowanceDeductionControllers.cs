using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api")]
public class AllowanceDeductionController : ControllerBase
{
    private readonly AppDbContext _context;

    public AllowanceDeductionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("allowanceList/get")]
    public async Task<IActionResult> GetAllowanceLists()
    {
        var allowanceLists = await _context.AllowanceLists.ToListAsync();
        return Ok(allowanceLists);
    }

    [HttpPost("allowanceList/create")]
    public async Task<IActionResult> CreateAllowanceList([FromBody] NameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Allowance List name cannot be empty.");
        }

        var normalizedAllowanceListName = request.Name.Trim().ToLower();

        var existingAllowanceList = await _context.AllowanceLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedAllowanceListName
        );

        if (existingAllowanceList != null)
        {
            return Conflict($"The allowanceList '{request.Name}' already exists.");
        }

        var allowanceList = new AllowanceList { Name = request.Name };

        _context.AllowanceLists.Add(allowanceList);
        await _context.SaveChangesAsync();

        var employees = await _context.Employees.Select(e => e.Id).ToListAsync();

        var payrollAllowances = new List<PayrollAllowance>();

        foreach (var employeeId in employees)
        {
            var payrollAllowance = new PayrollAllowance
            {
                EmployeeId = employeeId,
                AllowanceListId = allowanceList.Id,
                Amount = 0m,
                Description = $"Auto-created for new allowance type: {allowanceList.Name}",
                LastGrantedBy = "System",
                LastGrantedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow,
            };

            payrollAllowances.Add(payrollAllowance);
        }

        if (payrollAllowances.Any())
        {
            await _context.PayrollAllowances.AddRangeAsync(payrollAllowances);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(
            nameof(GetAllowanceLists),
            new { id = allowanceList.Id },
            allowanceList
        );
    }

    [HttpPut("allowanceList/update/{id}")]
    public async Task<IActionResult> UpdateAllowanceList(int id, [FromBody] NameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("AllowanceList name cannot be empty.");
        }

        var allowanceList = await _context.AllowanceLists.FindAsync(id);
        if (allowanceList == null)
        {
            return NotFound("AllowanceList not found.");
        }

        var normalizedAllowanceListName = request.Name.Trim().ToLower();
        var existingAllowanceList = await _context.AllowanceLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedAllowanceListName && r.Id != id
        );

        if (existingAllowanceList != null)
        {
            return Conflict($"The allowanceList '{request.Name}' already exists.");
        }

        allowanceList.Name = request.Name;
        await _context.SaveChangesAsync();

        return Ok(new { message = "AllowanceList has been updated successfully." });
    }

    [HttpDelete("allowanceList/delete/{id}")]
    public async Task<IActionResult> DeleteAllowanceList(int id)
    {
        var allowanceList = await _context.AllowanceLists.FindAsync(id);
        if (allowanceList == null)
        {
            return NotFound("AllowanceList not found.");
        }

        var payrollAllowances = await _context
            .PayrollAllowances.Where(pa => pa.AllowanceListId == id)
            .ToListAsync();

        if (payrollAllowances.Any())
        {
            _context.PayrollAllowances.RemoveRange(payrollAllowances);
        }

        _context.AllowanceLists.Remove(allowanceList);
        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                message = "AllowanceList has been deleted successfully.",
                deletedPayrollAllowances = payrollAllowances.Count,
            }
        );
    }

    [HttpGet("deductionList/get")]
    public async Task<IActionResult> GetDeductionLists()
    {
        var DeductionLists = await _context.DeductionLists.ToListAsync();
        return Ok(DeductionLists);
    }

    [HttpPost("deductionList/create")]
    public async Task<IActionResult> CreateDeductionList([FromBody] NameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Deduction List name cannot be empty.");
        }

        var normalizedDeductionListName = request.Name.Trim().ToLower();

        var existingDeductionList = await _context.DeductionLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedDeductionListName
        );

        if (existingDeductionList != null)
        {
            return Conflict($"The DeductionList '{request.Name}' already exists.");
        }

        var DeductionList = new DeductionList { Name = request.Name };

        _context.DeductionLists.Add(DeductionList);
        await _context.SaveChangesAsync();

        var employees = await _context.Employees.Select(e => e.Id).ToListAsync();

        var payrollDeductions = new List<PayrollDeduction>();

        foreach (var employeeId in employees)
        {
            var payrollDeduction = new PayrollDeduction
            {
                EmployeeId = employeeId,
                DeductionListId = DeductionList.Id,
                Amount = 0m,
                Description = $"Auto-created for new Deduction type: {DeductionList.Name}",
                LastDeductedBy = "System",
                LastDeductedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow,
            };

            payrollDeductions.Add(payrollDeduction);
        }

        if (payrollDeductions.Any())
        {
            await _context.PayrollDeductions.AddRangeAsync(payrollDeductions);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(
            nameof(GetDeductionLists),
            new { id = DeductionList.Id },
            DeductionList
        );
    }

    [HttpPut("deductionList/update/{id}")]
    public async Task<IActionResult> UpdateDeductionList(int id, [FromBody] NameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("DeductionList name cannot be empty.");
        }

        var DeductionList = await _context.DeductionLists.FindAsync(id);
        if (DeductionList == null)
        {
            return NotFound("DeductionList not found.");
        }

        var normalizedDeductionListName = request.Name.Trim().ToLower();
        var existingDeductionList = await _context.DeductionLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedDeductionListName && r.Id != id
        );

        if (existingDeductionList != null)
        {
            return Conflict($"The DeductionList '{request.Name}' already exists.");
        }

        DeductionList.Name = request.Name;
        await _context.SaveChangesAsync();

        return Ok(new { message = "DeductionList has been updated successfully." });
    }

    [HttpDelete("deductionList/delete/{id}")]
    public async Task<IActionResult> DeleteDeductionList(int id)
    {
        var DeductionList = await _context.DeductionLists.FindAsync(id);
        if (DeductionList == null)
        {
            return NotFound("DeductionList not found.");
        }

        var payrollDeductions = await _context
            .PayrollDeductions.Where(pa => pa.DeductionListId == id)
            .ToListAsync();

        if (payrollDeductions.Any())
        {
            _context.PayrollDeductions.RemoveRange(payrollDeductions);
        }

        _context.DeductionLists.Remove(DeductionList);
        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                message = "DeductionList has been deleted successfully.",
                deletedPayrollDeductions = payrollDeductions.Count,
            }
        );
    }
}
