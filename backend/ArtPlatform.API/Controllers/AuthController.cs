using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>تسجيل الدخول</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.LoginAsync(request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>إنشاء حساب جديد</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.RegisterAsync(request);
        return CreatedAtAction(nameof(Me), new { success = true, data = result });
    }

    /// <summary>بيانات المستخدم الحالي</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _authService.GetCurrentUserAsync(userId);
        return user == null ? NotFound() : Ok(new { success = true, data = user });
    }

    /// <summary>تحديث الملف الشخصي</summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _authService.UpdateProfileAsync(userId, request);
        return Ok(new { success = true, data = user, message = "تم تحديث الملف الشخصي بنجاح" });
    }

    /// <summary>الدورات المسجّل فيها</summary>
    [HttpGet("my-courses")]
    [Authorize]
    public async Task<IActionResult> MyCourses()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var courses = await _authService.GetMyCoursesAsync(userId);
        return Ok(new { success = true, data = courses });
    }
}
