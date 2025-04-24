using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("announcements")]
    public partial class Announcement
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string Message { get; set; } = null!;

        public string? PostedBy { get; set; }

        public DateTime? PostedAt { get; set; }
    }
}
