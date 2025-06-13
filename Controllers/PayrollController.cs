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
                .Payrolls.OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(payrolls);
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
                .Include(p => p.PayrollDeductions)
                .Include(p => p.PayrollAllowances)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payroll == null)
            {
                return NotFound(new { message = $"Payroll with ID {id} not found" });
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

            // Create history records (this will also remove current allowances/deductions)
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

            // Save all changes within the transaction
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // After transaction commits successfully, recreate the allowances and deductions
            // This is done outside the transaction to avoid conflicts
            try
            {
                await _payrollService.CreatePayrollAllowancesForEmployee(payroll.EmployeeId);
                await _payrollService.CreatePayrollDeductionsForEmployee(payroll.EmployeeId);
            }
            catch (Exception ex)
            {
                // Log this error but don't fail the entire operation
                // The payroll processing was successful, but recreation of allowances/deductions failed
                // This can be handled separately or logged for manual intervention
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
}
