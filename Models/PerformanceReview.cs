using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PerformanceReview
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public string EmployeeId { get; set; } = null!;

    [InverseProperty("PerformanceReviews")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("Reviewer")]
    public string ReviewerId { get; set; } = null!;

    [InverseProperty("ReviewsDone")]
    public virtual Employee Reviewer { get; set; } = null!;

    [Required]
    public DateOnly ReviewPeriod { get; set; }

    public decimal? PerformanceScore { get; set; }

    [StringLength(2000)]
    public string? Feedback { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Acknowledged

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<PerformanceReviewHistory> ReviewHistories { get; set; } =
        new List<PerformanceReviewHistory>();
}
