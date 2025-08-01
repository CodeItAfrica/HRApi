using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class InterestHobbiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public InterestHobbiesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetInterestHobbies()
    {
        var interestHobbies = await _context.InterestHobbies.ToListAsync();
        return Ok(interestHobbies);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetInterestHobbiesByEmployee(string employeeId)
    {
        var interestHobbies = await _context
            .InterestHobbies.Where(i => i.EmployeeId == employeeId)
            .ToListAsync();

        if (interestHobbies == null || !interestHobbies.Any())
        {
            return NotFound($"No interest hobbies found for employee ID {employeeId}.");
        }

        return Ok(interestHobbies);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateInterestHobby(
        [FromBody] CreateInterestHobbyRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Hobby))
        {
            return BadRequest("Interest hobby name cannot be empty.");
        }

        var normalizedHobby = request.Hobby.Trim().ToLower();

        var existingInterestHobby = await _context.InterestHobbies.FirstOrDefaultAsync(i =>
            i.Hobby.ToLower() == normalizedHobby && i.EmployeeId == request.EmployeeId
        );

        if (existingInterestHobby != null)
        {
            return Conflict($"The interest hobby '{request.Hobby}' already exists.");
        }

        var interestHobby = new InterestHobbies
        {
            EmployeeId = request.EmployeeId,
            Hobby = request.Hobby.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        _context.InterestHobbies.Add(interestHobby);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetInterestHobbies),
            new { id = interestHobby.Id },
            interestHobby
        );
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateInterestHobby(
        int id,
        [FromBody] UpdateInterestHobbyRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.Hobby))
        {
            return BadRequest("Interest hobby name cannot be empty.");
        }

        var interestHobby = await _context.InterestHobbies.FindAsync(id);
        if (interestHobby == null)
        {
            return NotFound($"Interest hobby with ID {id} not found.");
        }

        var normalizedHobby = request.Hobby.Trim().ToLower();
        var existingInterestHobby = await _context.InterestHobbies.FirstOrDefaultAsync(i =>
            i.Hobby.ToLower() == normalizedHobby
            && i.EmployeeId == interestHobby.EmployeeId
            && i.Id != id
        );

        if (existingInterestHobby != null)
        {
            return Conflict($"The interest hobby '{request.Hobby}' already exists.");
        }

        interestHobby.Hobby = request.Hobby.Trim();
        interestHobby.Description = request.Description?.Trim();
        interestHobby.CreatedAt = DateTime.UtcNow;

        _context.InterestHobbies.Update(interestHobby);
        await _context.SaveChangesAsync();

        return Ok(interestHobby);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteInterestHobby(int id)
    {
        var interestHobby = await _context.InterestHobbies.FindAsync(id);
        if (interestHobby == null)
        {
            return NotFound($"Interest hobby with ID {id} not found.");
        }

        _context.InterestHobbies.Remove(interestHobby);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
