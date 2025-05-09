using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class Document
{
    public int Id { get; set; }

    public string? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? DocumentType { get; set; }

    public string FileUrl { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }
}
