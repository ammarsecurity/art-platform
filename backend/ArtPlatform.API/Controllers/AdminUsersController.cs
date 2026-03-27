using System.Security.Claims;
using ArtPlatform.Application.DTOs;
using ArtPlatform.Domain.Enums;
using ArtPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminUsersController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserListItemDto
            {
                Id = u.Id,
                Name = u.Name,
                Phone = u.Phone,
                Role = u.Role.ToString(),
                Bio = u.Bio,
                AvatarUrl = u.AvatarUrl,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(new { success = true, data = list });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdminUpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound(new { success = false, message = "المستخدم غير موجود" });

        var phoneNorm = request.Phone.Trim();
        if (await _context.Users.AnyAsync(u => u.Id != id && u.Phone == phoneNorm))
            return BadRequest(new { success = false, message = "رقم الهاتف مستخدم مسبقاً" });

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role) || !Enum.IsDefined(typeof(UserRole), role))
            return BadRequest(new { success = false, message = "الدور غير صالح (Admin أو Student)" });

        var adminCount = await _context.Users.CountAsync(u => u.Role == UserRole.Admin);

        if (user.Role == UserRole.Admin && role == UserRole.Student && adminCount <= 1)
            return BadRequest(new { success = false, message = "لا يمكن إزالة آخر مدير في النظام" });

        if (user.Role == UserRole.Admin && !request.IsActive && adminCount <= 1)
            return BadRequest(new { success = false, message = "لا يمكن تعطيل آخر مدير في النظام" });

        user.Name = request.Name.Trim();
        user.Phone = phoneNorm;
        user.Role = role;
        user.IsActive = request.IsActive;
        user.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio.Trim();

        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "تم تحديث المستخدم" });
    }

    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound(new { success = false, message = "المستخدم غير موجود" });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "تم تغيير كلمة المرور" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var actorId = TryGetCurrentUserId();
        if (actorId is null)
            return Unauthorized(new { success = false, message = "غير مصرّح" });

        if (id == actorId.Value)
            return BadRequest(new { success = false, message = "لا يمكن حذف حسابك الحالي" });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound(new { success = false, message = "المستخدم غير موجود" });

        if (user.Role == UserRole.Admin)
        {
            var adminCount = await _context.Users.CountAsync(u => u.Role == UserRole.Admin);
            if (adminCount <= 1)
                return BadRequest(new { success = false, message = "لا يمكن حذف آخر مدير في النظام" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "تم حذف المستخدم" });
    }

    private int? TryGetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var uid) ? uid : null;
    }
}
