using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
