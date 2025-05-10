using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PerformanceReviewHistory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("PerformanceReview")]
    public int PerformanceReviewId { get; set; }
    public virtual PerformanceReview PerformanceReview { get; set; } = null!;

    public decimal? PerformanceScore { get; set; }

    [StringLength(2000)]
    public string? Feedback { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [ForeignKey("ChangedBy")]
    public int? ChangedByUserId { get; set; }
    public virtual User? ChangedBy { get; set; }

    public DateTime ChangeDatetime { get; set; } = DateTime.UtcNow;
}
