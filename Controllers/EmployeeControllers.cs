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
    public async Task<IActionResult> GetEmployees()
    {
        var users = await _context
            .Users.Include(u => u.Employee)
            .ThenInclude(u => u.Branch)
            .Include(u => u.Employee)
            .ThenInclude(u => u.Grade)
            .Select(static u => new
            {
                u.Id,
                u.EmployeeId,
                u.Email,
                Surname = u.Employee != null ? (u.Employee.Surname).Trim() : null,
                OtherNames = u.Employee != null && u.Employee.OtherNames != null
                    ? u.Employee.OtherNames.Trim()
                    : null,
                Grade = u.Employee != null && u.Employee.Grade != null
                    ? u.Employee.Grade.GradeName
                    : null,
                Branch = u.Employee != null
                && u.Employee.Branch != null
                && u.Employee.Branch.BranchName != null
                    ? u.Employee.Branch.BranchName
                    : null,
                HiredDate = u.Employee != null ? u.Employee.HireDate : null,
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("get/{id}")]
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
            .ThenInclude(e => e.PaySheet)
            .Include(u => u.Employee)
            .ThenInclude(e => e.EmploymentType)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null || employee.Employee == null)
            return NotFound("User or associated employee not found.");

        var roles = await _context
            .UserRoles.Where(r => r.UserId == id)
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
            BankName1 = emp.BankName1,
            AcctNo2 = emp.AcctNo2,
            AcctName2 = emp.AcctName2,
            BankName2 = emp.BankName2,
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
            PaySheetId = emp.PaySheetId,
            PaySheetName = emp.PaySheet?.Name,
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
    public async Task<IActionResult> UpdateEmployeeProfile(
        string id,
        [FromForm] UpdateEmployeeRequest request,
        IFormFile? Photo
    )
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

        if (request.Email != null)
            employee.Email = request.Email;
        if (request.Email2 != null)
            employee.Email2 = request.Email2;
        if (request.Title != null)
            employee.Title = request.Title;
        if (request.Surname != null)
            employee.Surname = request.Surname;
        if (request.OtherNames != null)
            employee.OtherNames = request.OtherNames;
        if (request.Phone != null)
            employee.Telephone = request.Phone;
        if (request.MobilePhone != null)
            employee.MobilePhone = request.MobilePhone;
        if (request.Address != null)
            employee.Address = request.Address;
        if (request.State != null)
            employee.State = request.State;
        if (request.Country != null)
            employee.Country = request.Country;
        if (request.Sex != null)
            employee.Sex = request.Sex;
        if (request.MaritalStatus != null)
            employee.MaritalStatus = request.MaritalStatus;
        if (request.StateOrigin != null)
            employee.StateOrigin = request.StateOrigin;
        if (request.NationalIdNo != null)
            employee.NationalIdNo = request.NationalIdNo;
        if (request.AcctNo1 != null)
            employee.AcctNo1 = request.AcctNo1;
        if (request.AcctName1 != null)
            employee.AcctName1 = request.AcctName1;
        if (request.AcctNo2 != null)
            employee.AcctNo2 = request.AcctNo2;
        if (request.AcctName2 != null)
            employee.AcctName2 = request.AcctName2;
        if (request.BranchId.HasValue)
            employee.BranchId = request.BranchId.Value;
        if (request.DeptId.HasValue)
            employee.DeptId = request.DeptId.Value;
        if (request.UnitId.HasValue)
            employee.UnitId = request.UnitId.Value;
        if (request.GradeId.HasValue)
            employee.GradeId = request.GradeId.Value;
        if (request.BirthDate.HasValue)
            employee.BirthDate = request.BirthDate.Value;
        if (request.HireDate.HasValue)
            employee.HireDate = request.HireDate.Value;
        if (request.NextKin != null)
            employee.NextKin = request.NextKin;
        if (request.KinAddress != null)
            employee.KinAddress = request.KinAddress;
        if (request.KinPhone != null)
            employee.KinPhone = request.KinPhone;
        if (request.KinRelationship != null)
            employee.KinRelationship = request.KinRelationship;
        if (request.Height.HasValue)
            employee.Height = request.Height.Value;
        if (request.Weight.HasValue)
            employee.Weight = request.Weight.Value;
        if (request.Smoker.HasValue)
            employee.Smoker = request.Smoker.Value;
        if (request.DisableType != null)
            employee.DisableType = request.DisableType;
        if (request.Remarks != null)
            employee.Remarks = request.Remarks;
        if (request.Tag != null)
            employee.Tag = request.Tag;
        if (request.PayFirstMonth.HasValue)
            employee.PayFirstMonth = request.PayFirstMonth.Value;
        if (request.PaySheetId.HasValue)
            employee.PaySheetId = request.PaySheetId.Value;
        if (request.ConfirmStatus != null)
            employee.ConfirmStatus = request.ConfirmStatus;
        if (request.ConfirmDuration.HasValue)
            employee.ConfirmDuration = request.ConfirmDuration.Value;
        if (request.ConfirmationDate.HasValue)
            employee.ConfirmationDate = request.ConfirmationDate.Value;
        if (request.RetiredDate.HasValue)
            employee.RetiredDate = request.RetiredDate.Value;
        if (request.Active.HasValue)
            employee.Active = request.Active.Value;
        if (request.HmoName != null)
            employee.HmoName = request.HmoName;
        if (request.HmoId != null)
            employee.HmoId = request.HmoId;
        if (request.ModifiedBy != null)
            employee.ModifiedBy = request.ModifiedBy;

        employee.ModifiedOn = DateTime.UtcNow;

        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // Get the User ID associated with this employee to retrieve the full profile
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == id);
        if (user == null)
        {
            return NotFound("Associated user not found");
        }

        // Return the complete employee profile using the GetEmployeeProfile logic
        var updatedEmployee = await _context
            .Users.Include(u => u.Employee)
            .ThenInclude(e => e.Branch)
            .Include(u => u.Employee)
            .ThenInclude(e => e.Department)
            .Include(u => u.Employee)
            .ThenInclude(e => e.Unit)
            .Include(u => u.Employee)
            .ThenInclude(e => e.Grade)
            .Include(u => u.Employee)
            .ThenInclude(e => e.PaySheet)
            .Include(u => u.Employee)
            .ThenInclude(e => e.EmploymentType)
            .FirstOrDefaultAsync(x => x.Id == user.Id);

        if (updatedEmployee == null || updatedEmployee.Employee == null)
            return NotFound("Updated employee profile not found.");

        var roles = await _context
            .UserRoles.Where(r => r.UserId == user.Id)
            .Select(r => r.Role.RoleName)
            .ToListAsync();

        var emp = updatedEmployee.Employee;

        var response = new EmployeeResponse
        {
            EmployeeId = emp.Id,
            UserId = updatedEmployee.Id.ToString(),
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
            PaySheetId = emp.PaySheetId,
            PaySheetName = emp.PaySheet?.Name,
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
}
