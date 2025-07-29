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

            var updatePayroll = await _context.Payrolls.FirstOrDefaultAsync(p =>
                p.EmployeeId == payroll.EmployeeId
            );

            if (updatePayroll == null)
            {
                return NotFound($"Created Payroll not found.");
            }

            try
            {
                await _payrollService.CreatePayrollAllowancesForEmployee(payroll.EmployeeId);
                await _payrollService.CreatePayrollDeductionsForEmployee(payroll.EmployeeId);

                var updateTotalAllowance = await _payrollService.CalculateTotalAllowancesAsync(
                    payroll.EmployeeId
                );
                var updateTotalDeduction = await _payrollService.CalculateTotalDeductionsAsync(
                    payroll.EmployeeId
                );

                updatePayroll.TotalAllowances = updateTotalAllowance;
                updatePayroll.TotalDeductions = updateTotalDeduction;

                updatePayroll.GrossSalary = updatePayroll.BaseSalary + updateTotalAllowance;
                updatePayroll.NetSalary = updatePayroll.GrossSalary - updateTotalDeduction;
                updatePayroll.LastModifiedBy = "System";
                updatePayroll.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
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

    [Authorize(Roles = "ADMIN")]
    [HttpPost("process-paysheet/{paySheetId}/")]
    public async Task<IActionResult> ProcessPaysheetPayroll(
        int paySheetId,
        [FromBody] PaysheetPayrollRequest request
    )
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var paySheet = await _context.PaySheets.FindAsync(paySheetId);
            if (paySheet == null)
            {
                return NotFound(new { message = $"PaySheet with ID {paySheetId} not found" });
            }

            var employees = await _context
                .Employees.Where(e =>
                    e.PaySheetId == paySheetId && e.Active == true && e.Deleted != true
                )
                .ToListAsync();

            if (!employees.Any())
            {
                return BadRequest(
                    new { message = $"No active employees found in PaySheet {paySheet.Name}" }
                );
            }

            var result = new PaysheetPayrollResult { TotalEmployees = employees.Count };

            var currentUserEmail =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "System";

            foreach (var employee in employees)
            {
                try
                {
                    var isAlreadyProcessed = await _payrollService.IsPayrollAlreadyProcessedAsync(
                        employee.Id,
                        request.ProcessMonth,
                        request.ProcessYear
                    );

                    if (isAlreadyProcessed)
                    {
                        result.Skipped++;
                        result.SkippedEmployees.Add(
                            $"Employee {employee.Email} - already processed for {request.ProcessMonth:D2}/{request.ProcessYear}"
                        );
                        continue;
                    }

                    var sourcePayrollHistory = await _context
                        .PayrollHistories.Include(ph => ph.Employee)
                        .FirstOrDefaultAsync(ph =>
                            ph.EmployeeId == employee.Id
                            && ph.Month == request.PreviousMonth
                            && ph.Year == request.PreviousYear
                        );

                    if (sourcePayrollHistory == null)
                    {
                        result.Skipped++;
                        result.SkippedEmployees.Add(
                            $"Employee {employee.Email} - no payroll history found for {request.PreviousMonth:D2}/{request.PreviousYear}"
                        );
                        continue;
                    }

                    var sourceAllowanceHistories = await _context
                        .PayrollAllowanceHistories.Where(pah =>
                            pah.EmployeeId == employee.Id
                            && pah.Month == request.PreviousMonth
                            && pah.Year == request.PreviousYear
                        )
                        .ToListAsync();

                    var sourceDeductionHistories = await _context
                        .PayrollDeductionHistories.Where(pdh =>
                            pdh.EmployeeId == employee.Id
                            && pdh.Month == request.PreviousMonth
                            && pdh.Year == request.PreviousYear
                        )
                        .ToListAsync();

                    var sourceVariantAllowances = await _context
                        .VariantPayrollAllowances.Include(vpa => vpa.VariantAllowance)
                        .Where(vpa => vpa.PayrollHistoryId == sourcePayrollHistory.Id)
                        .ToListAsync();

                    var sourceVariantDeductions = await _context
                        .VariantPayrollDeductions.Include(vpd => vpd.VariantDeduction)
                        .Where(vpd => vpd.PayrollHistoryId == sourcePayrollHistory.Id)
                        .ToListAsync();

                    var newPayrollHistory = new PayrollHistory
                    {
                        EmployeeId = employee.Id,
                        Month = request.ProcessMonth,
                        Year = request.ProcessYear,
                        BaseSalary = sourcePayrollHistory.BaseSalary,
                        HousingAllowance = sourcePayrollHistory.HousingAllowance,
                        TransportAllowance = sourcePayrollHistory.TransportAllowance,
                        AnnualTax = sourcePayrollHistory.AnnualTax,
                        TotalAllowances = sourcePayrollHistory.TotalAllowances,
                        TotalVariantAllowances = sourcePayrollHistory.TotalVariantAllowances,
                        TotalDeductions = sourcePayrollHistory.TotalDeductions,
                        TotalVariantDeductions = sourcePayrollHistory.TotalVariantDeductions,
                        GrossSalary = sourcePayrollHistory.GrossSalary,
                        NetSalary = sourcePayrollHistory.NetSalary,
                        PaymentStatus = PayrollHistoryStatus.Processing,
                        ProcessedByUserId = sourcePayrollHistory.ProcessedByUserId,
                        CreatedAt = DateTime.UtcNow,
                    };

                    _context.PayrollHistories.Add(newPayrollHistory);
                    await _context.SaveChangesAsync(); 

                    foreach (var sourceAllowance in sourceAllowanceHistories)
                    {
                        var newAllowanceHistory = new PayrollAllowanceHistory
                        {
                            EmployeeId = employee.Id,
                            Month = request.ProcessMonth,
                            Year = request.ProcessYear,
                            AllowanceName = sourceAllowance.AllowanceName,
                            Amount = sourceAllowance.Amount,
                            Description =
                                $"Duplicated from {request.PreviousMonth:D2}/{request.PreviousYear} - {sourceAllowance.Description}",
                            LastModifiedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                            CreatedAt = DateTime.UtcNow,
                        };

                        _context.PayrollAllowanceHistories.Add(newAllowanceHistory);
                    }

                    foreach (var sourceDeduction in sourceDeductionHistories)
                    {
                        var newDeductionHistory = new PayrollDeductionHistory
                        {
                            EmployeeId = employee.Id,
                            Month = request.ProcessMonth,
                            Year = request.ProcessYear,
                            DeductionName = sourceDeduction.DeductionName,
                            Amount = sourceDeduction.Amount,
                            Description =
                                $"Duplicated from {request.PreviousMonth:D2}/{request.PreviousYear} - {sourceDeduction.Description}",
                            LastModifiedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                            CreatedAt = DateTime.UtcNow,
                        };

                        _context.PayrollDeductionHistories.Add(newDeductionHistory);
                    }

                    foreach (var sourceVariantAllowance in sourceVariantAllowances)
                    {
                        var newVariantAllowance = new VariantPayrollAllowance
                        {
                            PayrollHistoryId = newPayrollHistory.Id,
                            VariantAllowanceId = sourceVariantAllowance.VariantAllowanceId,
                            Amount = sourceVariantAllowance.Amount,
                            GrantedBy = currentUserEmail,
                            CreatedAt = DateTime.UtcNow,
                        };

                        _context.VariantPayrollAllowances.Add(newVariantAllowance);
                    }

                    foreach (var sourceVariantDeduction in sourceVariantDeductions)
                    {
                        var newVariantDeduction = new VariantPayrollDeduction
                        {
                            PayrollHistoryId = newPayrollHistory.Id,
                            VariantDeductionId = sourceVariantDeduction.VariantDeductionId,
                            Amount = sourceVariantDeduction.Amount,
                            GrantedBy = currentUserEmail,
                            CreatedAt = DateTime.UtcNow,
                        };

                        _context.VariantPayrollDeductions.Add(newVariantDeduction);
                    }

                    // Create payroll payment record
                    var payrollPayment = new PayrollPayment
                    {
                        PayrollHistoryId = newPayrollHistory.Id,
                        EmployeeId = employee.Id,
                        PaymentMethod = "Bank Transfer",
                        PaymentStatus = PaymentStatus.Pending,
                        CreatedAt = DateTime.UtcNow,
                    };

                    _context.PayrollPayments.Add(payrollPayment);

                    result.SuccessfullyProcessed++;
                    result.ProcessedEmployees.Add(
                        $"Employee {employee.Email} - successfully processed for {request.ProcessMonth:D2}/{request.ProcessYear}"
                    );
                }
                catch (Exception ex)
                {
                    result.Skipped++;
                    result.SkippedEmployees.Add(
                        $"Employee {employee.Email} - error during processing: {ex.Message}"
                    );
                    continue;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            result.Message =
                $"Paysheet '{paySheet.Name}' payroll processing completed. "
                + $"Total: {result.TotalEmployees}, Processed: {result.SuccessfullyProcessed}, Skipped: {result.Skipped}";

            return Ok(
                new
                {
                    success = true,
                    paySheetId,
                    paySheetName = paySheet.Name,
                    sourceMonth = request.PreviousMonth,
                    sourceYear = request.PreviousYear,
                    targetMonth = request.ProcessMonth,
                    targetYear = request.ProcessYear,
                    processedBy = currentUserEmail,
                    processedAt = DateTime.UtcNow,
                    result,
                }
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "An error occurred while processing paysheet payroll",
                    error = ex.Message,
                }
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

        var response = payrollHistory.Select(ph => new
        {
            ph.Id,
            EmployeeId = ph.Employee.Id,
            Employee = new { ph.Employee.FullName, ph.Employee.Email },
            ph.Month,
            ph.Year,
            ph.BaseSalary,
            ph.HousingAllowance,
            ph.TransportAllowance,
            ph.AnnualTax,
            ph.TotalAllowances,
            ph.TotalDeductions,
            ph.GrossSalary,
            ph.NetSalary,
            ph.PaymentStatus,
            ph.PaidOn,
            ProcessedBy = ph.ProcessedBy?.Email,
            ph.CreatedAt,
        });

        return Ok(response);
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

        var payrollHistoryAllowances = await _context
            .PayrollAllowanceHistories.Where(pa =>
                pa.EmployeeId == payrollHistory.EmployeeId
                && pa.Month == payrollHistory.Month
                && pa.Year == payrollHistory.Year
            )
            .ToListAsync();

        var payrollHistoryDeductions = await _context
            .PayrollDeductionHistories.Where(pd =>
                pd.EmployeeId == payrollHistory.EmployeeId
                && pd.Month == payrollHistory.Month
                && pd.Year == payrollHistory.Year
            )
            .ToListAsync();

        var variantPayrollAllowances = await _context
            .VariantPayrollAllowances.Include(pa => pa.VariantAllowance)
            .Where(vpa => vpa.PayrollHistoryId == payrollHistory.Id)
            .ToListAsync();

        var variantPayrollDeductions = await _context
            .VariantPayrollDeductions.Include(pa => pa.VariantDeduction)
            .Where(vpd => vpd.PayrollHistoryId == payrollHistory.Id)
            .ToListAsync();

        var result = new
        {
            payrollHistory.Id,
            payrollHistory.EmployeeId,
            Employee = new { payrollHistory.Employee.FullName, payrollHistory.Employee.Email },
            payrollHistory.Month,
            payrollHistory.Year,
            payrollHistory.BaseSalary,
            payrollHistory.HousingAllowance,
            payrollHistory.TransportAllowance,
            payrollHistory.AnnualTax,
            payrollHistory.TotalAllowances,
            payrollHistory.TotalDeductions,
            payrollHistory.TotalVariantAllowances,
            payrollHistory.TotalVariantDeductions,
            payrollHistory.GrossSalary,
            payrollHistory.NetSalary,
            payrollHistory.PaymentStatus,
            payrollHistory.PaidOn,
            ProcessedBy = payrollHistory.ProcessedBy?.Email,
            payrollHistory.CreatedAt,
            PayrollPayment = payrollHistory
                .PayrollPayments.Select(pp => new
                {
                    pp.Id,
                    pp.PaymentMethod,
                    pp.PaymentStatus,
                    pp.TransactionId,
                    pp.PaymentDate,
                    pp.ProcessedByUserId,
                })
                .ToList(),
            PayrollAllowanceHistories = payrollHistoryAllowances
                .Select(pa => new
                {
                    pa.Id,
                    Name = pa.AllowanceName,
                    pa.Amount,
                    pa.Description,
                    pa.CreatedAt,
                })
                .ToList(),
            PayrollDeductionHistories = payrollHistoryDeductions
                .Select(pd => new
                {
                    pd.Id,
                    Name = pd.DeductionName,
                    pd.Amount,
                    pd.Description,
                    pd.CreatedAt,
                })
                .ToList(),
            VariantPayrollAllowance = variantPayrollAllowances.Select(vpa => new
            {
                vpa.Id,
                VariantAllowanceType = new { vpa.VariantAllowance.Id, vpa.VariantAllowance.Name },
                vpa.Amount,
                LastGrantedBy = vpa.GrantedBy,
                vpa.CreatedAt,
            }),
            VariantPayrollDeduction = variantPayrollDeductions.Select(vpd => new
            {
                vpd.Id,
                VariantDeductionType = new { vpd.VariantDeduction.Id, vpd.VariantDeduction.Name },
                vpd.Amount,
                LastGranted = vpd.GrantedBy,
                vpd.CreatedAt,
            }),
        };

        return Ok(result);
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
