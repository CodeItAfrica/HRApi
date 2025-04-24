using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("employees")]
    public partial class Employee
    {
        public string Id { get; set; } = null!;
        public string StaffIdNo { get; set; } = null!;
        public string? Title { get; set; }
        public string Surname { get; set; } = null!;
        public string? OtherNames { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Sex { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? MaritalStatus { get; set; }
        public string? StateOrigin { get; set; }
        public string? NationalIdNo { get; set; }
        public string? AccountNo1 { get; set; }
        public string? AccountName1 { get; set; }
        public string? AccountNo2 { get; set; }
        public string? AccountName2 { get; set; }
        public string? BranchId { get; set; }
        public string? Branch { get; set; }
        public string? DepartmentId { get; set; }
        public string? Department { get; set; }
        public string? UnitId { get; set; }
        public string? Unit { get; set; }
        public string? GradeId { get; set; }
        public string? Grade { get; set; }
        public DateOnly? HireDate { get; set; }
        public string? Telephone { get; set; }
        public string? MobilePhone { get; set; }
        public string Email { get; set; } = null!;
        public string? Email2 { get; set; }
        public string? NextOfKin { get; set; }
        public string? KinAddress { get; set; }
        public string? KinPhone { get; set; }
        public string? KinRelationship { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public bool? Smoker { get; set; }
        public string? DisabilityType { get; set; }
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
        public string? SubmittedBy { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? HmoName { get; set; }
        public string? HmoId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
