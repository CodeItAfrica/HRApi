using System;
using System.Collections.Generic;

namespace HRApi.Models
{
    public partial class Role
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }

    public class RoleNameBodyRequest
    {
        public required string roleName { get; set; }
    }

    public class AssignRoleRequest
    {
        public required int UserId { get; set; }
        public required int RoleId { get; set; }
    }
}