using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models;

public class GradeAllowance
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Grade")]
    public int GradeId { get; set; }
    public virtual Grade Grade { get; set; } = null!;

    [ForeignKey("AllowanceList")]
    public int AllowanceListId { get; set; }
    public virtual AllowanceList AllowanceList { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
