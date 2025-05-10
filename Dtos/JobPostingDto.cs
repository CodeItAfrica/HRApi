using System.ComponentModel.DataAnnotations;

public class CreateJobPostingDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    public int DepartmentId { get; set; }

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = null!;

    [StringLength(2000)]
    public string? Requirements { get; set; }

    public DateTime? ClosingDate { get; set; }
}

public class JobApplicationDto
{
    [Required]
    public int JobPostingId { get; set; }

    [Required]
    [StringLength(200)]
    public string ApplicantName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(300)]
    public string? CoverLetter { get; set; }

    // Resume file would be handled separately in the controller
}