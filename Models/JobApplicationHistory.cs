using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class JobApplicationHistory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("JobApplication")]
    public int JobApplicationId { get; set; }
    public virtual JobApplication JobApplication { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [StringLength(500)]
    public string? Comments { get; set; }

    [ForeignKey("ChangedBy")]
    public int? ChangedByUserId { get; set; }
    public virtual User? ChangedBy { get; set; }

    public DateTime ChangeDatetime { get; set; } = DateTime.UtcNow;
}
