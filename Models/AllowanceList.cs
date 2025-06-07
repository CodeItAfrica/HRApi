using System.ComponentModel.DataAnnotations;

namespace HRApi.Models;

public class AllowanceList
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
