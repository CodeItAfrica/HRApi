using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{

    public class User
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public virtual ICollection<PayAuditLog> AuditLogs { get; set; } = new List<PayAuditLog>();
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string? Email2 { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public string Phone { get; set; }
        public string? MobilePhone { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Sex { get; set; }
        public string? MaritalStatus { get; set; }
        public string? StateOrigin { get; set; }
        public string? NationalIdNo { get; set; }
        public string? AcctNo1 { get; set; }
        public string? AcctName1 { get; set; }
        public string? AcctNo2 { get; set; }
        public string? AcctName2 { get; set; }
        public string? BranchId { get; set; }
        public string? Branch { get; set; }
        public string? DeptId { get; set; }
        public string? Dept { get; set; }
        public string? UnitId { get; set; }
        public string? Unit { get; set; }
        public string? GradeId { get; set; }
        public string? Grade { get; set; }
        public DateOnly? BirthDate { get; set; }
        public DateOnly? HireDate { get; set; }
        public string? NextKin { get; set; }
        public string? KinAddress { get; set; }
        public string? KinPhone { get; set; }
        public string? KinRelationship { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public bool? Smoker { get; set; }
        public string? DisableType { get; set; }
        public string? Remarks { get; set; }
        public string? Tag { get; set; }
        public string? Photo { get; set; }
        public bool? PayFirstMonth { get; set; }
        public string? SheetId2 { get; set; }
        public bool? ConfirmStatus { get; set; }
        public int? ConfirmDuration { get; set; }
        public DateOnly? ConfirmationDate { get; set; }
        public DateOnly? RetiredDate { get; set; }
        public bool? Deleted { get; set; }
        public bool? Active { get; set; }
        public string? SubmitBy { get; set; }
        public DateTime? SubmitOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? HmoName { get; set; }
        public string? HmoId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? EmployeeId { get; set; }
        public List<string> Roles { get; set; }
    }

    public class CodeVerificationRequest
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
    }


    public class ResetPasswordCodeRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
    public class EmailRequest
    {
        public string Email { get; set; }
    }
}