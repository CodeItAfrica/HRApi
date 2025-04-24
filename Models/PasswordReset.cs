using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("password_resets")]
    public partial class PasswordReset
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string ResetToken { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}