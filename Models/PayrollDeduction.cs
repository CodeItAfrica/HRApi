using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    [Table("payroll_deductions")]
    public partial class PayrollDeduction
    {
        public int Id { get; set; }

        public int? PayrollId { get; set; }

        public string? DeductionType { get; set; }

        public decimal Amount { get; set; }
    }
}
