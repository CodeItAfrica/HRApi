using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollHistory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    public DateOnly Period { get; set; }

    [Required]
    public decimal BaseSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HousingAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TransportAllowance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AnnualTax { get; set; }

    public decimal TotalAllowances { get; set; }

    public decimal TotalDeductions { get; set; }

    public decimal GrossSalary { get; set; }

    public decimal NetSalary { get; set; }

    [StringLength(20)]
    public PayrollHistoryStatus? PaymentStatus { get; set; } = PayrollHistoryStatus.Pending;

    public DateTime? PaidOn { get; set; }

    [ForeignKey("ProcessedBy")]
    public int? ProcessedByUserId { get; set; }
    public virtual User? ProcessedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<PayrollPayment> PayrollPayments { get; set; } =
        new List<PayrollPayment>();
}

public enum PayrollHistoryStatus
{
    Pending = 1,
    Processing = 2,
    Paid = 3,
    Failed = 4,
    Cancelled = 5,
}
