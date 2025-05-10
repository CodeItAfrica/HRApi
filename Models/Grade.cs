using System.ComponentModel.DataAnnotations;

namespace HRApi.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string GradeName { get; set; } = null!;

        [Required]
        public decimal BaseSalary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}