using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GradeController : ControllerBase
{
    private readonly AppDbContext _context;

    public GradeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetGrades()
    {
        var grades = await _context.Grades.Select(u => new { u.Id, u.GradeName, u.BaseSalary, u.Description }).ToListAsync();
        return Ok(grades);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeRequest request)
    {
        if (string.IsNullOrEmpty(request.GradeName))
        {
            return BadRequest("Grade name cannot be empty.");
        }

        var normalizedGradeName = request.GradeName.Trim().ToLower();

        var existingGrade = await _context.Grades
            .FirstOrDefaultAsync(g => g.GradeName.ToLower() == normalizedGradeName);

        if (existingGrade != null)
        {
            return Conflict($"The grade '{request.GradeName}' already exists.");
        }

        var grade = new Grade
        {
            GradeName = request.GradeName,
            Description = request.Description,
            BaseSalary = request.BaseSalary,
            CreatedAt = DateTime.UtcNow
        };

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGrades), new { id = grade.Id }, grade);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] CreateGradeRequest request)
    {
        if (string.IsNullOrEmpty(request.GradeName))
        {
            return BadRequest("Grade name cannot be empty.");
        }

        var normalizedGradeName = request.GradeName.Trim().ToLower();

        var existingGrade = await _context.Grades
            .FirstOrDefaultAsync(g => g.Id != id && g.GradeName.ToLower() == normalizedGradeName);

        if (existingGrade != null)
        {
            return Conflict($"The grade '{request.GradeName}' already exists.");
        }

        var grade = await _context.Grades.FindAsync(id);
        if (grade == null)
        {
            return NotFound($"No grade found with ID {id}");
        }

        grade.GradeName = request.GradeName;
        grade.Description = request.Description;
        grade.BaseSalary = request.BaseSalary;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null)
        {
            return NotFound($"No grade found with ID {id}");
        }

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetGrade(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null)
        {
            return NotFound($"No grade found with ID {id}");
        }

        return Ok(grade);
    }
}