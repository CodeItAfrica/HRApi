using HRApi.Data;
using HRApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoleController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return Ok(roles);
    }

    [HttpGet("get/user-roles")]
    public async Task<IActionResult> GetUserRoles()
    {
        var userRoles = await _context.UserRoles.ToListAsync();
        return Ok(userRoles);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] RoleNameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.RoleName))
        {
            return BadRequest("Role name cannot be empty.");
        }

        var normalizedRoleName = request.RoleName.Trim().ToLower();

        var existingRole = await _context.Roles.FirstOrDefaultAsync(r =>
            r.RoleName.ToLower() == normalizedRoleName
        );

        if (existingRole != null)
        {
            return Conflict($"The role '{request.RoleName}' already exists.");
        }

        var role = new Role { RoleName = request.RoleName, CreatedAt = DateTime.UtcNow };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleNameBodyRequest request)
    {
        if (string.IsNullOrEmpty(request.RoleName))
        {
            return BadRequest("Role name cannot be empty.");
        }

        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound("Role not found.");
        }

        var normalizedRoleName = request.RoleName.Trim().ToLower();
        var existingRole = await _context.Roles.FirstOrDefaultAsync(r =>
            r.RoleName.ToLower() == normalizedRoleName && r.Id != id
        );

        if (existingRole != null)
        {
            return Conflict($"The role '{request.RoleName}' already exists.");
        }

        role.RoleName = request.RoleName;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Role has been updated successfully." });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound("Role not found.");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Role has been deleted successfully." });
    }

    // Assign a role to a user
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var role = await _context.Roles.FindAsync(request.RoleId);
        if (role == null)
        {
            return NotFound("Role not found.");
        }

        var userRole = new UserRole
        {
            UserId = request.UserId,
            // UserEmail = user.Email,
            RoleId = request.RoleId,
            // RoleName = role.RoleName,
            AssignedAt = DateTime.UtcNow,
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Role has been assigned to the user successfully." });
    }

    // Remove a role from a user
    [HttpDelete("remove-role/{userId}/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur =>
            ur.UserId == userId && ur.RoleId == roleId
        );
        if (userRole == null)
        {
            return NotFound("User role not found.");
        }

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Role has been removed from the user successfully." });
    }

    [HttpGet("get-assigned-roles/{userId}")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var userWithRoles = await _context
            .UserRoles.Where(ur => ur.UserId == userId)
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Select(ur => new
            {
                UserId = ur.User.Id,
                UserEmail = ur.User.Email,
                ur.User.EmployeeId,
                ur.User.IsActive,
                RoleId = ur.Role.Id,
                ur.Role.RoleName,
                ur.AssignedAt,
            })
            .ToListAsync();

        if (userWithRoles == null || userWithRoles.Count == 0)
        {
            return NotFound("No roles found for this user.");
        }

        // Group the data to return user info once with all roles
        var result = new
        {
            userWithRoles.First().UserId,
            userWithRoles.First().UserEmail,
            userWithRoles.First().EmployeeId,
            userWithRoles.First().IsActive,
            Roles = userWithRoles
                .Select(ur => new
                {
                    id = ur.RoleId,
                    ur.RoleName,
                    ur.AssignedAt,
                })
                .ToList(),
        };

        return Ok(result);
    }

    [HttpGet("get-assigned-users/{roleId}")]
    public async Task<IActionResult> GetRoleUsers(int roleId)
    {
        var roleWithUsers = await _context
            .UserRoles.Where(ur => ur.RoleId == roleId)
            .Include(ur => ur.Role)
            .Include(ur => ur.User)
            .Select(ur => new
            {
                RoleId = ur.Role.Id,
                ur.Role.RoleName,
                UserId = ur.User.Id,
                UserEmail = ur.User.Email,
                ur.User.EmployeeId,
                ur.AssignedAt,
            })
            .ToListAsync();

        if (roleWithUsers == null || roleWithUsers.Count == 0)
        {
            return NotFound("No users found for this role.");
        }

        // Group the data to return role info once with all users
        var result = new
        {
            roleWithUsers.First().RoleId,
            roleWithUsers.First().RoleName,
            Users = roleWithUsers
                .Select(ru => new
                {
                    ru.UserId,
                    Email = ru.UserEmail,
                    ru.EmployeeId,
                    ru.AssignedAt,
                })
                .ToList(),
        };

        return Ok(result);
    }
}
