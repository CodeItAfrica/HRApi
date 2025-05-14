using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IDService _idService;

        public AdminController(AppDbContext context, IConfiguration config, IDService idService)
        {
            _context = context;
            _config = config;
            _idService = idService;
        }

        [HttpGet("get-branches")]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _context
                .Branches.Select(u => new
                {
                    u.Id,
                    u.BranchName,
                    u.Address,
                    u.City,
                    u.State,
                    u.Country,
                })
                .ToListAsync();
            return Ok(branches);
        }

        [HttpGet("branches/{id}")]
        public async Task<IActionResult> GetBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound($"No branch found with ID {id}");
            }

            return Ok(branch);
        }

        [HttpPost("branches-create")]
        public async Task<IActionResult> AddBranch([FromBody] CreateBranchRequest request)
        {
            var newBranch = new Branch
            {
                BranchName = request.BranchName,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                CreatedAt = DateTime.UtcNow
            };

            _context.Branches.Add(newBranch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBranches), new { id = newBranch.Id }, newBranch);
        }

        [HttpPut("branches/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] CreateBranchRequest branch)
        {
            var branch1 = await _context.Branches.FindAsync(id);
            if (branch1 == null)
            {
                return NotFound($"No branch found with ID {id}");
            }

            branch1.BranchName = branch.BranchName;
            branch1.Address = branch.Address;
            branch1.City = branch.City;
            branch1.State = branch.State;
            branch1.Country = branch.Country;

            _context.Entry(branch1).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("branches/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound($"No branch found with ID {id}");
            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("get-training-program")]
        public async Task<IActionResult> GetTrainingPrograms()
        {
            var trainingPrograms = await _context.TrainingPrograms.ToListAsync();
            return Ok(trainingPrograms);
        }

        [HttpGet("training-program/{id}")]
        public async Task<IActionResult> GetTrainingProgram(int id)
        {
            var trainingProgram = await _context.TrainingPrograms.FindAsync(id);
            if (trainingProgram == null)
            {
                return NotFound($"No training program found with ID {id}");
            }

            return Ok(trainingProgram);
        }

        [HttpPost("training-program/create")]
        public async Task<IActionResult> AddTrainingProgram([FromBody] CreateTrainingProgramRequest request)
        {
            var trainingProgram = new Admin.TrainingProgram
            {
                Title = request.Title,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };

            _context.TrainingPrograms.Add(trainingProgram);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTrainingProgram),
                new { id = trainingProgram.TrainingID },
                trainingProgram
            );
        }

        [HttpPut("training-program/{id}")]
        public async Task<IActionResult> UpdateTrainingProgram(int id, [FromBody] CreateTrainingProgramRequest request)
        {
            var trainingProgram = await _context.TrainingPrograms.FindAsync(id);
            if (trainingProgram == null)
            {
                return NotFound($"No training program found with ID {id}");
            }

            trainingProgram.Title = request.Title;
            trainingProgram.Description = request.Description;
            trainingProgram.StartDate = request.StartDate;
            trainingProgram.EndDate = request.EndDate;

            _context.Entry(trainingProgram).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(trainingProgram);
        }

        [HttpDelete("training-program/{id}")]
        public async Task<IActionResult> DeleteTrainingProgram(int id)
        {
            var trainingProgram = await _context.TrainingPrograms.FindAsync(id);
            if (trainingProgram == null)
            {
                return NotFound($"No training program found with ID {id}");
            }
            _context.TrainingPrograms.Remove(trainingProgram);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
