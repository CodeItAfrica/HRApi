using System.ComponentModel.DataAnnotations;

public class CreateLeaveRequestDto
{
    [Required]
    public int LeaveTypeId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }
}

public class UpdateLeaveStatusDto
{
    [Required]
    public string Status { get; set; } = null!; // "Approved" or "Rejected"

    [StringLength(500)]
    public string? Comments { get; set; }
}