using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
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
}
