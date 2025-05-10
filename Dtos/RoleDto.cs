public class RoleNameBodyRequest
{
    public required string roleName { get; set; }
}

public class AssignRoleRequest
{
    public required int UserId { get; set; }
    public required int RoleId { get; set; }
}