using HRApi.Data;
using HRApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Services
{
    public class PayrollService
    {
        private readonly AppDbContext _context;

        public PayrollService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calculates the total allowances for an employee for the current month and year.
        /// This includes monthly allowances from PayrollAllowance table plus housing and transport allowances from Payroll table.
        /// </summary>
        /// <param name="employeeId">The employee ID</param>
        /// <returns>Total allowances amount</returns>
        public async Task<decimal> CalculateTotalAllowancesAsync(string employeeId)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentYear = currentDate.Year;
            var currentMonth = currentDate.Month;

            var monthlyAllowances = await _context
                .PayrollAllowances.Where(pa => pa.EmployeeId == employeeId)
                .SumAsync(pa => pa.Amount);

            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p =>
                p.EmployeeId == employeeId
            );

            decimal housingAllowance = payroll?.HousingAllowance ?? 0;
            decimal transportAllowance = payroll?.TransportAllowance ?? 0;

            decimal totalAllowances = monthlyAllowances + housingAllowance + transportAllowance;

            return totalAllowances;
        }

        /// <summary>
        /// Calculates the total deductions for an employee for the current month and year.
        /// This includes monthly deductions from PayrollDeduction table plus monthly tax (annual tax / 12) from Payroll table.
        /// </summary>
        /// <param name="employeeId">The employee ID</param>
        /// <returns>Total deductions amount</returns>
        public async Task<decimal> CalculateTotalDeductionsAsync(string employeeId)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentYear = currentDate.Year;
            var currentMonth = currentDate.Month;

            var monthlyDeductions = await _context
                .PayrollDeductions.Where(pd => pd.EmployeeId == employeeId)
                .SumAsync(pd => pd.Amount);

            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p =>
                p.EmployeeId == employeeId
            );

            decimal monthlyTax = payroll?.AnnualTax / 12 ?? 0;

            decimal totalDeductions = monthlyDeductions + monthlyTax;

            return totalDeductions;
        }

        public async Task CreatePayrollAllowancesForEmployee(string employeeId)
        {
            try
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);

                if (!employeeExists)
                {
                    throw new ArgumentException($"Employee with ID {employeeId} not found.");
                }

                var allowanceTypes = await _context.AllowanceLists.ToListAsync();

                var existingAllowances = await _context
                    .PayrollAllowances.Where(pa => pa.EmployeeId == employeeId)
                    .Select(pa => pa.AllowanceListId)
                    .ToListAsync();

                var newAllowances = new List<PayrollAllowance>();

                foreach (var allowanceType in allowanceTypes)
                {
                    if (existingAllowances.Contains(allowanceType.Id))
                        continue;

                    var payrollAllowance = new PayrollAllowance
                    {
                        EmployeeId = employeeId,
                        AllowanceListId = allowanceType.Id,
                        Amount = 0m,
                        Description = $"Initial allowance setup for {allowanceType.Name}",
                        LastGrantedBy = "System",
                        LastGrantedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = DateTime.UtcNow,
                    };

                    newAllowances.Add(payrollAllowance);
                }

                if (newAllowances.Any())
                {
                    await _context.PayrollAllowances.AddRangeAsync(newAllowances);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error creating payroll allowances: {ex.Message}",
                    ex
                );
            }
        }

        public async Task CreatePayrollDeductionsForEmployee(string employeeId)
        {
            try
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);

                if (!employeeExists)
                {
                    throw new ArgumentException($"Employee with ID {employeeId} not found.");
                }

                var deductionTypes = await _context.DeductionLists.ToListAsync();

                var existingDeductions = await _context
                    .PayrollDeductions.Where(pa => pa.EmployeeId == employeeId)
                    .Select(pa => pa.DeductionListId)
                    .ToListAsync();

                var newDeductions = new List<PayrollDeduction>();

                foreach (var deductionType in deductionTypes)
                {
                    if (existingDeductions.Contains(deductionType.Id))
                        continue;

                    var payrollDeduction = new PayrollDeduction
                    {
                        EmployeeId = employeeId,
                        DeductionListId = deductionType.Id,
                        Amount = 0m,
                        Description = $"Initial allowance setup for {deductionType.Name}",
                        LastDeductedBy = "System",
                        LastDeductedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = DateTime.UtcNow,
                    };

                    newDeductions.Add(payrollDeduction);
                }

                if (newDeductions.Any())
                {
                    await _context.PayrollDeductions.AddRangeAsync(newDeductions);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error creating payroll deductions: {ex.Message}",
                    ex
                );
            }
        }
    }
}
