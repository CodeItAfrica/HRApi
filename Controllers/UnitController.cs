using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UnitController : ControllerBase
{
    private readonly AppDbContext _context;

    public UnitController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetUnits()
    {
        var units = await _context.Units.Include(u => u.Department).ToListAsync();
        return Ok(units);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUnit([FromBody] CreateUnitRequest request)
    {
        if (string.IsNullOrEmpty(request.UnitName))
        {
            return BadRequest("Unit name cannot be empty.");
        }

        var normalizedUnitName = request.UnitName.Trim().ToLower();

        var existingUnit = await _context.Units
            .FirstOrDefaultAsync(u => u.UnitName.ToLower() == normalizedUnitName);

        if (existingUnit != null)
        {
            return Conflict($"The unit '{request.UnitName}' already exists.");
        }

        var unit = new Unit
        {
            UnitName = request.UnitName,
            DepartmentId = request.DepartmentId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Units.Add(unit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUnits), new { id = unit.Id }, unit);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUnit(int id, [FromBody] CreateUnitRequest request)
    {
        if (string.IsNullOrEmpty(request.UnitName))
        {
            return BadRequest("Unit name cannot be empty.");
        }

        var unit = await _context.Units.FindAsync(id);
        if (unit == null)
        {
            return NotFound($"Unit with ID {id} not found.");
        }

        var normalizedUnitName = request.UnitName.Trim().ToLower();
        var existingUnit = await _context.Units
            .FirstOrDefaultAsync(u => u.UnitName.ToLower() == normalizedUnitName && u.Id != id);

        if (existingUnit != null)
        {
            return Conflict($"The unit '{request.UnitName}' already exists.");
        }

        unit.UnitName = request.UnitName;
        unit.DepartmentId = request.DepartmentId;

        _context.Units.Update(unit);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Unit has been updated successfully." });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUnit(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit == null)
        {
            return NotFound($"Unit with ID {id} not found.");
        }

        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetUnitById(int id)
    {
        var unit = await _context.Units.Include(u => u.Department).FirstOrDefaultAsync(u => u.Id == id);
        if (unit == null)
        {
            return NotFound($"Unit with ID {id} not found.");
        }

        return Ok(unit);
    }

    [HttpGet("get-by-department/{departmentId}")]
    public async Task<IActionResult> GetUnitsByDepartment(int departmentId)
    {
        var units = await _context.Units
            .Where(u => u.DepartmentId == departmentId)
            .Select(u => new
            {
                u.Id,
                u.UnitName,
                u.CreatedAt,
                u.DepartmentId,
                DepartmentName = u.Department.DepartmentName
            })
            .ToListAsync();

        if (units == null || !units.Any())
        {
            return NotFound($"No units found for department ID {departmentId}.");
        }

        return Ok(units);
    }

}