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

        public async Task<decimal> CalculateTotalVariantAllowanceAsync(int payrollHistoryId)
        {
            var totalVariantAllowances = await _context
                .VariantPayrollAllowances.Where(vpa => vpa.PayrollHistoryId == payrollHistoryId)
                .SumAsync(vpa => vpa.Amount);

            return totalVariantAllowances;
        }

        public async Task<decimal> CalculateTotalVariantDeductionAsync(int payrollHistoryId)
        {
            var totalVariantDeductions = await _context
                .VariantPayrollDeductions.Where(vpa => vpa.PayrollHistoryId == payrollHistoryId)
                .SumAsync(vpa => vpa.Amount);

            return totalVariantDeductions;
        }

        public async Task CreatePayrollAllowancesForEmployee(string employeeId)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);

                if (employee == null)
                {
                    throw new ArgumentException($"Employee with ID {employeeId} not found.");
                }

                if (!employee.GradeId.HasValue)
                {
                    throw new ArgumentException(
                        $"Employee with ID {employeeId} does not have a grade assigned."
                    );
                }

                var allowanceTypesForGrade = await _context
                    .GradeAllowances.Where(ga => ga.GradeId == employee.GradeId.Value)
                    .Include(ga => ga.AllowanceList)
                    .Select(ga => ga.AllowanceList)
                    .ToListAsync();

                var existingAllowances = await _context
                    .PayrollAllowances.Where(pa => pa.EmployeeId == employeeId)
                    .Select(pa => pa.AllowanceListId)
                    .ToListAsync();

                var newAllowances = new List<PayrollAllowance>();

                foreach (var allowanceType in allowanceTypesForGrade)
                {
                    if (existingAllowances.Contains(allowanceType.Id))
                        continue;

                    var payrollAllowance = new PayrollAllowance
                    {
                        EmployeeId = employeeId,
                        AllowanceListId = allowanceType.Id,
                        Amount = allowanceType.Amount,
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
                var employee = await _context.Employees.FindAsync(employeeId);

                if (employee == null)
                {
                    throw new ArgumentException($"Employee with ID {employeeId} not found.");
                }

                if (!employee.GradeId.HasValue)
                {
                    throw new ArgumentException(
                        $"Employee with ID {employeeId} does not have a grade assigned."
                    );
                }

                var deductionTypesForGrade = await _context
                    .GradeDeductions.Where(gd => gd.GradeId == employee.GradeId.Value)
                    .Include(gd => gd.DeductionList)
                    .Select(gd => gd.DeductionList)
                    .ToListAsync();

                var existingDeductions = await _context
                    .PayrollDeductions.Where(pa => pa.EmployeeId == employeeId)
                    .Select(pa => pa.DeductionListId)
                    .ToListAsync();

                var newDeductions = new List<PayrollDeduction>();

                foreach (var deductionType in deductionTypesForGrade)
                {
                    if (existingDeductions.Contains(deductionType.Id))
                        continue;

                    var payrollDeduction = new PayrollDeduction
                    {
                        EmployeeId = employeeId,
                        DeductionListId = deductionType.Id,
                        Amount = deductionType.Amount,
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

        public string BuildDescriptionUpdate(decimal oldAmount, decimal newAmount)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (newAmount > oldAmount)
            {
                var addedAmount = newAmount - oldAmount;
                return $"[{timestamp}] Added {addedAmount} to {oldAmount}";
            }
            else if (newAmount < oldAmount)
            {
                var removedAmount = oldAmount - newAmount;
                return $"[{timestamp}] Removed {removedAmount} from {oldAmount}";
            }
            else
            {
                return $"[{timestamp}] Amount unchanged at {oldAmount}";
            }
        }

        /// <summary>
        /// Checks if payroll has already been processed for an employee in a specific month/year
        /// </summary>
        /// <param name="employeeId">The employee ID</param>
        /// <param name="month">The month (1-12)</param>
        /// <param name="year">The year</param>
        /// <returns>True if payroll already exists for the month/year</returns>
        public async Task<bool> IsPayrollAlreadyProcessedAsync(
            string employeeId,
            int month,
            int year
        )
        {
            return await _context.PayrollHistories.AnyAsync(ph =>
                ph.EmployeeId == employeeId && ph.Month == month && ph.Year == year
            );
        }

        public async Task CreatePayrollAllowanceHistoryForEmployee(
            string employeeId,
            int month,
            int year
        )
        {
            try
            {
                var allowances = await _context
                    .Set<PayrollAllowance>()
                    .Include(pa => pa.AllowanceList)
                    .Where(pa => pa.EmployeeId == employeeId)
                    .ToListAsync();

                if (!allowances.Any())
                    return;

                var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                var historyRecords = allowances
                    .Select(allowance => new PayrollAllowanceHistory
                    {
                        EmployeeId = allowance.EmployeeId,
                        Month = month,
                        Year = year,
                        AllowanceName = allowance.AllowanceList?.Name ?? "Unknown",
                        Amount = allowance.Amount,
                        Description = allowance.Description,
                        LastModifiedOn = currentDate,
                        CreatedAt = DateTime.UtcNow,
                    })
                    .ToList();

                await _context.Set<PayrollAllowanceHistory>().AddRangeAsync(historyRecords);

                _context.Set<PayrollAllowance>().RemoveRange(allowances);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create payroll allowance history for employee {employeeId}: {ex.Message}",
                    ex
                );
            }
        }

        public async Task CreatePayrollDeductionHistoryForEmployee(
            string employeeId,
            int month,
            int year
        )
        {
            try
            {
                var payrollDeductions = await _context
                    .Set<PayrollDeduction>()
                    .Include(pd => pd.DeductionList)
                    .Where(pd => pd.EmployeeId == employeeId)
                    .ToListAsync();

                if (!payrollDeductions.Any())
                {
                    return;
                }

                var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                var historyRecords = payrollDeductions
                    .Select(deduction => new PayrollDeductionHistory
                    {
                        EmployeeId = deduction.EmployeeId,
                        Month = month,
                        Year = year,
                        DeductionName = deduction.DeductionList?.Name ?? "Unknown",
                        Amount = deduction.Amount,
                        Description = deduction.Description,
                        LastModifiedOn = currentDate,
                        CreatedAt = DateTime.UtcNow,
                    })
                    .ToList();

                await _context.Set<PayrollDeductionHistory>().AddRangeAsync(historyRecords);

                _context.Set<PayrollDeduction>().RemoveRange(payrollDeductions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create payroll deduction history for employee {employeeId}: {ex.Message}",
                    ex
                );
            }
        }
    }
}
