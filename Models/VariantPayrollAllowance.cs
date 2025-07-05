using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class VariantPayrollAllowance
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("PayrollHistory")]
    public int PayrollHistoryId { get; set; }
    public virtual PayrollHistory PayrollHistory { get; set; } = null!;

    [ForeignKey("VariantAllowance")]
    public int? VariantAllowanceId { get; set; }
    public virtual VariantAllowance? VariantAllowance { get; set; }

    [Required]
    public decimal Amount { get; set; }
    public string? GrantedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
