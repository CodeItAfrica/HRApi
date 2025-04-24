using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("pay_audit_log")]
    public partial class PayAuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public string? Action { get; set; }

        public string? TableName { get; set; }

        public int? RecordId { get; set; }

        public string? OldData { get; set; }

        public string? NewData { get; set; }

        public DateTime? DateTime { get; set; }
    }
}