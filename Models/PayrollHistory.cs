using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollHistory
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    public DateOnly MonthYear { get; set; }

    [Required]
    public decimal BasicSalary { get; set; }

    public decimal TotalAllowances { get; set; }

    public decimal? TotalOvertime { get; set; }

    public decimal? TotalBonus { get; set; }

    public decimal TaxDeducted { get; set; }

    public decimal PensionDeducted { get; set; }

    public decimal HealthInsurance { get; set; }

    public decimal LoanRepayment { get; set; }

    public decimal? OtherDeductions { get; set; }

    public decimal GrossSalary { get; set; }

    public decimal TotalDeductions { get; set; }

    public decimal NetSalary { get; set; }

    [StringLength(20)]
    public string? PaymentStatus { get; set; } = "Pending"; // Pending, Processing, Paid, Failed

    public DateTime? PaidOn { get; set; }

    [ForeignKey("ProcessedBy")]
    public int? ProcessedByUserId { get; set; }
    public virtual User? ProcessedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<PayrollPayment> PayrollPayments { get; set; } = new List<PayrollPayment>();
}
