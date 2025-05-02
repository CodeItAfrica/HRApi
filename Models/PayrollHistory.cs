using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public partial class PayrollHistory
    {
        public string Id { get; set; } = null!;

        public string? EmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        public DateOnly MonthYear { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal? TotalOvertime { get; set; }

        public decimal? TotalBonus { get; set; }

        public decimal TaxDeducted { get; set; }

        public decimal PensionDeducted { get; set; }

        public decimal HealthInsurance { get; set; }

        public decimal LoanRepayment { get; set; }

        public decimal? OtherDeductions { get; set; }

        public decimal GrossSalary { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal NetSalary { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime? PaidOn { get; set; }

        public string? ProcessedBy { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
