using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class GradeDeduction
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Grade")]
    public int GradeId { get; set; }
    public virtual Grade Grade { get; set; } = null!;

    [ForeignKey("DeductionList")]
    public int DeductionListId { get; set; }
    public virtual DeductionList DeductionList { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
