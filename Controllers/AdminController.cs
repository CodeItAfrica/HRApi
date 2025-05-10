using HRApi.Data;
using HRApi.Repository;
using HRApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HRApi.Models.Admin;

namespace HRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly AdminRepository _repository;
        private readonly IDService _idService;

        public AdminController(AppDbContext context, IConfiguration config, AdminRepository adminRepository, IDService idService)
        {
            _context = context;
            _config = config;
            _repository = adminRepository;
            _idService = idService;
        }


        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches()
        {
            return Ok(await _repository.GetBranchesAsync());
        }

        [HttpGet("branches/{id}")]
        public async Task<IActionResult> GetBranch(int id)
        {
            var branch = await _repository.GetBranchAsync(id);
            if (branch == null) return NotFound();
            return Ok(branch);
        }

        [HttpPost("branches")]
        public async Task<IActionResult> AddBranch([FromBody] Branch branch)
        {
            await _repository.AddBranchAsync(branch);
            return CreatedAtAction(nameof(GetBranch), new { id = branch.BranchID }, branch);
        }

        [HttpPut("branches/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] Branch branch)
        {
            if (id != branch.BranchID) return BadRequest();
            await _repository.UpdateBranchAsync(branch);
            return NoContent();
        }

        [HttpDelete("branches/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            await _repository.DeleteBranchAsync(id);
            return NoContent();
        }

    }
}
