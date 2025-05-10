using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollDeduction
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Payroll")]
    public string PayrollId { get; set; } = null!;
    public virtual Payroll Payroll { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string DeductionType { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
