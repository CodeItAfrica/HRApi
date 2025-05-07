using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class JobApplicationHistory
{
    public int Id { get; set; }

    public string? JobApplicationId { get; set; }

    public string? CandidateName { get; set; }

    public string? PositionApplied { get; set; }

    public string? Status { get; set; }

    public DateTime? ChangeDatetime { get; set; }
}
