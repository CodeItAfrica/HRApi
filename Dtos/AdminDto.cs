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