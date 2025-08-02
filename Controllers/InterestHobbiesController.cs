using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Controllers
{
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
        public async Task<ActionResult<IEnumerable<InterestHobbiesDto>>> GetInterestHobbies()
        {
            var hobbies = await _context
                .InterestHobbies.Include(h => h.Employee)
                .Select(h => new InterestHobbiesDto
                {
                    Id = h.Id,
                    EmployeeId = h.EmployeeId,
                    EmployeeName = h.Employee.Surname + " " + h.Employee.OtherNames,
                    EmployeeStaffId = h.Employee.StaffIdNo,
                    Hobby = h.Hobby,
                    Description = h.Description,
                    CreatedAt = h.CreatedAt,
                })
                .OrderBy(h => h.EmployeeName)
                .ThenBy(h => h.Hobby)
                .ToListAsync();

            return Ok(hobbies);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<InterestHobbiesDto>> GetInterestHobby(int id)
        {
            var hobby = await _context
                .InterestHobbies.Include(h => h.Employee)
                .Where(h => h.Id == id)
                .Select(h => new InterestHobbiesDto
                {
                    Id = h.Id,
                    EmployeeId = h.EmployeeId,
                    EmployeeName = h.Employee.Surname + " " + h.Employee.OtherNames,
                    EmployeeStaffId = h.Employee.StaffIdNo,
                    Hobby = h.Hobby,
                    Description = h.Description,
                    CreatedAt = h.CreatedAt,
                })
                .FirstOrDefaultAsync();

            if (hobby == null)
            {
                return NotFound();
            }

            return Ok(hobby);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<InterestHobbiesDto>>> GetHobbiesByEmployee(
            string employeeId
        )
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found");
            }

            var hobbies = await _context
                .InterestHobbies.Include(h => h.Employee)
                .Where(h => h.EmployeeId == employeeId)
                .Select(h => new InterestHobbiesDto
                {
                    Id = h.Id,
                    EmployeeId = h.EmployeeId,
                    EmployeeName = h.Employee.Surname + " " + h.Employee.OtherNames,
                    EmployeeStaffId = h.Employee.StaffIdNo,
                    Hobby = h.Hobby,
                    Description = h.Description,
                    CreatedAt = h.CreatedAt,
                })
                .OrderBy(h => h.Hobby)
                .ToListAsync();

            return Ok(hobbies);
        }

        [HttpGet("employee/{employeeId}/summary")]
        public async Task<ActionResult<HobbiesSummaryDto>> GetHobbiesSummary(string employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found");
            }

            var hobbies = await _context
                .InterestHobbies.Where(h => h.EmployeeId == employeeId)
                .ToListAsync();

            var summary = new HobbiesSummaryDto
            {
                EmployeeId = employeeId,
                EmployeeName = employee.Surname + " " + employee.OtherNames,
                EmployeeStaffId = employee.StaffIdNo,
                TotalHobbies = hobbies.Count,
                HobbiesList = hobbies.Select(h => h.Hobby).ToList(),
                HobbiesWithDescription = hobbies.Count(h => !string.IsNullOrEmpty(h.Description)),
                MostRecentHobby = hobbies
                    .OrderByDescending(h => h.CreatedAt)
                    .FirstOrDefault()
                    ?.Hobby,
                OldestHobby = hobbies.OrderBy(h => h.CreatedAt).FirstOrDefault()?.Hobby,
            };

            return Ok(summary);
        }

        // [HttpGet("statistics")]
        // public async Task<ActionResult<HobbiesStatisticsDto>> GetHobbiesStatistics()
        // {
        //     var hobbies = await _context.InterestHobbies.Include(h => h.Employee).ToListAsync();

        //     var statistics = new HobbiesStatisticsDto
        //     {
        //         TotalHobbiesRecorded = hobbies.Count,
        //         UniqueHobbies = hobbies.Select(h => h.Hobby.ToLower()).Distinct().Count(),
        //         EmployeesWithHobbies = hobbies.Select(h => h.EmployeeId).Distinct().Count(),
        //         AverageHobbiesPerEmployee = await _context
        //             .InterestHobbies.GroupBy(h => h.EmployeeId)
        //             .Select(g => g.Count())
        //             .DefaultIfEmpty(0)
        //             .AverageAsync(),
        //         MostPopularHobbies = hobbies
        //             .GroupBy(h => h.Hobby.ToLower())
        //             .OrderByDescending(g => g.Count())
        //             .Take(10)
        //             .Select(g => new HobbyStatistic
        //             {
        //                 Hobby = g.First().Hobby,
        //                 Count = g.Count(),
        //                 Percentage = Math.Round((double)g.Count() / hobbies.Count * 100, 2),
        //             })
        //             .ToList(),
        //         HobbiesWithDescription = hobbies.Count(h => !string.IsNullOrEmpty(h.Description)),
        //         PercentageWithDescription = hobbies.Any()
        //             ? Math.Round(
        //                 (double)hobbies.Count(h => !string.IsNullOrEmpty(h.Description))
        //                     / hobbies.Count
        //                     * 100,
        //                 2
        //             )
        //             : 0,
        //     };

        //     return Ok(statistics);
        // }

        [HttpGet("search/{hobby}")]
        public async Task<ActionResult<IEnumerable<InterestHobbiesDto>>> SearchByHobby(string hobby)
        {
            var hobbies = await _context
                .InterestHobbies.Include(h => h.Employee)
                .Where(h => h.Hobby.ToLower().Contains(hobby.ToLower()))
                .Select(h => new InterestHobbiesDto
                {
                    Id = h.Id,
                    EmployeeId = h.EmployeeId,
                    EmployeeName = h.Employee.Surname + " " + h.Employee.OtherNames,
                    EmployeeStaffId = h.Employee.StaffIdNo,
                    Hobby = h.Hobby,
                    Description = h.Description,
                    CreatedAt = h.CreatedAt,
                })
                .OrderBy(h => h.EmployeeName)
                .ToListAsync();

            return Ok(hobbies);
        }

        [HttpGet("common")]
        public async Task<ActionResult<IEnumerable<CommonHobbyDto>>> GetCommonHobbies()
        {
            var commonHobbies = await _context
                .InterestHobbies.GroupBy(h => h.Hobby.ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => new CommonHobbyDto
                {
                    Hobby = g.First().Hobby,
                    Count = g.Count(),
                    Employees = g.Select(h => new EmployeeBasicDto
                        {
                            EmployeeId = h.EmployeeId,
                            EmployeeName = h.Employee.Surname + " " + h.Employee.OtherNames,
                            StaffId = h.Employee.StaffIdNo,
                        })
                        .ToList(),
                })
                .OrderByDescending(ch => ch.Count)
                .ToListAsync();

            return Ok(commonHobbies);
        }

        [HttpPost("create")]
        public async Task<ActionResult<InterestHobbiesDto>> CreateInterestHobby(
            CreateInterestHobbiesDto createDto
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

            var existingHobby = await _context.InterestHobbies.FirstOrDefaultAsync(h =>
                h.EmployeeId == createDto.EmployeeId
                && h.Hobby.ToLower() == createDto.Hobby.ToLower()
            );

            if (existingHobby != null)
            {
                return BadRequest($"Hobby '{createDto.Hobby}' already exists for this employee");
            }

            var hobby = new InterestHobbies
            {
                EmployeeId = createDto.EmployeeId,
                Hobby = createDto.Hobby.Trim(),
                Description = createDto.Description?.Trim(),
                CreatedAt = DateTime.UtcNow,
            };

            _context.InterestHobbies.Add(hobby);
            await _context.SaveChangesAsync();

            var result = await _context
                .InterestHobbies.Include(h => h.Employee)
                .Where(h => h.Id == hobby.Id)
                .Select(h => new InterestHobbiesDto
                {
                    Id = h.Id,
                    EmployeeId = h.EmployeeId,
                    EmployeeName = h.Employee.Surname + " " + h.Employee.OtherNames,
                    EmployeeStaffId = h.Employee.StaffIdNo,
                    Hobby = h.Hobby,
                    Description = h.Description,
                    CreatedAt = h.CreatedAt,
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetInterestHobby), new { id = hobby.Id }, result);
        }

        [HttpPost("create-bulk")]
        public async Task<ActionResult<IEnumerable<InterestHobbiesDto>>> CreateMultipleHobbies(
            CreateMultipleHobbiesDto createDto
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

            var existingHobbies = await _context
                .InterestHobbies.Where(h => h.EmployeeId == createDto.EmployeeId)
                .Select(h => h.Hobby.ToLower())
                .ToListAsync();

            var newHobbies = new List<InterestHobbies>();
            var duplicates = new List<string>();

            foreach (var hobbyDto in createDto.Hobbies)
            {
                if (existingHobbies.Contains(hobbyDto.Hobby.ToLower()))
                {
                    duplicates.Add(hobbyDto.Hobby);
                    continue;
                }

                newHobbies.Add(
                    new InterestHobbies
                    {
                        EmployeeId = createDto.EmployeeId,
                        Hobby = hobbyDto.Hobby.Trim(),
                        Description = hobbyDto.Description?.Trim(),
                        CreatedAt = DateTime.UtcNow,
                    }
                );
            }

            if (duplicates.Any())
            {
                return BadRequest(
                    $"The following hobbies already exist for this employee: {string.Join(", ", duplicates)}"
                );
            }

            if (!newHobbies.Any())
            {
                return BadRequest("No new hobbies to add");
            }

            _context.InterestHobbies.AddRange(newHobbies);
            await _context.SaveChangesAsync();

            var results = await _context
                .InterestHobbies.Include(h => h.Employee)
                .Where(h => newHobbies.Select(nh => nh.Id).Contains(h.Id))
                .Select(h => new InterestHobbiesDto
                {
                    Id = h.Id,
                    EmployeeId = h.EmployeeId,
                    EmployeeName = h.Employee.Surname + " " + h.Employee.OtherNames,
                    EmployeeStaffId = h.Employee.StaffIdNo,
                    Hobby = h.Hobby,
                    Description = h.Description,
                    CreatedAt = h.CreatedAt,
                })
                .ToListAsync();

            return Ok(results);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<InterestHobbiesDto>> UpdateInterestHobby(
            int id,
            UpdateInterestHobbiesDto updateDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hobby = await _context
                .InterestHobbies.Include(h => h.Employee)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hobby == null)
            {
                return NotFound();
            }

            var duplicateHobby = await _context.InterestHobbies.FirstOrDefaultAsync(h =>
                h.EmployeeId == hobby.EmployeeId
                && h.Hobby.ToLower() == updateDto.Hobby.ToLower()
                && h.Id != id
            );

            if (duplicateHobby != null)
            {
                return BadRequest($"Hobby '{updateDto.Hobby}' already exists for this employee");
            }

            hobby.Hobby = updateDto.Hobby.Trim();
            hobby.Description = updateDto.Description?.Trim();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InterestHobbyExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            var result = new InterestHobbiesDto
            {
                Id = hobby.Id,
                EmployeeId = hobby.EmployeeId,
                EmployeeName = hobby.Employee.Surname + " " + hobby.Employee.OtherNames,
                EmployeeStaffId = hobby.Employee.StaffIdNo,
                Hobby = hobby.Hobby,
                Description = hobby.Description,
                CreatedAt = hobby.CreatedAt,
            };

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteInterestHobby(int id)
        {
            var hobby = await _context.InterestHobbies.FindAsync(id);
            if (hobby == null)
            {
                return NotFound();
            }

            _context.InterestHobbies.Remove(hobby);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("employee/{employeeId}/delete-all")]
        public async Task<IActionResult> DeleteAllHobbiesForEmployee(string employeeId)
        {
            var hobbies = await _context
                .InterestHobbies.Where(h => h.EmployeeId == employeeId)
                .ToListAsync();

            if (!hobbies.Any())
            {
                return NotFound($"No hobbies found for employee {employeeId}");
            }

            _context.InterestHobbies.RemoveRange(hobbies);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InterestHobbyExists(int id)
        {
            return _context.InterestHobbies.Any(e => e.Id == id);
        }
    }
}
