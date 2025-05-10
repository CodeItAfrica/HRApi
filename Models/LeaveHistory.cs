using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class LeaveHistory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("LeaveRequest")]
    public int LeaveRequestId { get; set; }
    public virtual LeaveRequest LeaveRequest { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [StringLength(500)]
    public string? Comments { get; set; }

    [ForeignKey("ChangedBy")]
    public int? ChangedByUserId { get; set; }
    public virtual User? ChangedBy { get; set; }

    public DateTime ChangeDatetime { get; set; } = DateTime.UtcNow;
}
