using System.ComponentModel.DataAnnotations;
using HRApi.Models;

public class AdminDashboardDto
{
    public EmployeeStatisticsDto EmployeeStatistics { get; set; } = new();
    public PayrollStatisticsDto PayrollStatistics { get; set; } = new();
    public SystemMetricsDto SystemMetrics { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class EmployeeStatisticsDto
{
    public int TotalEmployees { get; set; }
    public int NewEmployeesThisMonth { get; set; }
    public int PendingConfirmations { get; set; }
    public int UpcomingRetirements { get; set; }
    public List<DepartmentStatsDto> EmployeesByDepartment { get; set; } = new();
    public List<BranchStatsDto> EmployeesByBranch { get; set; } = new();
}

public class PayrollStatisticsDto
{
    public decimal TotalPayrollThisMonth { get; set; }
    public decimal AverageSalary { get; set; }
    public List<PayrollStatusDto> PayrollStatusSummary { get; set; } = new();
    public List<GradeStatsDto> GradeDistribution { get; set; } = new();
}

public class SystemMetricsDto
{
    public int ActiveUsers { get; set; }
    public int TotalBranches { get; set; }
    public int TotalDepartments { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class DepartmentStatsDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class BranchStatsDto
{
    public string BranchName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class PayrollStatusDto
{
    public PayrollHistoryStatus? Status { get; set; }
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class GradeStatsDto
{
    public string GradeName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public decimal AverageBaseSalary { get; set; }
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
}

// DTOs for Employee Dashboard
public class EmployeeDashboardDto
{
    public PersonalInfoDto PersonalInfo { get; set; } = new();
    public CurrentPayrollDto CurrentPayroll { get; set; } = new();
    public List<PayrollHistoryDto> PayrollHistory { get; set; } = new();
    public List<AllowanceDto> CurrentAllowances { get; set; } = new();
    public List<DeductionDto> CurrentDeductions { get; set; } = new();
    public List<LeaveRequestSummaryDto> LeaveRequestsSummary { get; set; } = new();
    public List<PerformanceReviewDto> PerformanceReviews { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class PersonalInfoDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string StaffIdNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public DateOnly? HireDate { get; set; }
    public string ConfirmationStatus { get; set; } = string.Empty;
}

public class CurrentPayrollDto
{
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class PayrollHistoryDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public PayrollHistoryStatus? PaymentStatus { get; set; }
    public DateTime? PaidOn { get; set; }
}

public class AllowanceDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateOnly LastGrantedOn { get; set; }
}

public class DeductionDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateOnly LastDeductedOn { get; set; }
}

public class LeaveRequestSummaryDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class PerformanceReviewDto
{
    public DateTime ReviewDate { get; set; }
    public decimal? OverallRating { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string? Comments { get; set; }
}
