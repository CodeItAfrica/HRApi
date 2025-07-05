using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string GradeName { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HousingAllowance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TransportAllowance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualTax { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<GradeAllowance> GradeAllowances { get; set; } =
            new List<GradeAllowance>();
        public virtual ICollection<GradeDeduction> GradeDeductions { get; set; } =
            new List<GradeDeduction>();
    }
}
