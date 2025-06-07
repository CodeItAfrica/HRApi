using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class EmployeeGradeHistory
    {
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey("Grade")]
        public int? GradeId { get; set; }
        public virtual Grade? Grade { get; set; }
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HousingAllowance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TransportAllowance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualTax { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
