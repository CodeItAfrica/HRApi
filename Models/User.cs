using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{

    public class User
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public string? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        //public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public virtual ICollection<PayAuditLog> AuditLogs { get; set; } = new List<PayAuditLog>();
    }
}