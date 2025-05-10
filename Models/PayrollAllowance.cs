using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollAllowance
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string AllowanceType { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [ForeignKey("GrantedBy")]
    public int? GrantedByUserId { get; set; }
    public virtual User? GrantedBy { get; set; }

    [Required]
    public DateOnly GrantedOn { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}