using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollDeduction
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    public DateOnly Period { get; set; }

    [Required]
    [StringLength(50)]
    public string DeductionType { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [ForeignKey("DeductedBy")]
    public int? DeductedByUserId { get; set; }
    public virtual User? DeductedBy { get; set; }

    [Required]
    public DateOnly DeductedOn { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
