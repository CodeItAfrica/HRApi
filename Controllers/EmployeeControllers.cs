using HRApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly AppDbContext _context;
    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetEmployeeProfile(int id)
    {
        var employee = await _context
            .Users.Include(u => u.Employee)
            .ThenInclude(e => e.Branch)
            .Include(u => u.Employee)
            .ThenInclude(e => e.Department)
            .Include(u => u.Employee)
            .ThenInclude(e => e.Unit)
            .Include(u => u.Employee)
            .ThenInclude(e => e.Grade)
            .Include(u => u.Employee)
            .ThenInclude(e => e.EmploymentType)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null || employee.Employee == null)
            return NotFound("User or associated employee not found.");

        var roles = await _context.UserRoles
            .Where(r => r.UserId == id)
            .Select(r => r.Role.RoleName)
            .ToListAsync();

        var emp = employee.Employee;

        var response = new EmployeeResponse
        {
            EmployeeId = emp.Id,
            UserId = employee.Id.ToString(),
            StaffIdNo = emp.StaffIdNo,
            Title = emp.Title,
            Name = $"{emp.Surname} {emp.OtherNames}",
            Surname = emp.Surname,
            Email = emp.Email,
            Email2 = emp.Email2,
            Sex = emp.Sex,
            Address = emp.Address,
            State = emp.State,
            Country = emp.Country,
            BirthDate = emp.BirthDate,
            MaritalStatus = emp.MaritalStatus,
            StateOrigin = emp.StateOrigin,
            NationalIdNo = emp.NationalIdNo,
            AcctNo1 = emp.AcctNo1,
            AcctName1 = emp.AcctName1,
            AcctNo2 = emp.AcctNo2,
            AcctName2 = emp.AcctName2,
            BranchId = emp.BranchId,
            BranchName = emp.Branch?.BranchName,
            DeptId = emp.DeptId,
            DeptName = emp.Department?.DepartmentName,
            UnitId = emp.UnitId,
            UnitName = emp.Unit?.UnitName,
            GradeId = emp.GradeId,
            GradeName = emp.Grade?.GradeName,
            EmploymentTypeId = emp.EmploymentTypeId,
            EmploymentTypeName = emp.EmploymentType?.TypeName,
            HireDate = emp.HireDate,
            Telephone = emp.Telephone,
            MobilePhone = emp.MobilePhone,
            NextKin = emp.NextKin,
            KinAddress = emp.KinAddress,
            KinPhone = emp.KinPhone,
            KinRelationship = emp.KinRelationship,
            Height = emp.Height,
            Weight = emp.Weight,
            Smoker = emp.Smoker,
            DisableType = emp.DisableType,
            HmoName = emp.HmoName,
            HmoId = emp.HmoId,
            Remarks = emp.Remarks,
            Tag = emp.Tag,
            Photo = emp.Photo,
            PayFirstMonth = emp.PayFirstMonth,
            SheetId2 = emp.SheetId2,
            ConfirmStatus = emp.ConfirmStatus,
            ConfirmDuration = emp.ConfirmDuration,
            ConfirmationDate = emp.ConfirmationDate,
            RetiredDate = emp.RetiredDate,
            Deleted = emp.Deleted,
            Active = emp.Active,
            SubmitBy = emp.SubmitBy,
            SubmitOn = emp.SubmitOn,
            ModifiedBy = emp.ModifiedBy,
            ModifiedOn = emp.ModifiedOn,
            CreatedAt = emp.CreatedAt,
            Roles = roles,
        };

        return Ok(response);
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateEmployeeProfile(string id, [FromForm] RegisterRequest request, IFormFile? Photo)

    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound("Employee not found");
        }
        // Handle photo upload
        if (Photo != null && Photo.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Photo.CopyToAsync(stream);
            }

            employee.Photo = fileName;
        }
        employee.Email = request.Email;
        employee.Email2 = request.Email2;
        employee.Title = request.Title;
        employee.Surname = request.Surname;
        employee.OtherNames = request.OtherNames;
        employee.Telephone = request.Phone;
        employee.MobilePhone = request.MobilePhone;
        employee.Address = request.Address;
        employee.State = request.State;
        employee.Country = request.Country;
        employee.Sex = request.Sex;
        employee.MaritalStatus = request.MaritalStatus;
        employee.StateOrigin = request.StateOrigin;
        employee.NationalIdNo = request.NationalIdNo;
        employee.AcctNo1 = request.AcctNo1;
        employee.AcctName1 = request.AcctName1;
        employee.AcctNo2 = request.AcctNo2;
        employee.AcctName2 = request.AcctName2;
        employee.BranchId = request.BranchId;
        employee.DeptId = request.DeptId;
        employee.UnitId = request.UnitId;
        employee.GradeId = request.GradeId;
        employee.BirthDate = request.BirthDate;
        employee.HireDate = request.HireDate;
        employee.NextKin = request.NextKin;
        employee.KinAddress = request.KinAddress;
        employee.KinPhone = request.KinPhone;
        employee.KinRelationship = request.KinRelationship;
        employee.Height = request.Height;
        employee.Weight = request.Weight;
        employee.Smoker = request.Smoker;
        employee.DisableType = request.DisableType;
        employee.Remarks = request.Remarks;
        employee.Tag = request.Tag;
        employee.PayFirstMonth = request.PayFirstMonth;
        employee.SheetId2 = request.SheetId2;
        employee.ConfirmStatus = request.ConfirmStatus;
        employee.ConfirmDuration = request.ConfirmDuration;
        employee.ConfirmationDate = request.ConfirmationDate;
        employee.RetiredDate = request.RetiredDate;
        employee.Active = request.Active;
        employee.HmoName = request.HmoName;
        employee.HmoId = request.HmoId;
        employee.ModifiedBy = request.ModifiedBy;
        employee.ModifiedOn = DateTime.UtcNow;

        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(employee);
    }
}
