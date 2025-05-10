using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class Payroll
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    public decimal BasicSalary { get; set; }

    public decimal? TaxRate { get; set; }

    public decimal? PensionRate { get; set; }

    public decimal? HealthInsurance { get; set; }

    public decimal? LoanDeduction { get; set; }

    public decimal? OtherDeductions { get; set; }

    public decimal? Allowances { get; set; }

    public decimal? OvertimeRate { get; set; }

    public decimal? Bonus { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [StringLength(100)]
    public string? BankName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; } = new List<PayrollDeduction>();
}
