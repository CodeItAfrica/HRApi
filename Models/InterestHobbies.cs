using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class InterestHobbies
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Hobby { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
