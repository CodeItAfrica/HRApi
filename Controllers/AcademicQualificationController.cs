using HRApi.Data;
using HRApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AcademicQualificationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AcademicQualificationService _service;

    public AcademicQualificationController(
        AppDbContext context,
        AcademicQualificationService service
    )
    {
        _context = context;
        _service = service;
    }

    [HttpGet("get")]
    public async Task<
        ActionResult<IEnumerable<AcademicQualificationResponseDto>>
    > GetAcademicQualifications()
    {
        var qualifications = await _service.GetAllAsync();
        return Ok(qualifications);
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<AcademicQualificationResponseDto>> GetAcademicQualification(
        int id
    )
    {
        var qualification = await _service.GetByIdAsync(id);

        if (qualification == null)
        {
            return NotFound($"Academic qualification with ID {id} not found.");
        }

        return Ok(qualification);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<
        ActionResult<IEnumerable<AcademicQualificationResponseDto>>
    > GetQualificationsByEmployee(string employeeId)
    {
        var qualifications = await _service.GetByEmployeeIdAsync(employeeId);
        return Ok(qualifications);
    }

    [HttpPost("create")]
    public async Task<ActionResult<AcademicQualificationResponseDto>> CreateAcademicQualification(
        AcademicQualificationCreateDto dto
    )
    {
        try
        {
            var qualification = await _service.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetAcademicQualification),
                new { id = qualification.Id },
                qualification
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<AcademicQualificationResponseDto>> UpdateAcademicQualification(
        int id,
        AcademicQualificationUpdateDto dto
    )
    {
        try
        {
            var qualification = await _service.UpdateAsync(id, dto);

            if (qualification == null)
            {
                return NotFound($"Academic qualification with ID {id} not found.");
            }

            return Ok(qualification);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAcademicQualification(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound($"Academic qualification with ID {id} not found.");
        }

        return NoContent();
    }
}
