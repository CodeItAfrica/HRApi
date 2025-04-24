using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("permissions")]
    public partial class Permission
    {
        public int Id { get; set; }

        public string PermissionName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}