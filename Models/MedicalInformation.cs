using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class MedicalInformation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string BloodGroup { get; set; } = null!;

        [StringLength(500)]
        public string? Genotype { get; set; }

        [StringLength(250)]
        public string? MajorAllergies { get; set; }

        [StringLength(500)]
        public string? ChronicConditions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
