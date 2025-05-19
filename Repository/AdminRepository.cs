using HRApi.Data;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;
using static HRApi.Models.Admin;

namespace HRApi.Repository
{
    public class AdminRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AdminRepository(AppDbContext context, IConfiguration config)
        {
        _context = context;
            _config = config;
        }


        public async Task<List<JobTitle>> GetJobTitlesAsync()
        {
            return await _context.JobTitles.ToListAsync();
        }

        public async Task<JobTitle> GetJobTitleAsync(int id)
        {
            return await _context.JobTitles.FindAsync(id);
        }

        public async Task AddJobTitleAsync(JobTitle jobTitle)
        {
            _context.JobTitles.Add(jobTitle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateJobTitleAsync(JobTitle jobTitle)
        {
            _context.Entry(jobTitle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteJobTitleAsync(int id)
        {
            var jobTitle = await _context.JobTitles.FindAsync(id);
            if (jobTitle != null)
            {
                _context.JobTitles.Remove(jobTitle);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Branch>> GetBranchesAsync()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<Branch> GetBranchAsync(int id)
        {
            return await _context.Branches.FindAsync(id);
        }

        public async Task AddBranchAsync(CreateBranchRequest branch)
        {
            var newBranch = new Branch
            {
                BranchName = branch.BranchName,
                Address = branch.Address,
                City = branch.City,
                State = branch.State,
                Country = branch.Country,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Branches.Add(newBranch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBranchAsync(CreateBranchRequest branch)
        {
            _context.Entry(branch).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBranchAsync(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
            }
        }
    }
}
