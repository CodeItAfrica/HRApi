using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PaySheetController : ControllerBase
{
    private readonly AppDbContext _context;

    public PaySheetController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetPaySheets()
    {
        var paySheets = await _context.PaySheets.ToListAsync();
        return Ok(paySheets);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetPaySheetById(int id)
    {
        var paySheet = await _context.PaySheets.FindAsync(id);
        if (paySheet == null)
        {
            return NotFound("PaySheet not found.");
        }
        return Ok(paySheet);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePaySheet([FromBody] NameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("PaySheet name cannot be empty.");
        }

        var normalizedPaySheetName = request.Name.Trim().ToLower();

        var existingPaySheet = await _context.PaySheets.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedPaySheetName
        );

        if (existingPaySheet != null)
        {
            return Conflict($"The paySheet '{request.Name}' already exists.");
        }

        var paySheet = new PaySheet { Name = request.Name };

        _context.PaySheets.Add(paySheet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPaySheets), new { id = paySheet.Id }, paySheet);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdatePaySheet(int id, [FromBody] NameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("PaySheet name cannot be empty.");
        }

        var paySheet = await _context.PaySheets.FindAsync(id);
        if (paySheet == null)
        {
            return NotFound("PaySheet not found.");
        }

        var normalizedPaySheetName = request.Name.Trim().ToLower();
        var existingPaySheet = await _context.PaySheets.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedPaySheetName && r.Id != id
        );

        if (existingPaySheet != null)
        {
            return Conflict($"The paySheet '{request.Name}' already exists.");
        }

        paySheet.Name = request.Name;
        await _context.SaveChangesAsync();

        return Ok(new { message = "PaySheet has been updated successfully." });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePaySheet(int id)
    {
        var paySheet = await _context.PaySheets.FindAsync(id);
        if (paySheet == null)
        {
            return NotFound("PaySheet not found.");
        }

        _context.PaySheets.Remove(paySheet);
        await _context.SaveChangesAsync();

        return Ok(new { message = "PaySheet has been deleted successfully." });
    }

    [HttpPatch("update-employee")]
    public async Task<IActionResult> UpdateEmployeePaysheetId(
        [FromBody] UpdateEmployeePaySheetIDRequest request
    )
    {
        var employee = await _context
            .Employees.Where(e => e.Id == request.EmployeeId)
            .FirstOrDefaultAsync();
        if (employee == null)
        {
            return NotFound($"No employee found with ID {request.EmployeeId}");
        }

        var paySheet = await _context.PaySheets.FindAsync(request.PaySheetId);
        if (paySheet == null)
        {
            return NotFound($"No grade found with ID {request.PaySheetId}");
        }

        employee.PaySheetId = request.PaySheetId;

        await _context.SaveChangesAsync();
        return Ok("Employee PaySheetId updated successfully");
    }
}
