using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class JobPosting
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [ForeignKey("Department")]
    public int DepartmentId { get; set; }
    public virtual Department Department { get; set; } = null!;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = null!;

    [StringLength(2000)]
    public string? Requirements { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Open"; // Open, Closed, Filled

    [ForeignKey("PostedBy")]
    public int? PostedByUserId { get; set; }
    public virtual User? PostedBy { get; set; }

    public DateTime PostedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ClosingDate { get; set; }

    // Navigation properties
    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
