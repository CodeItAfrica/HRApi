using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class Payroll
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    public int GradeId { get; set; }

    [Required]
    public decimal BaseSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HousingAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TransportAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AnnualTax { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAllowances { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDeductions { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSalary { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [StringLength(100)]
    public string? BankName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [StringLength(100)]
    public string LastModifiedBy { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; } =
        new List<PayrollDeduction>();

    public virtual ICollection<PayrollAllowance> PayrollAllowances { get; set; } =
        new List<PayrollAllowance>();
}
