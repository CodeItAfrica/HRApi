using System.ComponentModel.DataAnnotations;
using HRApi.Models;

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

public class PeriodRequest
{
    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
    public int Month { get; set; }

    [Required]
    [Range(2020, 2100, ErrorMessage = "Year must be between 2020 and 2100")]
    public int Year { get; set; }
}

public class PaymentMethodRequest
{
    public required string PaymentMethod { get; set; }
}
