using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("audit_log")]
    public partial class AuditLog
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? UserEmail { get; set; }

        public string Action { get; set; } = null!;

        public string? Details { get; set; }

        public string? IpAddress { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}