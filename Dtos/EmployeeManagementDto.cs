using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

public class InterestHobbiesDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string EmployeeStaffId { get; set; } = null!;
    public string Hobby { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInterestHobbiesDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Hobby { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }
}

public class UpdateInterestHobbiesDto
{
    [Required]
    [StringLength(100)]
    public string Hobby { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }
}

public class CreateMultipleHobbiesDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    [MinLength(1, ErrorMessage = "At least one hobby is required")]
    public List<HobbyDto> Hobbies { get; set; } = new List<HobbyDto>();
}

public class HobbyDto
{
    [Required]
    [StringLength(100)]
    public string Hobby { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }
}

public class HobbiesSummaryDto
{
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string EmployeeStaffId { get; set; } = null!;
    public int TotalHobbies { get; set; }
    public List<string> HobbiesList { get; set; } = new List<string>();
    public int HobbiesWithDescription { get; set; }
    public string? MostRecentHobby { get; set; }
    public string? OldestHobby { get; set; }
}

public class HobbiesStatisticsDto
{
    public int TotalHobbiesRecorded { get; set; }
    public int UniqueHobbies { get; set; }
    public int EmployeesWithHobbies { get; set; }
    public double AverageHobbiesPerEmployee { get; set; }
    public List<HobbyStatistic> MostPopularHobbies { get; set; } = new List<HobbyStatistic>();
    public int HobbiesWithDescription { get; set; }
    public double PercentageWithDescription { get; set; }
}

public class HobbyStatistic
{
    public string Hobby { get; set; } = null!;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CommonHobbyDto
{
    public string Hobby { get; set; } = null!;
    public int Count { get; set; }
    public List<EmployeeBasicDto> Employees { get; set; } = new List<EmployeeBasicDto>();
}

public class EmployeeBasicDto
{
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string StaffId { get; set; } = null!;
}

public class MedicalInformationDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string BloodGroup { get; set; } = null!;
    public string? Genotype { get; set; }
    public string? MajorAllergies { get; set; }
    public string? ChronicConditions { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMedicalInformationDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string BloodGroup { get; set; } = null!;

    [StringLength(500)]
    public string? Genotype { get; set; }

    [StringLength(250)]
    public string? MajorAllergies { get; set; }

    [StringLength(500)]
    public string? ChronicConditions { get; set; }

    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
}

public class UpdateMedicalInformationDto
{
    [Required]
    [StringLength(100)]
    public string BloodGroup { get; set; } = null!;

    [StringLength(500)]
    public string? Genotype { get; set; }

    [StringLength(250)]
    public string? MajorAllergies { get; set; }

    [StringLength(500)]
    public string? ChronicConditions { get; set; }

    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
}

public class WorkExperienceDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public int YearStarted { get; set; }
    public int YearEnded { get; set; }
    public string Industry { get; set; } = null!;
    public string? Achievement { get; set; }
    public string Duration { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class CreateWorkExperienceDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string JobTitle { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string CompanyName { get; set; } = null!;

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearStarted { get; set; }

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearEnded { get; set; }

    [Required]
    public string Industry { get; set; } = null!;

    [StringLength(500)]
    public string? Achievement { get; set; }
}

public class UpdateWorkExperienceDto
{
    [Required]
    [StringLength(100)]
    public string JobTitle { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string CompanyName { get; set; } = null!;

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearStarted { get; set; }

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int YearEnded { get; set; }

    [Required]
    public string Industry { get; set; } = null!;

    [StringLength(500)]
    public string? Achievement { get; set; }
}

public class WorkExperienceSummaryDto
{
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public int TotalExperiences { get; set; }
    public int TotalYearsOfExperience { get; set; }
    public List<string> Industries { get; set; } = new List<string>();
    public string? MostRecentJob { get; set; }
    public string? MostRecentCompany { get; set; }
}

public class StaffCompensationDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string EmployeeStaffId { get; set; } = null!;
    public DateOnly? IncidentDate { get; set; }
    public DateOnly? NotifyDate { get; set; }
    public string InjuryDetails { get; set; } = null!;
    public string? LocationDetails { get; set; }
    public int DaysAway { get; set; }
    public decimal MedicalCost { get; set; }
    public decimal OtherCost { get; set; }
    public decimal TotalCost { get; set; }
    public int? NotificationDelay { get; set; }
}

public class CreateStaffCompensationDto
{
    [Required]
    public string EmployeeId { get; set; } = null!;

    public DateOnly? IncidentDate { get; set; }

    public DateOnly? NotifyDate { get; set; }

    [Required]
    public string InjuryDetails { get; set; } = null!;

    public string? LocationDetails { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Days away must be non-negative")]
    public int DaysAway { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Medical cost must be non-negative")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MedicalCost { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Other cost must be non-negative")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherCost { get; set; }
}

public class UpdateStaffCompensationDto
{
    public DateOnly? IncidentDate { get; set; }

    public DateOnly? NotifyDate { get; set; }

    [Required]
    public string InjuryDetails { get; set; } = null!;

    public string? LocationDetails { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Days away must be non-negative")]
    public int DaysAway { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Medical cost must be non-negative")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MedicalCost { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Other cost must be non-negative")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherCost { get; set; }
}

public class StaffCompensationSummaryDto
{
    public string EmployeeId { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string EmployeeStaffId { get; set; } = null!;
    public int TotalIncidents { get; set; }
    public int TotalDaysAway { get; set; }
    public decimal TotalMedicalCost { get; set; }
    public decimal TotalOtherCost { get; set; }
    public decimal TotalCompensationCost { get; set; }
    public DateOnly? MostRecentIncidentDate { get; set; }
    public double AverageDaysAway { get; set; }
    public double AverageNotificationDelay { get; set; }
}

public class CompensationStatisticsDto
{
    public int TotalIncidents { get; set; }
    public int TotalEmployeesAffected { get; set; }
    public int TotalDaysAway { get; set; }
    public decimal TotalMedicalCost { get; set; }
    public decimal TotalOtherCost { get; set; }
    public decimal TotalCompensationCost { get; set; }
    public double AverageDaysAway { get; set; }
    public decimal AverageMedicalCost { get; set; }
    public double AverageNotificationDelay { get; set; }
    public List<LocationStatistic> MostCommonLocations { get; set; } =
        new List<LocationStatistic>();
}

public class LocationStatistic
{
    public string Location { get; set; } = null!;
    public int Count { get; set; }
}
