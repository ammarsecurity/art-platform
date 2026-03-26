using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService) => _authService = authService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var result = await _authService.GetUsersAsync(page, pageSize, search);
        return Ok(new { success = true, data = result });
    }

    [HttpPut("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleRequest request)
    {
        var user = await _authService.UpdateUserRoleAsync(id, request.Role);
        return user == null ? NotFound(new { success = false, message = "المستخدم غير موجود" })
                            : Ok(new { success = true, data = user, message = "تم تحديث الدور" });
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var user = await _authService.ToggleUserStatusAsync(id);
        return user == null ? NotFound(new { success = false, message = "المستخدم غير موجود" })
                            : Ok(new { success = true, data = user, message = user.IsActive ? "تم تفعيل الحساب" : "تم تعطيل الحساب" });
    }
}
