using System.ComponentModel.DataAnnotations;

public class EmployeeResponse
{
    public string? EmployeeId { get; set; }
    public string? UserId { get; set; }
    public string? StaffIdNo { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Sex { get; set; }
    public string? Email2 { get; set; }
    public string? Address { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? MaritalStatus { get; set; }
    public string? StateOrigin { get; set; }
    public string? NationalIdNo { get; set; }
    public string? AcctNo1 { get; set; }
    public string? AcctName1 { get; set; }
    public string? AcctNo2 { get; set; }
    public string? AcctName2 { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public int? DeptId { get; set; }
    public string? DeptName { get; set; }
    public int? UnitId { get; set; }
    public string? UnitName { get; set; }
    public int? GradeId { get; set; }
    public string? GradeName { get; set; }
    public int? EmploymentTypeId { get; set; }
    public string? EmploymentTypeName { get; set; }
    public DateOnly? HireDate { get; set; }
    public string? Telephone { get; set; }
    public string? MobilePhone { get; set; }
    public string? NextKin { get; set; }
    public string? KinAddress { get; set; }
    public string? KinPhone { get; set; }
    public string? KinRelationship { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public bool? Smoker { get; set; }
    public string? DisableType { get; set; }
    public string? HmoName { get; set; }
    public string? HmoId { get; set; }
    public string? Remarks { get; set; }
    public string? Tag { get; set; }
    public string? Photo { get; set; }
    public bool? PayFirstMonth { get; set; }
    public string? SheetId2 { get; set; }
    public bool? ConfirmStatus { get; set; }
    public int? ConfirmDuration { get; set; }
    public DateOnly? ConfirmationDate { get; set; }
    public DateOnly? RetiredDate { get; set; }
    public bool? Deleted { get; set; } = false;
    public bool? Active { get; set; } = true;
    public string? SubmitBy { get; set; }
    public DateTime? SubmitOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<string> Roles { get; set; } = new List<string>();
}

public class UpdateEmployeeRequest
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [EmailAddress]
    [StringLength(100)]
    public string? Email2 { get; set; }

    [StringLength(20)]
    public string? Title { get; set; }

    [Required]
    [StringLength(100)]
    public string Surname { get; set; } = null!;

    [StringLength(200)]
    public string? OtherNames { get; set; }

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = null!;

    [StringLength(20)]
    public string? MobilePhone { get; set; }

    [Required]
    [StringLength(300)]
    public string Address { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string State { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = null!;

    [Required]
    [StringLength(10)]
    public string Sex { get; set; } = null!;

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

    public int? BranchId { get; set; }

    public int? DeptId { get; set; }

    public int? UnitId { get; set; }

    public int? GradeId { get; set; }

    public DateOnly? BirthDate { get; set; }

    public DateOnly? HireDate { get; set; }

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

    [StringLength(500)]
    public string? Remarks { get; set; }

    [StringLength(100)]
    public string? Tag { get; set; }

    //[SwaggerSchema("Upload a photo")]
    //public IFormFile? PhotoFile { get; set; }

    public bool? PayFirstMonth { get; set; }

    [StringLength(50)]
    public string? SheetId2 { get; set; }

    public bool? ConfirmStatus { get; set; }

    public int? ConfirmDuration { get; set; }

    public DateOnly? ConfirmationDate { get; set; }

    public DateOnly? RetiredDate { get; set; }

    public bool? Active { get; set; } = true;

    [StringLength(100)]
    public string? HmoName { get; set; }

    [StringLength(50)]
    public string? HmoId { get; set; }
    public string? ModifiedBy { get; set; }
}