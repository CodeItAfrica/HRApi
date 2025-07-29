using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class AcademicQualification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
