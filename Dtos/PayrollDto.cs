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

public class PayrollsResponseDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public int GradeId { get; set; }
    public string GradeName { get; set; } = null!;
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal AnnualTax { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public string? PaymentMethod { get; set; }
    public string AccountNumber { get; set; }
    public string BankName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string LastModifiedBy { get; set; }
}

public class EmployeeProfile
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class CreateVariantRequest
{
    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
}

public class AssignVariantRequest
{
    public int Id { get; set; }
    public int PayrollHistoryId { get; set; }
    public decimal Amount { get; set; }
}

public class CreateAllowanceDeductionBodyRequest
{
    public required string Name { get; set; }
    public required decimal Amount { get; set; }
    public required int[] GradeAssign { get; set; }
}
