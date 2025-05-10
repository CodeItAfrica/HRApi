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
            var branches = await _context.Branches.Select(u => new { u.Id, u.BranchName, u.Address, u.City, u.State, u.Country }).ToListAsync();
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

    }
}
