using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class PayAuditLog
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    [StringLength(100)]
    public string? Action { get; set; }

    [StringLength(100)]
    public string? TableName { get; set; }

    public int? RecordId { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public DateTime Datetime { get; set; } = DateTime.UtcNow;
}
