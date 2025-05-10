using System.ComponentModel.DataAnnotations;

public class CreatePerformanceReviewDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    public DateOnly ReviewPeriod { get; set; }

    public decimal? PerformanceScore { get; set; }

    [StringLength(2000)]
    public string? Feedback { get; set; }
}

public class UpdatePerformanceReviewDto
{
    public decimal? PerformanceScore { get; set; }

    [StringLength(2000)]
    public string? Feedback { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }
}