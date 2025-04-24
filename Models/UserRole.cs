using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("user_roles")]
    public partial class UserRole
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? UserEmail { get; set; }

        public string? RoleId { get; set; }

        public string? RoleName { get; set; }

        public DateTime? AssignedAt { get; set; }
    }
}
