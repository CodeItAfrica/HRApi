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

    [ForeignKey("DeductionList")]
    public int? DeductionListId { get; set; }
    public virtual DeductionList? DeductionList { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? LastDeductedBy { get; set; }

    [Required]
    public DateOnly LastDeductedOn { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
