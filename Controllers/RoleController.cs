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

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            return BadRequest("Role name cannot be empty.");
        }

        var role = new Role
        {
            RoleName = roleName,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            return BadRequest("Role name cannot be empty.");
        }

        var role = await _context.Roles.FindAsync(id);
        if (role == null)
        {
            return NotFound("Role not found.");
        }

        role.RoleName = roleName;
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
            UserEmail = user.Email,
            RoleId = request.RoleId,
            RoleName = role.RoleName,
            AssignedAt = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Role has been assigned to the user successfully." });
    }

    // Remove a role from a user
    [HttpDelete("remove-role/{userId}/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
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
        var userRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
        if (userRoles == null || userRoles.Count == 0)
        {
            return NotFound("No roles found for this user.");
        }

        return Ok(userRoles);
    }

    [HttpGet("get-assigned-users/{roleId}")]
    public async Task<IActionResult> GetRoleUsers(int roleId)
    {
        var roleUsers = await _context.UserRoles.Where(ur => ur.RoleId == roleId).ToListAsync();
        if (roleUsers == null || roleUsers.Count == 0)
        {
            return NotFound("No users found for this role.");
        }

        return Ok(roleUsers);
    }
}