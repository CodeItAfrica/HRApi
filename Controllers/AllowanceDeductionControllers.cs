using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api")]
public class AllowanceDeductionController : ControllerBase
{
    private readonly AppDbContext _context;

    public AllowanceDeductionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("allowanceList/get")]
    public async Task<IActionResult> GetAllowanceLists()
    {
        var allowanceLists = await _context
            .AllowanceLists.Select(al => new
            {
                Id = al.Id,
                Name = al.Name,
                Amount = al.Amount,
                CreatedAt = al.CreatedAt,
                LastModifiedAt = al.LastModifiedAt,
                GradeAssign = al.GradeAllowances.Select(ga => ga.GradeId).ToArray(),
            })
            .ToListAsync();

        return Ok(allowanceLists);
    }

    [HttpGet("allowanceList/details/{id}")]
    public async Task<IActionResult> GetAllowanceDetails(int id)
    {
        var allowanceList = await _context.AllowanceLists.FindAsync(id);
        if (allowanceList == null)
        {
            return NotFound("AllowanceList not found.");
        }

        var assignedGrades = await _context
            .GradeAllowances.Where(ga => ga.AllowanceListId == id)
            .Include(ga => ga.Grade)
            .Select(ga => new
            {
                ga.GradeId,
                ga.Grade.GradeName,
                ga.Grade.BaseSalary,
                ga.Grade.HousingAllowance,
                ga.Grade.TransportAllowance,
                ga.AssignedAt,
            })
            .ToListAsync();

        var totalEmployeesWithAllowance = await _context
            .PayrollAllowances.Where(pa => pa.AllowanceListId == id)
            .CountAsync();

        var employeesByGrade = await _context
            .PayrollAllowances.Where(pa => pa.AllowanceListId == id)
            .Include(pa => pa.Employee)
            .ThenInclude(e => e.Grade)
            .GroupBy(pa => new { pa.Employee.GradeId, pa.Employee.Grade.GradeName })
            .Select(g => new
            {
                g.Key.GradeId,
                g.Key.GradeName,
                EmployeeCount = g.Count(),
            })
            .ToListAsync();

        return Ok(
            new
            {
                AllowanceDetails = new
                {
                    allowanceList.Id,
                    allowanceList.Name,
                    allowanceList.Amount,
                    allowanceList.CreatedAt,
                    allowanceList.LastModifiedAt,
                },
                AssignedGrades = assignedGrades,
                TotalEmployeesWithAllowance = totalEmployeesWithAllowance,
                EmployeesByGrade = employeesByGrade,
                Summary = new
                {
                    TotalGradesAssigned = assignedGrades.Count,
                    TotalEmployeesAffected = totalEmployeesWithAllowance,
                },
            }
        );
    }

    [HttpPost("allowanceList/create")]
    public async Task<IActionResult> CreateAllowanceList(
        [FromBody] CreateAllowanceDeductionBodyRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Allowance List name cannot be empty.");
        }

        int[] gradesToAssign;
        if (request.GradeAssign == null || request.GradeAssign.Length == 0)
        {
            gradesToAssign = await _context.Grades.Select(g => g.Id).ToArrayAsync();

            if (gradesToAssign.Length == 0)
            {
                return BadRequest("No grades exist in the system to assign the allowance to.");
            }
        }
        else
        {
            gradesToAssign = request.GradeAssign;
        }

        var normalizedAllowanceListName = request.Name.Trim().ToLower();

        var existingAllowanceList = await _context.AllowanceLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedAllowanceListName
        );

        if (existingAllowanceList != null)
        {
            return Conflict($"The allowanceList '{request.Name}' already exists.");
        }

        var allowanceList = new AllowanceList { Name = request.Name, Amount = request.Amount };

        _context.AllowanceLists.Add(allowanceList);
        await _context.SaveChangesAsync();

        var gradeAllowances = new List<GradeAllowance>();
        foreach (var gradeId in request.GradeAssign)
        {
            var gradeAllowance = new GradeAllowance
            {
                GradeId = gradeId,
                AllowanceListId = allowanceList.Id,
                AssignedAt = DateTime.UtcNow,
            };
            gradeAllowances.Add(gradeAllowance);
        }

        if (gradeAllowances.Any())
        {
            await _context.GradeAllowances.AddRangeAsync(gradeAllowances);
            await _context.SaveChangesAsync();
        }

        var employeesInAssignedGrades = await _context
            .Employees.Where(e => request.GradeAssign.Contains(e.GradeId.Value))
            .Select(e => e.Id)
            .ToListAsync();

        var payrollAllowances = new List<PayrollAllowance>();

        foreach (var employeeId in employeesInAssignedGrades)
        {
            var payrollAllowance = new PayrollAllowance
            {
                EmployeeId = employeeId,
                AllowanceListId = allowanceList.Id,
                Amount = request.Amount,
                Description = $"Auto-created for new allowance type: {allowanceList.Name}",
                LastGrantedBy = "System",
                LastGrantedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow,
            };

            payrollAllowances.Add(payrollAllowance);
        }

        if (payrollAllowances.Any())
        {
            await _context.PayrollAllowances.AddRangeAsync(payrollAllowances);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(
            nameof(GetAllowanceLists),
            new { id = allowanceList.Id },
            new
            {
                allowanceList = allowanceList,
                assignedGrades = request.GradeAssign,
                employeesAffected = employeesInAssignedGrades.Count,
            }
        );
    }

    [HttpPut("allowanceList/update/{id}")]
    public async Task<IActionResult> UpdateAllowanceList(
        int id,
        [FromBody] CreateAllowanceDeductionBodyRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("AllowanceList name cannot be empty.");
        }

        if (request.GradeAssign == null || request.GradeAssign.Length == 0)
        {
            return BadRequest("At least one grade must be assigned.");
        }

        var allowanceList = await _context.AllowanceLists.FindAsync(id);
        if (allowanceList == null)
        {
            return NotFound("AllowanceList not found.");
        }

        var normalizedAllowanceListName = request.Name.Trim().ToLower();
        var existingAllowanceList = await _context.AllowanceLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedAllowanceListName && r.Id != id
        );

        if (existingAllowanceList != null)
        {
            return Conflict($"The allowanceList '{request.Name}' already exists.");
        }

        allowanceList.Name = request.Name;
        allowanceList.Amount = request.Amount;
        allowanceList.LastModifiedAt = DateTime.UtcNow;

        var currentGradeAllowances = await _context
            .GradeAllowances.Where(ga => ga.AllowanceListId == id)
            .ToListAsync();

        var currentGradeIds = currentGradeAllowances.Select(ga => ga.GradeId).ToList();

        var gradesToRemove = currentGradeIds.Except(request.GradeAssign).ToList();

        var gradesToAdd = request.GradeAssign.Except(currentGradeIds).ToList();

        if (gradesToRemove.Any())
        {
            var gradeAllowancesToRemove = currentGradeAllowances
                .Where(ga => gradesToRemove.Contains(ga.GradeId))
                .ToList();

            _context.GradeAllowances.RemoveRange(gradeAllowancesToRemove);
        }

        if (gradesToAdd.Any())
        {
            var newGradeAllowances = gradesToAdd
                .Select(gradeId => new GradeAllowance
                {
                    GradeId = gradeId,
                    AllowanceListId = id,
                    AssignedAt = DateTime.UtcNow,
                })
                .ToList();

            await _context.GradeAllowances.AddRangeAsync(newGradeAllowances);
        }

        var employeesInRemovedGrades = await _context
            .Employees.Where(e => gradesToRemove.Contains(e.GradeId.Value))
            .Select(e => e.Id)
            .ToListAsync();

        if (employeesInRemovedGrades.Any())
        {
            var payrollAllowancesToRemove = await _context
                .PayrollAllowances.Where(pa =>
                    pa.AllowanceListId == id && employeesInRemovedGrades.Contains(pa.EmployeeId)
                )
                .ToListAsync();

            _context.PayrollAllowances.RemoveRange(payrollAllowancesToRemove);
        }

        var employeesInNewGrades = await _context
            .Employees.Where(e => gradesToAdd.Contains(e.GradeId.Value))
            .Select(e => e.Id)
            .ToListAsync();

        if (employeesInNewGrades.Any())
        {
            var newPayrollAllowances = employeesInNewGrades
                .Select(employeeId => new PayrollAllowance
                {
                    EmployeeId = employeeId,
                    AllowanceListId = id,
                    Amount = request.Amount,
                    Description = $"Auto-created for updated allowance type: {allowanceList.Name}",
                    LastGrantedBy = "System",
                    LastGrantedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                    CreatedAt = DateTime.UtcNow,
                })
                .ToList();

            await _context.PayrollAllowances.AddRangeAsync(newPayrollAllowances);
        }

        // Update existing PayrollAllowance amounts for employees in grades that remain assigned
        // var remainingGrades = currentGradeIds.Intersect(request.GradeAssign).ToList();
        // if (remainingGrades.Any())
        // {
        //     var employeesInRemainingGrades = await _context
        //         .Employees.Where(e => remainingGrades.Contains(e.GradeId.Value))
        //         .Select(e => e.Id)
        //         .ToListAsync();

        //     var existingPayrollAllowances = await _context
        //         .PayrollAllowances.Where(pa =>
        //             pa.AllowanceListId == id && employeesInRemainingGrades.Contains(pa.EmployeeId)
        //         )
        //         .ToListAsync();

        //     foreach (var payrollAllowance in existingPayrollAllowances)
        //     {
        //         payrollAllowance.Amount = request.Amount;
        //         payrollAllowance.Description = $"Updated allowance type: {allowanceList.Name}";
        //         payrollAllowance.LastGrantedBy = "System";
        //         payrollAllowance.LastGrantedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        //     }
        // }

        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                message = "AllowanceList has been updated successfully.",
                allowanceList,
                gradesRemoved = gradesToRemove.Count,
                gradesAdded = gradesToAdd.Count,
                employeesRemoved = employeesInRemovedGrades.Count,
                employeesAdded = employeesInNewGrades.Count,
            }
        );
    }

    [HttpDelete("allowanceList/delete/{id}")]
    public async Task<IActionResult> DeleteAllowanceList(int id)
    {
        var allowanceList = await _context.AllowanceLists.FindAsync(id);
        if (allowanceList == null)
        {
            return NotFound("AllowanceList not found.");
        }

        var payrollAllowances = await _context
            .PayrollAllowances.Where(pa => pa.AllowanceListId == id)
            .ToListAsync();

        if (payrollAllowances.Any())
        {
            _context.PayrollAllowances.RemoveRange(payrollAllowances);
        }

        var gradeAllowances = await _context
            .GradeAllowances.Where(ga => ga.AllowanceListId == id)
            .ToListAsync();

        if (gradeAllowances.Any())
        {
            _context.GradeAllowances.RemoveRange(gradeAllowances);
        }

        _context.AllowanceLists.Remove(allowanceList);
        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                message = "AllowanceList has been deleted successfully.",
                deletedPayrollAllowances = payrollAllowances.Count,
                deletedGradeAllowances = gradeAllowances.Count,
            }
        );
    }

    [HttpGet("deductionList/get")]
    public async Task<IActionResult> GetDeductionLists()
    {
        var deductionLists = await _context
            .DeductionLists.Select(dl => new
            {
                Id = dl.Id,
                Name = dl.Name,
                Amount = dl.Amount,
                CreatedAt = dl.CreatedAt,
                LastModifiedAt = dl.LastModifiedAt,
                GradeAssign = dl.GradeDeductions.Select(gd => gd.GradeId).ToArray(),
            })
            .ToListAsync();

        return Ok(deductionLists);
    }

    [HttpGet("deductionList/details/{id}")]
    public async Task<IActionResult> GetDeductionDetails(int id)
    {
        var deductionList = await _context.DeductionLists.FindAsync(id);
        if (deductionList == null)
        {
            return NotFound("DeductionList not found.");
        }

        var assignedGrades = await _context
            .GradeDeductions.Where(gd => gd.DeductionListId == id)
            .Include(gd => gd.Grade)
            .Select(gd => new
            {
                gd.GradeId,
                gd.Grade.GradeName,
                gd.Grade.BaseSalary,
                gd.Grade.HousingAllowance,
                gd.Grade.TransportAllowance,
                gd.AssignedAt,
            })
            .ToListAsync();

        var totalEmployeesWithDeduction = await _context
            .PayrollDeductions.Where(pd => pd.DeductionListId == id)
            .CountAsync();

        var employeesByGrade = await _context
            .PayrollDeductions.Where(pd => pd.DeductionListId == id)
            .Include(pd => pd.Employee)
            .ThenInclude(e => e.Grade)
            .GroupBy(pd => new { pd.Employee.GradeId, pd.Employee.Grade.GradeName })
            .Select(g => new
            {
                g.Key.GradeId,
                g.Key.GradeName,
                EmployeeCount = g.Count(),
            })
            .ToListAsync();

        return Ok(
            new
            {
                DeductionDetails = new
                {
                    deductionList.Id,
                    deductionList.Name,
                    deductionList.Amount,
                    deductionList.CreatedAt,
                    deductionList.LastModifiedAt,
                },
                AssignedGrades = assignedGrades,
                TotalEmployeesWithDeduction = totalEmployeesWithDeduction,
                EmployeesByGrade = employeesByGrade,
                Summary = new
                {
                    TotalGradesAssigned = assignedGrades.Count,
                    TotalEmployeesAffected = totalEmployeesWithDeduction,
                },
            }
        );
    }

    [HttpPost("deductionList/create")]
    public async Task<IActionResult> CreateDeductionList(
        [FromBody] CreateAllowanceDeductionBodyRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Deduction List name cannot be empty.");
        }

        // If no grades are assigned, get all grades in the system
        int[] gradesToAssign;
        if (request.GradeAssign == null || request.GradeAssign.Length == 0)
        {
            gradesToAssign = await _context.Grades.Select(g => g.Id).ToArrayAsync();

            if (gradesToAssign.Length == 0)
            {
                return BadRequest("No grades exist in the system to assign the deduction to.");
            }
        }
        else
        {
            gradesToAssign = request.GradeAssign;
        }

        var normalizedDeductionListName = request.Name.Trim().ToLower();

        var existingDeductionList = await _context.DeductionLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedDeductionListName
        );

        if (existingDeductionList != null)
        {
            return Conflict($"The deductionList '{request.Name}' already exists.");
        }

        // Create the deduction list
        var deductionList = new DeductionList { Name = request.Name, Amount = request.Amount };

        _context.DeductionLists.Add(deductionList);
        await _context.SaveChangesAsync();

        // Create GradeDeduction entries for the assigned grades
        var gradeDeductions = new List<GradeDeduction>();
        foreach (var gradeId in gradesToAssign)
        {
            var gradeDeduction = new GradeDeduction
            {
                GradeId = gradeId,
                DeductionListId = deductionList.Id,
                AssignedAt = DateTime.UtcNow,
            };
            gradeDeductions.Add(gradeDeduction);
        }

        if (gradeDeductions.Any())
        {
            await _context.GradeDeductions.AddRangeAsync(gradeDeductions);
            await _context.SaveChangesAsync();
        }

        var employeesInAssignedGrades = await _context
            .Employees.Where(e => gradesToAssign.Contains(e.GradeId.Value))
            .Select(e => e.Id)
            .ToListAsync();

        var payrollDeductions = new List<PayrollDeduction>();

        foreach (var employeeId in employeesInAssignedGrades)
        {
            var payrollDeduction = new PayrollDeduction
            {
                EmployeeId = employeeId,
                DeductionListId = deductionList.Id,
                Amount = request.Amount,
                Description = $"Auto-created for new deduction type: {deductionList.Name}",
                LastDeductedBy = "System",
                LastDeductedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow,
            };

            payrollDeductions.Add(payrollDeduction);
        }

        if (payrollDeductions.Any())
        {
            await _context.PayrollDeductions.AddRangeAsync(payrollDeductions);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(
            nameof(GetDeductionLists),
            new { id = deductionList.Id },
            new
            {
                deductionList = deductionList,
                assignedGrades = gradesToAssign,
                employeesAffected = employeesInAssignedGrades.Count,
            }
        );
    }

    [HttpPut("deductionList/update/{id}")]
    public async Task<IActionResult> UpdateDeductionList(
        int id,
        [FromBody] CreateAllowanceDeductionBodyRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("DeductionList name cannot be empty.");
        }

        if (request.GradeAssign == null || request.GradeAssign.Length == 0)
        {
            return BadRequest("At least one grade must be assigned.");
        }

        var deductionList = await _context.DeductionLists.FindAsync(id);
        if (deductionList == null)
        {
            return NotFound("DeductionList not found.");
        }

        var normalizedDeductionListName = request.Name.Trim().ToLower();
        var existingDeductionList = await _context.DeductionLists.FirstOrDefaultAsync(r =>
            r.Name.ToLower() == normalizedDeductionListName && r.Id != id
        );

        if (existingDeductionList != null)
        {
            return Conflict($"The deductionList '{request.Name}' already exists.");
        }

        deductionList.Name = request.Name;
        deductionList.Amount = request.Amount;
        deductionList.LastModifiedAt = DateTime.UtcNow;

        var currentGradeDeductions = await _context
            .GradeDeductions.Where(gd => gd.DeductionListId == id)
            .ToListAsync();

        var currentGradeIds = currentGradeDeductions.Select(gd => gd.GradeId).ToList();

        var gradesToRemove = currentGradeIds.Except(request.GradeAssign).ToList();
        var gradesToAdd = request.GradeAssign.Except(currentGradeIds).ToList();

        if (gradesToRemove.Any())
        {
            var gradeDeductionsToRemove = currentGradeDeductions
                .Where(gd => gradesToRemove.Contains(gd.GradeId))
                .ToList();

            _context.GradeDeductions.RemoveRange(gradeDeductionsToRemove);
        }

        if (gradesToAdd.Any())
        {
            var newGradeDeductions = gradesToAdd
                .Select(gradeId => new GradeDeduction
                {
                    GradeId = gradeId,
                    DeductionListId = id,
                    AssignedAt = DateTime.UtcNow,
                })
                .ToList();

            await _context.GradeDeductions.AddRangeAsync(newGradeDeductions);
        }

        var employeesInRemovedGrades = await _context
            .Employees.Where(e => gradesToRemove.Contains(e.GradeId.Value))
            .Select(e => e.Id)
            .ToListAsync();

        if (employeesInRemovedGrades.Any())
        {
            var payrollDeductionsToRemove = await _context
                .PayrollDeductions.Where(pd =>
                    pd.DeductionListId == id && employeesInRemovedGrades.Contains(pd.EmployeeId)
                )
                .ToListAsync();

            _context.PayrollDeductions.RemoveRange(payrollDeductionsToRemove);
        }

        var employeesInNewGrades = await _context
            .Employees.Where(e => gradesToAdd.Contains(e.GradeId.Value))
            .Select(e => e.Id)
            .ToListAsync();

        if (employeesInNewGrades.Any())
        {
            var newPayrollDeductions = employeesInNewGrades
                .Select(employeeId => new PayrollDeduction
                {
                    EmployeeId = employeeId,
                    DeductionListId = id,
                    Amount = request.Amount,
                    Description = $"Auto-created for updated deduction type: {deductionList.Name}",
                    LastDeductedBy = "System",
                    LastDeductedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                    CreatedAt = DateTime.UtcNow,
                })
                .ToList();

            await _context.PayrollDeductions.AddRangeAsync(newPayrollDeductions);
        }

        // Update existing PayrollDeduction amounts for employees in grades that remain assigned
        // var remainingGrades = currentGradeIds.Intersect(request.GradeAssign).ToList();
        // if (remainingGrades.Any())
        // {
        //     var employeesInRemainingGrades = await _context
        //         .Employees.Where(e => remainingGrades.Contains(e.GradeId.Value))
        //         .Select(e => e.Id)
        //         .ToListAsync();

        //     var existingPayrollDeductions = await _context
        //         .PayrollDeductions.Where(pd =>
        //             pd.DeductionListId == id && employeesInRemainingGrades.Contains(pd.EmployeeId)
        //         )
        //         .ToListAsync();

        //     foreach (var payrollDeduction in existingPayrollDeductions)
        //     {
        //         payrollDeduction.Amount = request.Amount;
        //         payrollDeduction.Description = $"Updated deduction type: {deductionList.Name}";
        //         payrollDeduction.LastDeductedBy = "System";
        //         payrollDeduction.LastDeductedOn = DateOnly.FromDateTime(DateTime.UtcNow);
        //     }
        // }

        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                message = "DeductionList has been updated successfully.",
                deductionList,
                gradesRemoved = gradesToRemove.Count,
                gradesAdded = gradesToAdd.Count,
                employeesRemoved = employeesInRemovedGrades.Count,
                employeesAdded = employeesInNewGrades.Count,
            }
        );
    }

    [HttpDelete("deductionList/delete/{id}")]
    public async Task<IActionResult> DeleteDeductionList(int id)
    {
        var deductionList = await _context.DeductionLists.FindAsync(id);
        if (deductionList == null)
        {
            return NotFound("DeductionList not found.");
        }

        var payrollDeductions = await _context
            .PayrollDeductions.Where(pd => pd.DeductionListId == id)
            .ToListAsync();

        if (payrollDeductions.Any())
        {
            _context.PayrollDeductions.RemoveRange(payrollDeductions);
        }

        var gradeDeductions = await _context
            .GradeDeductions.Where(gd => gd.DeductionListId == id)
            .ToListAsync();

        if (gradeDeductions.Any())
        {
            _context.GradeDeductions.RemoveRange(gradeDeductions);
        }

        _context.DeductionLists.Remove(deductionList);
        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                message = "DeductionList has been deleted successfully.",
                deletedPayrollDeductions = payrollDeductions.Count,
                deletedGradeDeductions = gradeDeductions.Count,
            }
        );
    }
}
