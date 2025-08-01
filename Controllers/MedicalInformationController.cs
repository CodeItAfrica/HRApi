using HRApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MedicalInformationController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicalInformationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetMedicalInformation()
    {
        var medicalInfo = await _context.MedicalInformations.ToListAsync();
        return Ok(medicalInfo);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetMedicalInformationByEmployee(string employeeId)
    {
        var medicalInfo = await _context
            .MedicalInformations.Where(m => m.EmployeeId == employeeId)
            .ToListAsync();

        if (medicalInfo == null || !medicalInfo.Any())
        {
            return NotFound($"No medical information found for employee ID {employeeId}.");
        }

        return Ok(medicalInfo);
    }
}
