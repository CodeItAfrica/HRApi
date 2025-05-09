using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class JobPosting
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public string? Description { get; set; }

    public string? Requirements { get; set; }

    public string? Status { get; set; }

    public DateTime? PostedAt { get; set; }
}
