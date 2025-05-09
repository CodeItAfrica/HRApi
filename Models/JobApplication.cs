using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class JobApplication
{
    public int Id { get; set; }

    public string? JobTitle { get; set; }

    public string ApplicantName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? ResumeUrl { get; set; }

    public string? Status { get; set; }

    public DateTime? AppliedAt { get; set; }
}
