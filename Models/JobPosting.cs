using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("job_postings")]
    public partial class JobPosting
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public string? Description { get; set; }

        public string? Requirements { get; set; }

        public string? Status { get; set; }

        public DateTime? PostedAt { get; set; }
    }
}