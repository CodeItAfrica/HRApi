using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? UserEmail { get; set; }

    public int? RoleId { get; set; }

    public string? RoleName { get; set; }

    public DateTime? AssignedAt { get; set; }
}
