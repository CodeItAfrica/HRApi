using System.ComponentModel.DataAnnotations;

namespace HRApi.Models;

public class VariantAllowance
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastModifiedAt { get; set; }
}
