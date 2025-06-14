using HRApi.Data;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Services
{
    public class IDService
    {
        private readonly AppDbContext _context;

        public IDService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateUniqueStaffIdAsync()
        {
            string newStaffId;
            bool exists;

            do
            {
                newStaffId = $"STAFF-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                exists = await _context.Employees.AnyAsync(e => e.StaffIdNo == newStaffId);
            } while (exists);

            return newStaffId;
        }

        public async Task<string> GenerateUniqueEmployeeIdAsync()
        {
            string newEmployeeId;
            bool exists;

            do
            {
                newEmployeeId = $"EMPLOYEE-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                exists = await _context.Users.AnyAsync(e => e.EmployeeId == newEmployeeId);
            } while (exists);

            return newEmployeeId;
        }
    }
}
