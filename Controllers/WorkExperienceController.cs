using System.ComponentModel.DataAnnotations;
using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkExperienceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkExperienceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<WorkExperienceDto>>> GetWorkExperiences()
        {
            var workExperiences = await _context
                .WorkExperiences.Include(w => w.Employee)
                .Select(w => new WorkExperienceDto
                {
                    Id = w.Id,
                    EmployeeId = w.EmployeeId,
                    EmployeeName = w.Employee.FullName,
                    JobTitle = w.JobTitle,
                    CompanyName = w.CompanyName,
                    YearStarted = w.YearStarted,
                    YearEnded = w.YearEnded,
                    Industry = w.Industry,
                    Achievement = w.Achievement,
                    Duration =
                        $"{w.YearStarted} - {w.YearEnded} ({w.YearEnded - w.YearStarted} years)",
                    CreatedAt = w.CreatedAt,
                })
                .OrderBy(w => w.EmployeeName)
                .ThenByDescending(w => w.YearEnded)
                .ToListAsync();

            return Ok(workExperiences);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<WorkExperienceDto>> GetWorkExperience(int id)
        {
            var workExperience = await _context
                .WorkExperiences.Include(w => w.Employee)
                .Where(w => w.Id == id)
                .Select(w => new WorkExperienceDto
                {
                    Id = w.Id,
                    EmployeeId = w.EmployeeId,
                    EmployeeName = w.Employee.FullName,
                    JobTitle = w.JobTitle,
                    CompanyName = w.CompanyName,
                    YearStarted = w.YearStarted,
                    YearEnded = w.YearEnded,
                    Industry = w.Industry,
                    Achievement = w.Achievement,
                    Duration =
                        $"{w.YearStarted} - {w.YearEnded} ({w.YearEnded - w.YearStarted} years)",
                    CreatedAt = w.CreatedAt,
                })
                .FirstOrDefaultAsync();

            if (workExperience == null)
            {
                return NotFound();
            }

            return Ok(workExperience);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<
            ActionResult<IEnumerable<WorkExperienceDto>>
        > GetWorkExperiencesByEmployee(string employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found");
            }

            var workExperiences = await _context
                .WorkExperiences.Include(w => w.Employee)
                .Where(w => w.EmployeeId == employeeId)
                .Select(w => new WorkExperienceDto
                {
                    Id = w.Id,
                    EmployeeId = w.EmployeeId,
                    EmployeeName = w.Employee.FullName,
                    JobTitle = w.JobTitle,
                    CompanyName = w.CompanyName,
                    YearStarted = w.YearStarted,
                    YearEnded = w.YearEnded,
                    Industry = w.Industry,
                    Achievement = w.Achievement,
                    Duration =
                        $"{w.YearStarted} - {w.YearEnded} ({w.YearEnded - w.YearStarted} years)",
                    CreatedAt = w.CreatedAt,
                })
                .OrderByDescending(w => w.YearEnded)
                .ToListAsync();

            return Ok(workExperiences);
        }

        [HttpGet("employee/{employeeId}/summary")]
        public async Task<ActionResult<WorkExperienceSummaryDto>> GetWorkExperienceSummary(
            string employeeId
        )
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found");
            }

            var workExperiences = await _context
                .WorkExperiences.Where(w => w.EmployeeId == employeeId)
                .ToListAsync();

            var summary = new WorkExperienceSummaryDto
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName,
                TotalExperiences = workExperiences.Count,
                TotalYearsOfExperience = workExperiences.Sum(w => w.YearEnded - w.YearStarted),
                Industries = workExperiences.Select(w => w.Industry).Distinct().ToList(),
                MostRecentJob = workExperiences
                    .OrderByDescending(w => w.YearEnded)
                    .FirstOrDefault()
                    ?.JobTitle,
                MostRecentCompany = workExperiences
                    .OrderByDescending(w => w.YearEnded)
                    .FirstOrDefault()
                    ?.CompanyName,
            };

            return Ok(summary);
        }

        [HttpPost("create")]
        public async Task<ActionResult<WorkExperienceDto>> CreateWorkExperience(
            CreateWorkExperienceDto createDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createDto.YearEnded <= createDto.YearStarted)
            {
                return BadRequest("Year ended must be greater than year started");
            }

            var employee = await _context.Employees.FindAsync(createDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest($"Employee with ID {createDto.EmployeeId} not found");
            }

            var workExperience = new WorkExperience
            {
                EmployeeId = createDto.EmployeeId,
                JobTitle = createDto.JobTitle,
                CompanyName = createDto.CompanyName,
                YearStarted = createDto.YearStarted,
                YearEnded = createDto.YearEnded,
                Industry = createDto.Industry,
                Achievement = createDto.Achievement,
                CreatedAt = DateTime.UtcNow,
            };

            _context.WorkExperiences.Add(workExperience);
            await _context.SaveChangesAsync();

            var result = await _context
                .WorkExperiences.Include(w => w.Employee)
                .Where(w => w.Id == workExperience.Id)
                .Select(w => new WorkExperienceDto
                {
                    Id = w.Id,
                    EmployeeId = w.EmployeeId,
                    EmployeeName = w.Employee.FullName,
                    JobTitle = w.JobTitle,
                    CompanyName = w.CompanyName,
                    YearStarted = w.YearStarted,
                    YearEnded = w.YearEnded,
                    Industry = w.Industry,
                    Achievement = w.Achievement,
                    Duration =
                        $"{w.YearStarted} - {w.YearEnded} ({w.YearEnded - w.YearStarted} years)",
                    CreatedAt = w.CreatedAt,
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(
                nameof(GetWorkExperience),
                new { id = workExperience.Id },
                result
            );
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<WorkExperienceDto>> UpdateWorkExperience(
            int id,
            UpdateWorkExperienceDto updateDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Custom validations
            if (updateDto.YearEnded <= updateDto.YearStarted)
            {
                return BadRequest("Year ended must be greater than year started");
            }

            var workExperience = await _context
                .WorkExperiences.Include(w => w.Employee)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workExperience == null)
            {
                return NotFound();
            }

            workExperience.JobTitle = updateDto.JobTitle;
            workExperience.CompanyName = updateDto.CompanyName;
            workExperience.YearStarted = updateDto.YearStarted;
            workExperience.YearEnded = updateDto.YearEnded;
            workExperience.Industry = updateDto.Industry;
            workExperience.Achievement = updateDto.Achievement;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkExperienceExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            var result = new WorkExperienceDto
            {
                Id = workExperience.Id,
                EmployeeId = workExperience.EmployeeId,
                EmployeeName = workExperience.Employee.FullName,
                JobTitle = workExperience.JobTitle,
                CompanyName = workExperience.CompanyName,
                YearStarted = workExperience.YearStarted,
                YearEnded = workExperience.YearEnded,
                Industry = workExperience.Industry,
                Achievement = workExperience.Achievement,
                Duration =
                    $"{workExperience.YearStarted} - {workExperience.YearEnded} ({workExperience.YearEnded - workExperience.YearStarted} years)",
                CreatedAt = workExperience.CreatedAt,
            };

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteWorkExperience(int id)
        {
            var workExperience = await _context.WorkExperiences.FindAsync(id);
            if (workExperience == null)
            {
                return NotFound();
            }

            _context.WorkExperiences.Remove(workExperience);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("employee/{employeeId}/delete-all")]
        public async Task<IActionResult> DeleteAllWorkExperiencesForEmployee(string employeeId)
        {
            var workExperiences = await _context
                .WorkExperiences.Where(w => w.EmployeeId == employeeId)
                .ToListAsync();

            if (!workExperiences.Any())
            {
                return NotFound($"No work experiences found for employee {employeeId}");
            }

            _context.WorkExperiences.RemoveRange(workExperiences);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkExperienceExists(int id)
        {
            return _context.WorkExperiences.Any(e => e.Id == id);
        }
    }
}
