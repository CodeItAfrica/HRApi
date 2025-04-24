using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("job_application_history")]
    public partial class JobApplicationHistory
    {
        public int Id { get; set; }

        public string? JobApplicationId { get; set; }

        public string? CandidateName { get; set; }

        public string? PositionApplied { get; set; }

        public string? Status { get; set; }

        public DateTime? ChangeDateTime { get; set; }
    }
}
