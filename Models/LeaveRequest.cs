using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("leave_requests")]
    public partial class LeaveRequest
    {
        public int Id { get; set; }

        public string? EmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        public string? LeaveType { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}