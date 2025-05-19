using HRApi.Data;
using HRApi.Models;
using HRApi.Repository;
using HRApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static HRApi.Models.Admin;

namespace HRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AdminRepository _adminRepository;

        private readonly IConfiguration _config;
        private readonly IDService _idService;


        public AdminController(AppDbContext context, IConfiguration config, AdminRepository adminRepository, IDService idService)
        {
            _context = context;
            _config = config;
            _adminRepository = adminRepository;
            _idService = idService;
        }
        [HttpGet("jobtitles")]
        public async Task<IActionResult> GetJobTitles()
        {
            return Ok(await _adminRepository.GetJobTitlesAsync());
        }

        [HttpGet("jobtitles/{id}")]
        public async Task<IActionResult> GetJobTitle(int id)
        {
            var jobTitle = await _adminRepository.GetJobTitleAsync(id);
            if (jobTitle == null) return NotFound();
            return Ok(jobTitle);
        }

        [HttpPost("jobtitles")]
        public async Task<IActionResult> AddJobTitle([FromBody] JobTitleDto dto)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == dto.DepartmentName);
            if (department == null) return BadRequest("Department not found");

            var jobTitle = new JobTitle
            {
                TitleName = dto.TitleName,
                DepartmentID = department.Id,
                CreatedAt = DateTime.Now
            };

            await _adminRepository.AddJobTitleAsync(jobTitle);
            return CreatedAtAction(nameof(GetJobTitle), new { id = jobTitle.JobTitleID }, jobTitle);
        }

        [HttpPut("jobtitles/{id}")]
        public async Task<IActionResult> UpdateJobTitle(int id, [FromBody] JobTitleDto dto)
        {
            var jobTitle = await _adminRepository.GetJobTitleAsync(id);
            if (jobTitle == null) return NotFound();

            var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == dto.DepartmentName);
            if (department == null) return BadRequest("Department not found");

            jobTitle.TitleName = dto.TitleName;
            jobTitle.DepartmentID = department.Id;

            await _adminRepository.UpdateJobTitleAsync(jobTitle);
            return NoContent();
        }

        [HttpDelete("jobtitles/{id}")]
        public async Task<IActionResult> DeleteJobTitle(int id)
        {
            await _adminRepository.DeleteJobTitleAsync(id);
            return NoContent();
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

        // Training Program Controllers and Routes

        [HttpGet("get-training-programs")]
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

        // Bank Controllers and Routes

        [HttpGet("get-banks")]
        public async Task<IActionResult> GetBanks()
        {
            var banks = await _context.Banks.ToListAsync();
            return Ok(banks);
        }

        [HttpGet("bank/{id}")]
        public async Task<IActionResult> GetBank(int id)
        {
            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound($"No bank found with ID {id}");
            }

            return Ok(bank);
        }

        [HttpPost("bank/create")]
        public async Task<IActionResult> AddBank([FromBody] CreateBankRequest request)
        {
            var bank = new Admin.Bank { BankName = request.BankName };

            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBank), new { id = bank.BankID }, bank);
        }

        [HttpPut("bank/{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] CreateBankRequest request)
        {
            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound($"No bank found with ID {id}");
            }

            bank.BankName = request.BankName;

            _context.Entry(bank).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(bank);
        }

        [HttpDelete("bank/{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound($"No bank found with ID {id}");
            }
            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // FAQ routes

        [HttpGet("get-faqs")]
        public async Task<IActionResult> GetFAQs()
        {
            var faqs = await _context.FAQs.ToListAsync();
            return Ok(faqs);
        }

        [HttpGet("faq/{id}")]
        public async Task<IActionResult> GetFAQ(int id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound($"No FAQ found with ID {id}");
            }

            return Ok(faq);
        }

        [HttpPost("faq/create")]
        public async Task<IActionResult> AddFAQ([FromBody] CreateFAQRequest request)
        {
            var faq = new Admin.FAQ { Question = request.Question, Answer = request.Answer };

            _context.FAQs.Add(faq);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFAQ), new { id = faq.FAQID }, faq);
        }

        [HttpPut("faq/{id}")]
        public async Task<IActionResult> UpdateFAQ(int id, [FromBody] CreateFAQRequest request)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound($"No FAQ found with ID {id}");
            }

            faq.Question = request.Question;
            faq.Answer = request.Answer;

            _context.Entry(faq).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(faq);
        }

        [HttpDelete("faq/{id}")]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound($"No FAQ found with ID {id}");
            }
            _context.FAQs.Remove(faq);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
