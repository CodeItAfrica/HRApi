using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class WorkExperience
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
