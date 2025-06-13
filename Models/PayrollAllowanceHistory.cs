using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollAllowanceHistory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
    public int Month { get; set; }

    [Required]
    [Range(2020, 2100, ErrorMessage = "Year must be between 2020 and 2100")]
    public int Year { get; set; }

    [Required]
    [StringLength(50)]
    public string AllowanceName { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [Required]
    public DateOnly LastModifiedOn { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
