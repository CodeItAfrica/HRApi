using System.ComponentModel.DataAnnotations;

public class RoleNameBodyRequest
{
    public required string RoleName { get; set; }
}

public class AssignRoleRequest
{
    public required int UserId { get; set; }
    public required int RoleId { get; set; }
}

public class NameBodyRequest
{
    public required string Name { get; set; }
}

public class UpdateEmployeePaySheetIDRequest
{
    public required string EmployeeId { get; set; }
    public required int PaySheetId { get; set; }
}

public class UpdateAmountRequest
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value")]
    public decimal NewAmount { get; set; }
}
