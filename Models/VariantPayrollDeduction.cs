using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class VariantPayrollDeduction
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("PayrollHistory")]
    public int PayrollHistoryId { get; set; }
    public virtual PayrollHistory PayrollHistory { get; set; } = null!;

    [ForeignKey("VariantDeduction")]
    public int? VariantDeductionId { get; set; }
    public virtual VariantDeduction? VariantDeduction { get; set; }

    [Required]
    public decimal Amount { get; set; }
    public string? GrantedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
