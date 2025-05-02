using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRApi.Models
{
    public partial class Payroll
    {
        public string Id { get; set; } = null!;
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? PensionRate { get; set; }
        public decimal? HealthInsurance { get; set; }
        public decimal? LoanDeduction { get; set; }
        public decimal? OtherDeductions { get; set; }
        public decimal? Allowances { get; set; }
        public decimal? OvertimeRate { get; set; }
        public decimal? Bonus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
