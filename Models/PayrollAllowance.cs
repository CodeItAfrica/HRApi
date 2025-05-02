using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public partial class PayrollAllowance
    {
        public string Id { get; set; } = null!;
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? AllowanceType { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? GrantedBy { get; set; }
        public DateOnly GrantedOn { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
