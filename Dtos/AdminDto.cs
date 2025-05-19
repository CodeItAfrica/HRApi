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
    public string TitleName { get; set; }
    public string DepartmentName { get; set; }
}

public class CreateNotificationDto
{
    public int EmployeeId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
