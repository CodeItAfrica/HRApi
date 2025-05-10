using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class Document
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string DocumentName { get; set; } = null!;

    [ForeignKey("DocumentType")]
    public int DocumentTypeId { get; set; }
    public virtual DocumentType DocumentType { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string FilePath { get; set; } = null!;

    [StringLength(20)]
    public string? FileType { get; set; }

    [ForeignKey("Employee")]
    public string? EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    [ForeignKey("UploadedBy")]
    public int? UploadedByUserId { get; set; }
    public virtual User? UploadedBy { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiryDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Active"; // Active, Expired, Archived

    [StringLength(500)]
    public string? Notes { get; set; }
}
