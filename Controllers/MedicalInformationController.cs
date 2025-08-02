using HRApi.Data;
using HRApi.Models;
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
        var medicalInfo = await _context
            .MedicalInformations.Include(m => m.Employee)
            .Select(m => new MedicalInformationDto
            {
                Id = m.Id,
                EmployeeId = m.EmployeeId,
                EmployeeName = m.Employee.FullName,
                BloodGroup = m.BloodGroup,
                Genotype = m.Genotype,
                MajorAllergies = m.MajorAllergies,
                ChronicConditions = m.ChronicConditions,
                Height = m.Employee.Height,
                Weight = m.Employee.Weight,
                CreatedAt = m.CreatedAt,
            })
            .ToListAsync();

        return Ok(medicalInfo);
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<MedicalInformationDto>> GetMedicalInformation(int id)
    {
        var medicalInfo = await _context
            .MedicalInformations.Include(m => m.Employee)
            .Where(m => m.Id == id)
            .Select(m => new MedicalInformationDto
            {
                Id = m.Id,
                EmployeeId = m.EmployeeId,
                EmployeeName = m.Employee.FullName,
                BloodGroup = m.BloodGroup,
                Genotype = m.Genotype,
                MajorAllergies = m.MajorAllergies,
                ChronicConditions = m.ChronicConditions,
                Height = m.Employee.Height,
                Weight = m.Employee.Weight,
                CreatedAt = m.CreatedAt,
            })
            .FirstOrDefaultAsync();

        if (medicalInfo == null)
        {
            return NotFound();
        }

        return Ok(medicalInfo);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<MedicalInformationDto>> GetMedicalInformationByEmployee(
        string employeeId
    )
    {
        var medicalInfo = await _context
            .MedicalInformations.Include(m => m.Employee)
            .Where(m => m.EmployeeId == employeeId)
            .Select(m => new MedicalInformationDto
            {
                Id = m.Id,
                EmployeeId = m.EmployeeId,
                EmployeeName = m.Employee.FullName,
                BloodGroup = m.BloodGroup,
                Genotype = m.Genotype,
                MajorAllergies = m.MajorAllergies,
                ChronicConditions = m.ChronicConditions,
                Height = m.Employee.Height,
                Weight = m.Employee.Weight,
                CreatedAt = m.CreatedAt,
            })
            .FirstOrDefaultAsync();

        if (medicalInfo == null)
        {
            return NotFound($"Medical information not found for employee {employeeId}");
        }

        return Ok(medicalInfo);
    }

    [HttpPost("create")]
    public async Task<ActionResult<MedicalInformationDto>> CreateMedicalInformation(
        CreateMedicalInformationDto createDto
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var employee = await _context.Employees.FindAsync(createDto.EmployeeId);
        if (employee == null)
        {
            return BadRequest($"Employee with ID {createDto.EmployeeId} not found");
        }

        var existingMedicalInfo = await _context.MedicalInformations.FirstOrDefaultAsync(m =>
            m.EmployeeId == createDto.EmployeeId
        );

        if (existingMedicalInfo != null)
        {
            return BadRequest(
                $"Medical information already exists for employee {createDto.EmployeeId}"
            );
        }

        var medicalInfo = new MedicalInformation
        {
            EmployeeId = createDto.EmployeeId,
            BloodGroup = createDto.BloodGroup,
            Genotype = createDto.Genotype,
            MajorAllergies = createDto.MajorAllergies,
            ChronicConditions = createDto.ChronicConditions,
            CreatedAt = DateTime.UtcNow,
        };

        if (createDto.Height.HasValue)
        {
            employee.Height = createDto.Height;
        }
        if (createDto.Weight.HasValue)
        {
            employee.Weight = createDto.Weight;
        }

        _context.MedicalInformations.Add(medicalInfo);
        await _context.SaveChangesAsync();

        var result = await _context
            .MedicalInformations.Include(m => m.Employee)
            .Where(m => m.Id == medicalInfo.Id)
            .Select(m => new MedicalInformationDto
            {
                Id = m.Id,
                EmployeeId = m.EmployeeId,
                EmployeeName = m.Employee.FullName,
                BloodGroup = m.BloodGroup,
                Genotype = m.Genotype,
                MajorAllergies = m.MajorAllergies,
                ChronicConditions = m.ChronicConditions,
                Height = m.Employee.Height,
                Weight = m.Employee.Weight,
                CreatedAt = m.CreatedAt,
            })
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetMedicalInformation), new { id = medicalInfo.Id }, result);
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<MedicalInformationDto>> UpdateMedicalInformation(
        int id,
        UpdateMedicalInformationDto updateDto
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var medicalInfo = await _context
            .MedicalInformations.Include(m => m.Employee)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (medicalInfo == null)
        {
            return NotFound();
        }

        medicalInfo.BloodGroup = updateDto.BloodGroup;
        medicalInfo.Genotype = updateDto.Genotype;
        medicalInfo.MajorAllergies = updateDto.MajorAllergies;
        medicalInfo.ChronicConditions = updateDto.ChronicConditions;

        if (updateDto.Height.HasValue)
        {
            medicalInfo.Employee.Height = updateDto.Height;
        }
        if (updateDto.Weight.HasValue)
        {
            medicalInfo.Employee.Weight = updateDto.Weight;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MedicalInformationExists(id))
            {
                return NotFound();
            }
            throw;
        }

        var result = new MedicalInformationDto
        {
            Id = medicalInfo.Id,
            EmployeeId = medicalInfo.EmployeeId,
            EmployeeName = medicalInfo.Employee.FullName,
            BloodGroup = medicalInfo.BloodGroup,
            Genotype = medicalInfo.Genotype,
            MajorAllergies = medicalInfo.MajorAllergies,
            ChronicConditions = medicalInfo.ChronicConditions,
            Height = medicalInfo.Employee.Height,
            Weight = medicalInfo.Employee.Weight,
            CreatedAt = medicalInfo.CreatedAt,
        };

        return Ok(result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteMedicalInformation(int id)
    {
        var medicalInfo = await _context.MedicalInformations.FindAsync(id);
        if (medicalInfo == null)
        {
            return NotFound();
        }

        _context.MedicalInformations.Remove(medicalInfo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MedicalInformationExists(int id)
    {
        return _context.MedicalInformations.Any(e => e.Id == id);
    }
}
