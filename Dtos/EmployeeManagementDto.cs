using System.ComponentModel.DataAnnotations;

public class AcademicQualificationCreateDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string QualificationName { get; set; } = null!;

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearStarted { get; set; }

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearOfCompletion { get; set; }

    [StringLength(250)]
    public string? InstitutionName { get; set; }

    public string? CertificateId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(250)]
    public string? Photo { get; set; }
}

public class AcademicQualificationUpdateDto
{
    [Required]
    [StringLength(100)]
    public string QualificationName { get; set; } = null!;

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearStarted { get; set; }

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearOfCompletion { get; set; }

    [StringLength(250)]
    public string? InstitutionName { get; set; }

    public string? CertificateId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(250)]
    public string? Photo { get; set; }
}

public class AcademicQualificationResponseDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string QualificationName { get; set; } = null!;
    public int YearStarted { get; set; }
    public int YearOfCompletion { get; set; }
    public string? InstitutionName { get; set; }
    public string? CertificateId { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInterestHobbyRequest
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string? Hobby { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateInterestHobbyRequest
{
    public string? Hobby { get; set; }
    public string? Description { get; set; }
}
