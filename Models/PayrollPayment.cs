using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollPayment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("PayrollHistory")]
    public int PayrollHistoryId { get; set; }
    public virtual PayrollHistory PayrollHistory { get; set; } = null!;

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(20)]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [StringLength(100)]
    public string? TransactionId { get; set; } = Guid.NewGuid().ToString();

    public DateTime? PaymentDate { get; set; }

    [ForeignKey("ProcessedBy")]
    public int? ProcessedByUserId { get; set; }
    public virtual User? ProcessedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
}
