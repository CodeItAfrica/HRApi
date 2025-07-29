using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class StaffCompensation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;

        public DateOnly? IncidentDate { get; set; }

        public DateOnly? NotifyDate { get; set; }

        public string InjuryDetails { get; set; } = null!;

        public string? LocationDetails { get; set; }

        public int DaysAway { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MedicalCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OtherCost { get; set; }
    }
}
