using System.ComponentModel.DataAnnotations;

public class CreateDepartmentRequest
{
    public string DepartmentName { get; set; } = null!;
}

public class CreateUnitRequest
{
    public string UnitName { get; set; } = null!;
    public int DepartmentId { get; set; }
}

public class CreateGradeRequest
{
    public string GradeName { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal AnnualTax { get; set; }
}

public class UpdateGradeRequest
{
    [StringLength(
        100,
        MinimumLength = 1,
        ErrorMessage = "GradeName must be between 1 and 100 characters"
    )]
    public string? GradeName { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "BaseSalary must be non-negative")]
    public decimal? BaseSalary { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "HousingAllowance must be non-negative")]
    public decimal? HousingAllowance { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "TransportAllowance must be non-negative")]
    public decimal? TransportAllowance { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "AnnualTax must be non-negative")]
    public decimal? AnnualTax { get; set; }

    public bool? IsActive { get; set; }
}

public class AssignGradeRequest
{
    public string EmployeeId { get; set; } = null!;
    public int GradeId { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal AnnualTax { get; set; }
}

public class CreateBranchRequest
{
    public string BranchName { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}

public class CreateTrainingProgramRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class CreateBankRequest
{
    public string? BankName { get; set; }
}

public class CreateFAQRequest
{
    public string? Question { get; set; }
    public string? Answer { get; set; }
}

public class JobTitleDto
{
    public string TitleName { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
}

public class CreateNotificationDto
{
    public string EmployeeId { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}
