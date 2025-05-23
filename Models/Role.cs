using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRApi.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
