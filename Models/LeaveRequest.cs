using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class LeaveRequest
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("LeaveType")]
    public int LeaveTypeId { get; set; }
    public virtual LeaveType LeaveType { get; set; } = null!;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    [StringLength(500)]
    public string? Reason { get; set; }

    [ForeignKey("ApprovedBy")]
    public int? ApprovedByUserId { get; set; }
    public virtual User? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<LeaveHistory> LeaveHistories { get; set; } = new List<LeaveHistory>();
}
