using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffCompensationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffCompensationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<StaffCompensationDto>>> GetStaffCompensations()
        {
            var compensations = await _context
                .StaffCompensations.Include(s => s.Employee)
                .Select(s => new StaffCompensationDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee.FullName,
                    EmployeeStaffId = s.Employee.StaffIdNo,
                    IncidentDate = s.IncidentDate,
                    NotifyDate = s.NotifyDate,
                    InjuryDetails = s.InjuryDetails,
                    LocationDetails = s.LocationDetails,
                    DaysAway = s.DaysAway,
                    MedicalCost = s.MedicalCost,
                    OtherCost = s.OtherCost,
                    TotalCost = s.MedicalCost + s.OtherCost,
                    NotificationDelay =
                        s.IncidentDate.HasValue && s.NotifyDate.HasValue
                            ? s.NotifyDate.Value.DayNumber - s.IncidentDate.Value.DayNumber
                            : (int?)null,
                })
                .OrderByDescending(s => s.IncidentDate)
                .ToListAsync();

            return Ok(compensations);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<StaffCompensationDto>> GetStaffCompensation(int id)
        {
            var compensation = await _context
                .StaffCompensations.Include(s => s.Employee)
                .Where(s => s.Id == id)
                .Select(s => new StaffCompensationDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee.FullName,
                    EmployeeStaffId = s.Employee.StaffIdNo,
                    IncidentDate = s.IncidentDate,
                    NotifyDate = s.NotifyDate,
                    InjuryDetails = s.InjuryDetails,
                    LocationDetails = s.LocationDetails,
                    DaysAway = s.DaysAway,
                    MedicalCost = s.MedicalCost,
                    OtherCost = s.OtherCost,
                    TotalCost = s.MedicalCost + s.OtherCost,
                    NotificationDelay =
                        s.IncidentDate.HasValue && s.NotifyDate.HasValue
                            ? s.NotifyDate.Value.DayNumber - s.IncidentDate.Value.DayNumber
                            : (int?)null,
                })
                .FirstOrDefaultAsync();

            if (compensation == null)
            {
                return NotFound();
            }

            return Ok(compensation);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<
            ActionResult<IEnumerable<StaffCompensationDto>>
        > GetStaffCompensationsByEmployee(string employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found");
            }

            var compensations = await _context
                .StaffCompensations.Include(s => s.Employee)
                .Where(s => s.EmployeeId == employeeId)
                .Select(s => new StaffCompensationDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee.FullName,
                    EmployeeStaffId = s.Employee.StaffIdNo,
                    IncidentDate = s.IncidentDate,
                    NotifyDate = s.NotifyDate,
                    InjuryDetails = s.InjuryDetails,
                    LocationDetails = s.LocationDetails,
                    DaysAway = s.DaysAway,
                    MedicalCost = s.MedicalCost,
                    OtherCost = s.OtherCost,
                    TotalCost = s.MedicalCost + s.OtherCost,
                    NotificationDelay =
                        s.IncidentDate.HasValue && s.NotifyDate.HasValue
                            ? s.NotifyDate.Value.DayNumber - s.IncidentDate.Value.DayNumber
                            : (int?)null,
                })
                .OrderByDescending(s => s.IncidentDate)
                .ToListAsync();

            return Ok(compensations);
        }

        [HttpGet("employee/{employeeId}/summary")]
        public async Task<ActionResult<StaffCompensationSummaryDto>> GetStaffCompensationSummary(
            string employeeId
        )
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found");
            }

            var compensations = await _context
                .StaffCompensations.Where(s => s.EmployeeId == employeeId)
                .ToListAsync();

            var summary = new StaffCompensationSummaryDto
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName,
                EmployeeStaffId = employee.StaffIdNo,
                TotalIncidents = compensations.Count,
                TotalDaysAway = compensations.Sum(c => c.DaysAway),
                TotalMedicalCost = compensations.Sum(c => c.MedicalCost),
                TotalOtherCost = compensations.Sum(c => c.OtherCost),
                TotalCompensationCost = compensations.Sum(c => c.MedicalCost + c.OtherCost),
                MostRecentIncidentDate = compensations
                    .OrderByDescending(c => c.IncidentDate)
                    .FirstOrDefault()
                    ?.IncidentDate,
                AverageDaysAway = compensations.Any()
                    ? Math.Round(compensations.Average(c => c.DaysAway), 2)
                    : 0,
                AverageNotificationDelay = compensations
                    .Where(c => c.IncidentDate.HasValue && c.NotifyDate.HasValue)
                    .Select(c => c.NotifyDate!.Value.DayNumber - c.IncidentDate!.Value.DayNumber)
                    .DefaultIfEmpty(0)
                    .Average(),
            };

            return Ok(summary);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<CompensationStatisticsDto>> GetCompensationStatistics()
        {
            var compensations = await _context
                .StaffCompensations.Include(s => s.Employee)
                .ToListAsync();

            var statistics = new CompensationStatisticsDto
            {
                TotalIncidents = compensations.Count,
                TotalEmployeesAffected = compensations.Select(c => c.EmployeeId).Distinct().Count(),
                TotalDaysAway = compensations.Sum(c => c.DaysAway),
                TotalMedicalCost = compensations.Sum(c => c.MedicalCost),
                TotalOtherCost = compensations.Sum(c => c.OtherCost),
                TotalCompensationCost = compensations.Sum(c => c.MedicalCost + c.OtherCost),
                AverageDaysAway = compensations.Any()
                    ? Math.Round(compensations.Average(c => c.DaysAway), 2)
                    : 0,
                AverageMedicalCost = compensations.Any()
                    ? Math.Round(compensations.Average(c => c.MedicalCost), 2)
                    : 0,
                AverageNotificationDelay = compensations
                    .Where(c => c.IncidentDate.HasValue && c.NotifyDate.HasValue)
                    .Select(c => c.NotifyDate!.Value.DayNumber - c.IncidentDate!.Value.DayNumber)
                    .DefaultIfEmpty(0)
                    .Average(),
                MostCommonLocations = compensations
                    .Where(c => !string.IsNullOrEmpty(c.LocationDetails))
                    .GroupBy(c => c.LocationDetails)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new LocationStatistic { Location = g.Key!, Count = g.Count() })
                    .ToList(),
            };

            return Ok(statistics);
        }

        [HttpPost("create")]
        public async Task<ActionResult<StaffCompensationDto>> CreateStaffCompensation(
            CreateStaffCompensationDto createDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (
                createDto.IncidentDate.HasValue
                && createDto.NotifyDate.HasValue
                && createDto.NotifyDate < createDto.IncidentDate
            )
            {
                return BadRequest("Notification date cannot be before incident date");
            }

            if (createDto.MedicalCost < 0)
            {
                return BadRequest("Medical cost cannot be negative");
            }

            if (createDto.OtherCost < 0)
            {
                return BadRequest("Other cost cannot be negative");
            }

            if (createDto.DaysAway < 0)
            {
                return BadRequest("Days away cannot be negative");
            }

            var employee = await _context.Employees.FindAsync(createDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest($"Employee with ID {createDto.EmployeeId} not found");
            }

            var compensation = new StaffCompensation
            {
                EmployeeId = createDto.EmployeeId,
                IncidentDate = createDto.IncidentDate,
                NotifyDate = createDto.NotifyDate,
                InjuryDetails = createDto.InjuryDetails,
                LocationDetails = createDto.LocationDetails,
                DaysAway = createDto.DaysAway,
                MedicalCost = createDto.MedicalCost,
                OtherCost = createDto.OtherCost,
            };

            _context.StaffCompensations.Add(compensation);
            await _context.SaveChangesAsync();

            var result = await _context
                .StaffCompensations.Include(s => s.Employee)
                .Where(s => s.Id == compensation.Id)
                .Select(s => new StaffCompensationDto
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee.FullName,
                    EmployeeStaffId = s.Employee.StaffIdNo,
                    IncidentDate = s.IncidentDate,
                    NotifyDate = s.NotifyDate,
                    InjuryDetails = s.InjuryDetails,
                    LocationDetails = s.LocationDetails,
                    DaysAway = s.DaysAway,
                    MedicalCost = s.MedicalCost,
                    OtherCost = s.OtherCost,
                    TotalCost = s.MedicalCost + s.OtherCost,
                    NotificationDelay =
                        s.IncidentDate.HasValue && s.NotifyDate.HasValue
                            ? s.NotifyDate.Value.DayNumber - s.IncidentDate.Value.DayNumber
                            : (int?)null,
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(
                nameof(GetStaffCompensation),
                new { id = compensation.Id },
                result
            );
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<StaffCompensationDto>> UpdateStaffCompensation(
            int id,
            UpdateStaffCompensationDto updateDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (
                updateDto.IncidentDate.HasValue
                && updateDto.NotifyDate.HasValue
                && updateDto.NotifyDate < updateDto.IncidentDate
            )
            {
                return BadRequest("Notification date cannot be before incident date");
            }

            if (updateDto.MedicalCost < 0)
            {
                return BadRequest("Medical cost cannot be negative");
            }

            if (updateDto.OtherCost < 0)
            {
                return BadRequest("Other cost cannot be negative");
            }

            if (updateDto.DaysAway < 0)
            {
                return BadRequest("Days away cannot be negative");
            }

            var compensation = await _context
                .StaffCompensations.Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (compensation == null)
            {
                return NotFound();
            }

            compensation.IncidentDate = updateDto.IncidentDate;
            compensation.NotifyDate = updateDto.NotifyDate;
            compensation.InjuryDetails = updateDto.InjuryDetails;
            compensation.LocationDetails = updateDto.LocationDetails;
            compensation.DaysAway = updateDto.DaysAway;
            compensation.MedicalCost = updateDto.MedicalCost;
            compensation.OtherCost = updateDto.OtherCost;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffCompensationExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            var result = new StaffCompensationDto
            {
                Id = compensation.Id,
                EmployeeId = compensation.EmployeeId,
                EmployeeName = compensation.Employee.FullName,
                EmployeeStaffId = compensation.Employee.StaffIdNo,
                IncidentDate = compensation.IncidentDate,
                NotifyDate = compensation.NotifyDate,
                InjuryDetails = compensation.InjuryDetails,
                LocationDetails = compensation.LocationDetails,
                DaysAway = compensation.DaysAway,
                MedicalCost = compensation.MedicalCost,
                OtherCost = compensation.OtherCost,
                TotalCost = compensation.MedicalCost + compensation.OtherCost,
                NotificationDelay =
                    compensation.IncidentDate.HasValue && compensation.NotifyDate.HasValue
                        ? compensation.NotifyDate.Value.DayNumber
                            - compensation.IncidentDate.Value.DayNumber
                        : (int?)null,
            };

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteStaffCompensation(int id)
        {
            var compensation = await _context.StaffCompensations.FindAsync(id);
            if (compensation == null)
            {
                return NotFound();
            }

            _context.StaffCompensations.Remove(compensation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StaffCompensationExists(int id)
        {
            return _context.StaffCompensations.Any(e => e.Id == id);
        }
    }
}
