using System;
using System.Collections.Generic;

namespace HRApi.Models;

public partial class PayrollPayment
{
    public string Id { get; set; } = null!;

    public string? PayrollId { get; set; }

    public string? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? CreatedAt { get; set; }
}
