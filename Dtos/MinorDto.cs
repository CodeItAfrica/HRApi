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
