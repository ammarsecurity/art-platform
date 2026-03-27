using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService) => _courseService = courseService;

    private int? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim) : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ArtworkListRequest request)
    {
        var result = await _courseService.GetCoursesAsync(request, GetUserId());
        return Ok(new { success = true, data = result });
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int count = 4)
    {
        var result = await _courseService.GetFeaturedCoursesAsync(count);
        return Ok(new { success = true, data = result });
    }

    /// <summary>تفاصيل الدورة بالمعرّف (لوحة الإدارة — مسودة أو منشور)</summary>
    [HttpGet("by-id/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        return course == null ? NotFound(new { success = false, message = "الدورة غير موجودة" })
                              : Ok(new { success = true, data = course });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var course = await _courseService.GetCourseBySlugAsync(slug, GetUserId());
        return course == null ? NotFound(new { success = false, message = "الدورة غير موجودة" })
                              : Ok(new { success = true, data = course });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] CreateCourseRequest request, IFormFile? thumbnail)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var course = await _courseService.CreateCourseAsync(request, thumbnail);
        // Location يجب أن يكون ASCII فقط — الـ slug قد يحتوي عربية فيرفض Kestrel الترويسة
        var location = $"/api/courses/{Uri.EscapeDataString(course.Slug)}";
        return Created(location, new { success = true, data = course });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CreateCourseRequest request, IFormFile? thumbnail)
    {
        var course = await _courseService.UpdateCourseAsync(id, request, thumbnail);
        return course == null ? NotFound() : Ok(new { success = true, data = course });
    }

    /// <summary>تغيير حالة الدورة (مسودة / منشور / مؤرشف)</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetCourseStatusRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _courseService.SetCourseStatusAsync(id, request.Status);
        return ok ? Ok(new { success = true, message = "تم تحديث الحالة" })
                  : NotFound(new { success = false, message = "الدورة غير موجودة" });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _courseService.DeleteCourseAsync(id);
        return result ? Ok(new { success = true }) : NotFound();
    }

    [HttpPost("{courseId:int}/enroll")]
    [Authorize]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var userId = GetUserId()!.Value;
        await _courseService.EnrollAsync(courseId, userId);
        return Ok(new { success = true, message = "تم التسجيل في الدورة بنجاح" });
    }

    [HttpPost("progress")]
    [Authorize]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
    {
        var userId = GetUserId()!.Value;
        await _courseService.UpdateProgressAsync(userId, request);
        return Ok(new { success = true });
    }

    [HttpPost("lessons")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(524_288_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
    public async Task<IActionResult> AddLesson([FromForm] CreateLessonRequest request, IFormFile? video)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var hasFile = video != null && video.Length > 0;
        var hasUrl = !string.IsNullOrWhiteSpace(request.VideoUrl);
        if (!hasFile && !hasUrl)
            return BadRequest(new { success = false, message = "يرجى رفع ملف فيديو أو إدخال رابط (يوتيوب أو رابط مباشر)" });
        try
        {
            var lesson = await _courseService.AddLessonAsync(request, video);
            return Created("", new { success = true, data = lesson });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("lessons/{id:int}")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(524_288_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
    public async Task<IActionResult> UpdateLesson(int id, [FromForm] UpdateLessonRequest request, IFormFile? video)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var lesson = await _courseService.UpdateLessonAsync(id, request, video);
            return lesson == null
                ? NotFound(new { success = false, message = "الدرس غير موجود" })
                : Ok(new { success = true, data = lesson });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("lessons/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        var result = await _courseService.DeleteLessonAsync(id);
        return result ? Ok(new { success = true }) : NotFound();
    }
}
