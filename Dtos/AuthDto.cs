using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [EmailAddress]
    [StringLength(100)]
    public string? Email2 { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = null!;

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

public class LoginResponse
{
    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? Name { get; set; }
    public string? Photo { get; set; }

    public string? Surname { get; set; }

    public string? EmployeeId { get; set; }

    public List<string> Roles { get; set; } = new List<string>();
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