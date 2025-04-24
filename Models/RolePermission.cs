using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("role_permissions")]
    public partial class RolePermission
    {
        public int Id { get; set; }

        public string? RoleId { get; set; }

        public string? RoleName { get; set; }

        public string? PermissionId { get; set; }

        public string? PermissionName { get; set; }

        public DateTime? AssignedAt { get; set; }
    }
}