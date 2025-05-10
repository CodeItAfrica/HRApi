using System.ComponentModel.DataAnnotations;

public class CreatePayrollDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    public decimal BasicSalary { get; set; }

    public decimal? TaxRate { get; set; }

    public decimal? PensionRate { get; set; }

    public decimal? HealthInsurance { get; set; }

    public decimal? OvertimeRate { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [StringLength(100)]
    public string? BankName { get; set; }
}

public class CreatePayrollAllowanceDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string AllowanceType { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [Required]
    public DateOnly GrantedOn { get; set; }
}

public class ProcessPayrollDto
{
    [Required]
    public DateOnly PayrollMonth { get; set; }

    public List<string>? EmployeeIds { get; set; } // If null, process for all employees
}