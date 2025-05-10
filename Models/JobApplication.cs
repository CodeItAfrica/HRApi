using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class JobApplication
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("JobPosting")]
    public int JobPostingId { get; set; }
    public virtual JobPosting JobPosting { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string ApplicantName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? ResumeUrl { get; set; }

    [StringLength(300)]
    public string? CoverLetter { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Submitted"; // Submitted, Reviewing, Interviewed, Rejected, Offered, Hired

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<JobApplicationHistory> ApplicationHistories { get; set; } = new List<JobApplicationHistory>();
}
