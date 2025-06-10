using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayrollAllowance
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("AllowanceList")]
    public int AllowanceListId { get; set; }
    public virtual AllowanceList AllowanceList { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? LastGrantedBy { get; set; }

    [Required]
    public DateOnly LastGrantedOn { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
