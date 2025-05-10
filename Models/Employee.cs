using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public class Employee
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(20)]
        public string StaffIdNo { get; set; } = null!;

        [StringLength(20)]
        public string? Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Surname { get; set; } = null!;

        [StringLength(200)]
        public string? OtherNames { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(10)]
        public string? Sex { get; set; }

        public DateOnly? BirthDate { get; set; }

        [StringLength(20)]
        public string? MaritalStatus { get; set; }

        [StringLength(100)]
        public string? StateOrigin { get; set; }

        [StringLength(50)]
        public string? NationalIdNo { get; set; }

        [StringLength(50)]
        public string? AcctNo1 { get; set; }

        [StringLength(100)]
        public string? AcctName1 { get; set; }

        [StringLength(50)]
        public string? AcctNo2 { get; set; }

        [StringLength(100)]
        public string? AcctName2 { get; set; }

        [ForeignKey("Branch")]
        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        [ForeignKey("Unit")]
        public int? UnitId { get; set; }
        public virtual Unit? Unit { get; set; }

        [ForeignKey("Grade")]
        public int? GradeId { get; set; }
        public virtual Grade? Grade { get; set; }

        [ForeignKey("EmploymentType")]
        public int? EmploymentTypeId { get; set; }
        public virtual EmploymentType? EmploymentType { get; set; }

        public DateOnly? HireDate { get; set; }

        [StringLength(20)]
        public string? Telephone { get; set; }

        [StringLength(20)]
        public string? MobilePhone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [EmailAddress]
        [StringLength(100)]
        public string? Email2 { get; set; }

        [StringLength(150)]
        public string? NextKin { get; set; }

        [StringLength(300)]
        public string? KinAddress { get; set; }

        [StringLength(20)]
        public string? KinPhone { get; set; }

        [StringLength(50)]
        public string? KinRelationship { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public bool? Smoker { get; set; }

        [StringLength(100)]
        public string? DisableType { get; set; }

        [StringLength(100)]
        public string? HmoName { get; set; }

        [StringLength(50)]
        public string? HmoId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string? Tag { get; set; }

        [StringLength(250)]
        public string? Photo { get; set; }

        public bool? PayFirstMonth { get; set; }

        [StringLength(50)]
        public string? SheetId2 { get; set; }

        public bool? ConfirmStatus { get; set; }

        public int? ConfirmDuration { get; set; }

        public DateOnly? ConfirmationDate { get; set; }

        public DateOnly? RetiredDate { get; set; }

        public bool? Deleted { get; set; } = false;

        public bool? Active { get; set; } = true;

        [StringLength(100)]
        public string? SubmitBy { get; set; }

        public DateTime? SubmitOn { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
        public virtual Payroll? Payroll { get; set; }
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<PerformanceReview> PerformanceReviews { get; set; } = new List<PerformanceReview>();
        public virtual ICollection<PerformanceReview> ReviewsDone { get; set; } = new List<PerformanceReview>();
        public virtual ICollection<PayrollAllowance> PayrollAllowances { get; set; } = new List<PayrollAllowance>();
        public virtual ICollection<PayrollHistory> PayrollHistories { get; set; } = new List<PayrollHistory>();
        public virtual ICollection<PayrollPayment> PayrollPayments { get; set; } = new List<PayrollPayment>();
    }
}
