using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public partial class Role
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }

    public class AssignRoleRequest
    {
        public required int UserId;
        public required int RoleId;
    }
}
