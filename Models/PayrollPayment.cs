using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollPayment
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("PayrollHistory")]
    public string PayrollHistoryId { get; set; } = null!;
    public virtual PayrollHistory PayrollHistory { get; set; } = null!;

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(20)]
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Processing, Completed, Failed

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }

    [ForeignKey("ProcessedBy")]
    public int? ProcessedByUserId { get; set; }
    public virtual User? ProcessedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
