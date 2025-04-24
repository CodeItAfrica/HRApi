using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("roles")]
    public partial class Role
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}
