using System.Security.Claims;
using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PayrollService _payrollService;

    public PayrollController(AppDbContext context, PayrollService payrollService)
    {
        _context = context;
        _payrollService = payrollService;
    }

    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<Payroll>>> GetAllPayrolls()
    {
        try
        {
            var payrolls = await _context
                .Payrolls.Include(p => p.Employee)
                .Join(
                    _context.Grades,
                    payroll => payroll.GradeId,
                    grade => grade.Id,
                    (payroll, grade) => new { Payroll = payroll, Grade = grade }
                )
                .OrderByDescending(p => p.Payroll.CreatedAt)
                .ToListAsync();

            var response = payrolls.Select(p => new PayrollsResponseDto
            {
                Id = p.Payroll.Id,
                EmployeeId = p.Payroll.EmployeeId,
                Employee = new EmployeeProfile
                {
                    FullName = p.Payroll.Employee.FullName,
                    Email = p.Payroll.Employee.Email,
                },
                GradeId = p.Payroll.GradeId,
                GradeName = p.Grade.GradeName, // Add this line
                BaseSalary = p.Payroll.BaseSalary,
                HousingAllowance = p.Payroll.HousingAllowance,
                TransportAllowance = p.Payroll.TransportAllowance,
                AnnualTax = p.Payroll.AnnualTax,
                TotalAllowances = p.Payroll.TotalAllowances,
                TotalDeductions = p.Payroll.TotalDeductions,
                GrossSalary = p.Payroll.GrossSalary,
                NetSalary = p.Payroll.NetSalary,
                PaymentMethod = p.Payroll.PaymentMethod,
                AccountNumber = p.Payroll.AccountNumber ?? string.Empty,
                BankName = p.Payroll.BankName ?? string.Empty,
                CreatedAt = p.Payroll.CreatedAt,
                UpdatedAt = p.Payroll.UpdatedAt,
                LastModifiedBy = p.Payroll.LastModifiedBy,
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { message = "An error occurred while retrieving payrolls", error = ex.Message }
            );
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Payroll>> GetPayrollById(int id)
    {
        try
        {
            var payroll = await _context
                .Payrolls.Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payroll == null)
            {
                return NotFound(new { message = $"Payroll with ID {id} not found" });
            }

            var payrollAllowances = await _context
                .PayrollAllowances.Include(pa => pa.AllowanceList)
                .Where(pa => pa.EmployeeId == payroll.EmployeeId)
                .ToListAsync();

            var payrollDeductions = await _context
                .PayrollDeductions.Include(pd => pd.DeductionList)
                .Where(pd => pd.EmployeeId == payroll.EmployeeId)
                .ToListAsync();

            var newTotalAllowance = await _payrollService.CalculateTotalAllowancesAsync(
                payroll.EmployeeId
            );
            var newTotalDeduction = await _payrollService.CalculateTotalDeductionsAsync(
                payroll.EmployeeId
            );

            payroll.TotalAllowances = newTotalAllowance;
            payroll.TotalDeductions = newTotalDeduction;

            payroll.GrossSalary = payroll.BaseSalary + newTotalAllowance;
            payroll.NetSalary = payroll.GrossSalary - newTotalDeduction;

            await _context.SaveChangesAsync();

            var result = new
            {
                payroll.Id,
                payroll.EmployeeId,
                Employee = new { payroll.Employee.FullName, payroll.Employee.Email },
                payroll.BaseSalary,
                payroll.HousingAllowance,
                payroll.TransportAllowance,
                payroll.AnnualTax,
                payroll.TotalAllowances,
                payroll.TotalDeductions,
                payroll.GrossSalary,
                payroll.PaymentMethod,
                payroll.AccountNumber,
                payroll.BankName,
                payroll.NetSalary,
                payroll.CreatedAt,
                payroll.UpdatedAt,
                payroll.LastModifiedBy,
                PayrollAllowances = payrollAllowances
                    .Select(pa => new
                    {
                        pa.Id,
                        pa.Amount,
                        pa.Description,
                        pa.LastGrantedBy,
                        pa.LastGrantedOn,
                        pa.CreatedAt,
                        AllowanceType = new { pa.AllowanceList.Id, pa.AllowanceList.Name },
                    })
                    .ToList(),
                PayrollDeductions = payrollDeductions
                    .Select(pd => new
                    {
                        pd.Id,
                        pd.Amount,
                        pd.Description,
                        pd.LastDeductedBy,
                        pd.LastDeductedOn,
                        pd.CreatedAt,
                        DeductionType = new { pd.DeductionList.Id, pd.DeductionList.Name },
                    })
                    .ToList(),
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { message = "An error occurred while retrieving payroll", error = ex.Message }
            );
        }
    }

    [HttpPatch("update-payment-method/{id}")]
    public async Task<IActionResult> UpdatePaymentMethod(
        int id,
        [FromBody] PaymentMethodRequest request
    )
    {
        try
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound(new { message = $"Payroll with ID {id} not found" });
            }

            payroll.PaymentMethod = request.PaymentMethod;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment method has been updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { message = "An error occurred while retrieving payroll", error = ex.Message }
            );
        }
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("process/{id}")]
    public async Task<IActionResult> ProcessPayroll(int id, [FromBody] PeriodRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound(new { message = $"Payroll with ID {id} not found" });
            }

            if (payroll.PaymentMethod == null)
            {
                return BadRequest(
                    new { message = "Payment method is required to process payroll" }
                );
            }

            var isAlreadyProcessed = await _payrollService.IsPayrollAlreadyProcessedAsync(
                payroll.EmployeeId,
                request.Month,
                request.Year
            );

            if (isAlreadyProcessed)
            {
                return BadRequest(
                    new
                    {
                        message = $"Payroll for employee {payroll.EmployeeId} has already been processed for {request.Month:D2}/{request.Year}",
                    }
                );
            }

            var grade = await _context.Grades.FindAsync(payroll.GradeId);
            if (grade == null)
            {
                return NotFound(new { message = $"Grade for payroll ID {id} not found" });
            }

            var newTotalAllowance = await _payrollService.CalculateTotalAllowancesAsync(
                payroll.EmployeeId
            );
            var newTotalDeduction = await _payrollService.CalculateTotalDeductionsAsync(
                payroll.EmployeeId
            );

            payroll.TotalAllowances = newTotalAllowance;
            payroll.TotalDeductions = newTotalDeduction;

            payroll.GrossSalary = payroll.BaseSalary + newTotalAllowance;
            payroll.NetSalary = payroll.GrossSalary - newTotalDeduction;

            var payrollHistory = new PayrollHistory
            {
                EmployeeId = payroll.EmployeeId,
                Month = request.Month,
                Year = request.Year,
                BaseSalary = payroll.BaseSalary,
                HousingAllowance = payroll.HousingAllowance,
                TransportAllowance = payroll.TransportAllowance,
                AnnualTax = payroll.AnnualTax,
                TotalAllowances = payroll.TotalAllowances,
                TotalDeductions = payroll.TotalDeductions,
                GrossSalary = payroll.GrossSalary,
                NetSalary = payroll.NetSalary,
                PaymentStatus = PayrollHistoryStatus.Processing,
            };

            _context.PayrollHistories.Add(payrollHistory);

            await _payrollService.CreatePayrollAllowanceHistoryForEmployee(
                payroll.EmployeeId,
                request.Month,
                request.Year
            );

            await _payrollService.CreatePayrollDeductionHistoryForEmployee(
                payroll.EmployeeId,
                request.Month,
                request.Year
            );

            await _context.SaveChangesAsync();

            var payrollPayments = new PayrollPayment
            {
                PayrollHistoryId = payrollHistory.Id,
                EmployeeId = payroll.EmployeeId,
                PaymentMethod = payroll.PaymentMethod,
            };

            _context.PayrollPayments.Add(payrollPayments);

            var createdByEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var newPayroll = new Payroll
            {
                EmployeeId = payroll.EmployeeId,
                GradeId = grade.Id,
                BaseSalary = grade.BaseSalary,
                HousingAllowance = grade.HousingAllowance,
                TransportAllowance = grade.TransportAllowance,
                AnnualTax = grade.AnnualTax,
                TotalAllowances = 0m,
                TotalDeductions = 0m,
                GrossSalary = grade.BaseSalary + grade.HousingAllowance + grade.TransportAllowance,
                NetSalary = grade.BaseSalary + grade.HousingAllowance + grade.TransportAllowance,
                PaymentMethod = payroll.PaymentMethod,
                AccountNumber = payroll.AccountNumber,
                BankName = payroll.BankName,
                CreatedBy = createdByEmail ?? "Unknown",
                LastModifiedBy = createdByEmail ?? "Unknown",
            };

            _context.Payrolls.Add(newPayroll);
            _context.Payrolls.Remove(payroll);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            try
            {
                await _payrollService.CreatePayrollAllowancesForEmployee(payroll.EmployeeId);
                await _payrollService.CreatePayrollDeductionsForEmployee(payroll.EmployeeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Warning: Failed to recreate allowances/deductions for employee {payroll.EmployeeId}: {ex.Message}"
                );
            }

            return Ok(
                new
                {
                    message = "Payroll processed successfully",
                    payrollHistoryId = payrollHistory.Id,
                    employeeId = payroll.EmployeeId,
                    paymentMethod = payroll.PaymentMethod,
                    month = request.Month,
                    year = request.Year,
                    processedDate = DateTime.UtcNow,
                }
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(
                500,
                new { message = "An error occurred while processing payroll", error = ex.Message }
            );
        }
    }

    [HttpGet("history/month/{month}/year/{year}")]
    public async Task<ActionResult<IEnumerable<PayrollHistory>>> GetPayrollHistoryByMonthYear(
        int month,
        int year
    )
    {
        if (month < 1 || month > 12)
            return BadRequest("Month must be between 1 and 12");

        if (year < 2020 || year > 2100)
            return BadRequest("Year must be between 2020 and 2100");

        var payrollHistory = await _context
            .PayrollHistories.Include(p => p.Employee)
            .Include(p => p.ProcessedBy)
            .Include(p => p.PayrollPayments)
            .Where(p => p.Month == month && p.Year == year)
            .OrderBy(p => p.Employee.OtherNames)
            .ThenBy(p => p.Employee.Surname)
            .ToListAsync();

        return Ok(payrollHistory);
    }

    [HttpGet("history/employee/{employeeId}/month/{month}/year/{year}")]
    public async Task<ActionResult<PayrollHistory>> GetEmployeePayrollHistoryByMonthYear(
        string employeeId,
        int month,
        int year
    )
    {
        if (month < 1 || month > 12)
            return BadRequest("Month must be between 1 and 12");

        if (year < 2020 || year > 2100)
            return BadRequest("Year must be between 2020 and 2100");

        var payrollHistory = await _context
            .PayrollHistories.Include(p => p.Employee)
            .Include(p => p.ProcessedBy)
            .Include(p => p.PayrollPayments)
            .FirstOrDefaultAsync(p =>
                p.EmployeeId == employeeId && p.Month == month && p.Year == year
            );

        if (payrollHistory == null)
            return NotFound(
                $"Payroll history not found for employee {employeeId} in {month}/{year}"
            );

        return Ok(payrollHistory);
    }

    [HttpGet("history/employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<PayrollHistory>>> GetAllEmployeePayrollHistory(
        string employeeId
    )
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
            return NotFound($"Employee with ID {employeeId} not found");

        var payrollHistory = await _context
            .PayrollHistories.Include(p => p.Employee)
            .Include(p => p.ProcessedBy)
            .Include(p => p.PayrollPayments)
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync();

        return Ok(payrollHistory);
    }

    [HttpGet("history/{id}")]
    public async Task<ActionResult<PayrollHistory>> GetPayrollHistory(int id)
    {
        var payrollHistory = await _context
            .PayrollHistories.Include(p => p.Employee)
            .Include(p => p.ProcessedBy)
            .Include(p => p.PayrollPayments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payrollHistory == null)
            return NotFound();

        return Ok(payrollHistory);
    }

    [HttpGet("history/status/{status}")]
    public async Task<ActionResult<IEnumerable<PayrollHistory>>> GetPayrollHistoryByStatus(
        PayrollHistoryStatus status
    )
    {
        var payrollHistory = await _context
            .PayrollHistories.Include(p => p.Employee)
            .Include(p => p.ProcessedBy)
            .Where(p => p.PaymentStatus == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(payrollHistory);
    }
}
