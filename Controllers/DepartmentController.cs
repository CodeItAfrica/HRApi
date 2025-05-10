using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]

public class DepartmentController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();
        return Ok(departments);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        if (request.DepartmentName == null)
        {
            return BadRequest("Department name cannot be null");
        }

        var normalizedDepartmentName = request.DepartmentName.Trim().ToLower();

        var existingDepartment = await _context.Departments
            .FirstOrDefaultAsync(r => r.DepartmentName.ToLower() == normalizedDepartmentName);

        if (existingDepartment != null)
        {
            return Conflict($"The department '{request.DepartmentName}' already exists.");
        }

        var department = new Department
        {
            DepartmentName = request.DepartmentName,
            CreatedAt = DateTime.UtcNow
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDepartments), new { id = department.Id }, department);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] CreateDepartmentRequest request)
    {
        if (string.IsNullOrEmpty(request.DepartmentName))
        {
            return BadRequest("Department name cannot be empty.");
        }

        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound("Department not found.");
        }

        var normalizedDepartmentName = request.DepartmentName.Trim().ToLower();
        var existingDepartment = await _context.Departments
            .FirstOrDefaultAsync(r => r.DepartmentName.ToLower() == normalizedDepartmentName && r.Id != id);

        if (existingDepartment != null)
        {
            return Conflict($"The department '{request.DepartmentName}' already exists.");
        }

        department.DepartmentName = request.DepartmentName;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Department has been updated successfully." });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound("Department not found.");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Department has been deleted successfully." });
    }
}

