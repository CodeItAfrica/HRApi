using System.ComponentModel.DataAnnotations;
using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get admin dashboard with system-wide statistics and summaries
    /// </summary>
    [HttpGet("admin")]
    public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard()
    {
        try
        {
            var currentDate = DateTime.UtcNow;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            var totalEmployees = await _context.Employees.CountAsync(e =>
                e.Active == true && e.Deleted != true
            );
            var newEmployeesThisMonth = await _context.Employees.CountAsync(e =>
                e.CreatedAt.Month == currentMonth && e.CreatedAt.Year == currentYear
            );

            var employeesByDepartment = await _context
                .Employees.Where(e => e.Active == true && e.Deleted != true)
                .GroupBy(e => e.Department!.DepartmentName)
                .Select(g => new DepartmentStatsDto
                {
                    DepartmentName = g.Key ?? "Unassigned",
                    EmployeeCount = g.Count(),
                })
                .OrderByDescending(d => d.EmployeeCount)
                .ToListAsync();

            var employeesByBranch = await _context
                .Employees.Where(e => e.Active == true && e.Deleted != true)
                .GroupBy(e => e.Branch!.BranchName)
                .Select(g => new BranchStatsDto
                {
                    BranchName = g.Key ?? "Unassigned",
                    EmployeeCount = g.Count(),
                })
                .OrderByDescending(b => b.EmployeeCount)
                .ToListAsync();

            var totalPayrollAmount = await _context
                .PayrollHistories.Where(p => p.Month == currentMonth && p.Year == currentYear)
                .SumAsync(p => p.NetSalary);

            var averageSalary =
                totalEmployees > 0 ? await _context.Payrolls.AverageAsync(p => p.NetSalary) : 0;

            var payrollStatusSummary = await _context
                .PayrollHistories.Where(p => p.Month == currentMonth && p.Year == currentYear)
                .GroupBy(p => p.PaymentStatus)
                .Select(g => new PayrollStatusDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(p => p.NetSalary),
                })
                .ToListAsync();

            var gradeDistribution = await _context
                .Employees.Where(e => e.Active == true && e.Deleted != true && e.Grade != null)
                .GroupBy(e => e.Grade!.GradeName)
                .Select(g => new GradeStatsDto
                {
                    GradeName = g.Key,
                    EmployeeCount = g.Count(),
                    AverageBaseSalary = g.Average(e => e.Grade!.BaseSalary),
                })
                .OrderBy(g => g.GradeName)
                .ToListAsync();

            var recentHires = await _context
                .Employees.Where(e => e.CreatedAt >= currentDate.AddDays(-30))
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .Select(e => new RecentActivityDto
                {
                    Type = "New Hire",
                    Description = $"{e.FullName} joined {e.Department!.DepartmentName}",
                    Date = e.CreatedAt,
                    EmployeeName = e.FullName,
                })
                .ToListAsync();

            var pendingConfirmations = await _context
                .Employees.Where(e =>
                    e.ConfirmStatus != true && e.Active == true && e.HireDate.HasValue
                )
                .CountAsync();

            var upcomingRetirements = await _context
                .Employees.Where(e =>
                    e.RetiredDate.HasValue
                    && e.RetiredDate.Value <= DateOnly.FromDateTime(currentDate.AddMonths(6))
                )
                .CountAsync();

            var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
            var totalBranches = await _context.Branches.CountAsync();
            var totalDepartments = await _context.Departments.CountAsync();

            var dashboard = new AdminDashboardDto
            {
                EmployeeStatistics = new EmployeeStatisticsDto
                {
                    TotalEmployees = totalEmployees,
                    NewEmployeesThisMonth = newEmployeesThisMonth,
                    PendingConfirmations = pendingConfirmations,
                    UpcomingRetirements = upcomingRetirements,
                    EmployeesByDepartment = employeesByDepartment,
                    EmployeesByBranch = employeesByBranch,
                },
                PayrollStatistics = new PayrollStatisticsDto
                {
                    TotalPayrollThisMonth = totalPayrollAmount,
                    AverageSalary = averageSalary,
                    PayrollStatusSummary = payrollStatusSummary,
                    GradeDistribution = gradeDistribution,
                },
                SystemMetrics = new SystemMetricsDto
                {
                    ActiveUsers = activeUsers,
                    TotalBranches = totalBranches,
                    TotalDepartments = totalDepartments,
                    LastUpdated = currentDate,
                },
                RecentActivities = recentHires,
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    message = "An error occurred while fetching admin dashboard data",
                    error = ex.Message,
                }
            );
        }
    }

    /// <summary>
    /// Get employee-specific dashboard data
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<EmployeeDashboardDto>> GetEmployeeDashboard(
        [Required] string employeeId
    )
    {
        try
        {
            var employee = await _context
                .Employees.Include(e => e.Department)
                .Include(e => e.Branch)
                .Include(e => e.Unit)
                .Include(e => e.Grade)
                .Include(e => e.EmploymentType)
                .Include(e => e.Payroll)
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId && e.Active == true && e.Deleted != true
                );

            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            var currentDate = DateTime.UtcNow;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            var personalInfo = new PersonalInfoDto
            {
                EmployeeId = employee.Id,
                StaffIdNo = employee.StaffIdNo,
                FullName = employee.FullName,
                Email = employee.Email,
                Department = employee.Department?.DepartmentName ?? "Not Assigned",
                Branch = employee.Branch?.BranchName ?? "Not Assigned",
                Unit = employee.Unit?.UnitName ?? "Not Assigned",
                Grade = employee.Grade?.GradeName ?? "Not Assigned",
                EmploymentType = employee.EmploymentType?.TypeName ?? "Not Assigned",
                HireDate = employee.HireDate,
                ConfirmationStatus = employee.ConfirmStatus == true ? "Confirmed" : "Pending",
            };

            var currentPayroll = new CurrentPayrollDto();
            if (employee.Payroll != null)
            {
                currentPayroll = new CurrentPayrollDto
                {
                    BaseSalary = employee.Payroll.BaseSalary,
                    HousingAllowance = employee.Payroll.HousingAllowance,
                    TransportAllowance = employee.Payroll.TransportAllowance,
                    TotalAllowances = employee.Payroll.TotalAllowances,
                    TotalDeductions = employee.Payroll.TotalDeductions,
                    GrossSalary = employee.Payroll.GrossSalary,
                    NetSalary = employee.Payroll.NetSalary,
                    LastUpdated = employee.Payroll.UpdatedAt ?? employee.Payroll.CreatedAt,
                };
            }

            var payrollHistory = await _context
                .PayrollHistories.Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .Take(6)
                .Select(p => new PayrollHistoryDto
                {
                    Month = p.Month,
                    Year = p.Year,
                    GrossSalary = p.GrossSalary,
                    NetSalary = p.NetSalary,
                    PaymentStatus = p.PaymentStatus,
                    PaidOn = p.PaidOn,
                })
                .ToListAsync();

            var currentAllowances = await _context
                .PayrollAllowances.Where(pa => pa.EmployeeId == employeeId)
                .Include(pa => pa.AllowanceList)
                .Select(pa => new AllowanceDto
                {
                    Name = pa.AllowanceList!.Name ?? "Custom Allowance",
                    Amount = pa.Amount,
                    Description = pa.Description,
                    LastGrantedOn = pa.LastGrantedOn,
                })
                .ToListAsync();

            var currentDeductions = await _context
                .PayrollDeductions.Where(pd => pd.EmployeeId == employeeId)
                .Include(pd => pd.DeductionList)
                .Select(pd => new DeductionDto
                {
                    Name = pd.DeductionList!.Name ?? "Custom Deduction",
                    Amount = pd.Amount,
                    Description = pd.Description,
                    LastDeductedOn = pd.LastDeductedOn,
                })
                .ToListAsync();

            var leaveRequestsSummary = await _context
                .LeaveRequests.Where(lr => lr.EmployeeId == employeeId)
                .GroupBy(lr => lr.Status)
                .Select(g => new LeaveRequestSummaryDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                })
                .ToListAsync();

            var performanceReviews = await _context
                .PerformanceReviews.Where(pr => pr.EmployeeId == employeeId)
                .OrderByDescending(pr => pr.CreatedAt)
                .Take(3)
                .Select(pr => new PerformanceReviewDto
                {
                    ReviewDate = pr.ReviewPeriod.ToDateTime(new TimeOnly(0, 0)),
                    OverallRating = pr.PerformanceScore,
                    ReviewerName = pr.Reviewer!.FullName,
                    Comments = pr.Feedback,
                })
                .ToListAsync();

            var dashboard = new EmployeeDashboardDto
            {
                PersonalInfo = personalInfo,
                CurrentPayroll = currentPayroll,
                PayrollHistory = payrollHistory,
                CurrentAllowances = currentAllowances,
                CurrentDeductions = currentDeductions,
                LeaveRequestsSummary = leaveRequestsSummary,
                PerformanceReviews = performanceReviews,
                LastUpdated = currentDate,
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    message = "An error occurred while fetching employee dashboard data",
                    error = ex.Message,
                }
            );
        }
    }
}
