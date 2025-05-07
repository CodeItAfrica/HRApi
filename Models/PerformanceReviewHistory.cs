using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class PerformanceReviewHistory
{
    public int Id { get; set; }

    public string? PerformanceReviewId { get; set; }

    public string? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? ReviewerId { get; set; }

    public string? ReviewerName { get; set; }

    public DateOnly? ReviewPeriod { get; set; }

    public decimal? PerformanceScore { get; set; }

    public string? Feedback { get; set; }

    public DateTime? ChangeDatetime { get; set; }
}
