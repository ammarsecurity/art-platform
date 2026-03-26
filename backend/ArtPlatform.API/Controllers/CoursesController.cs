using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        return Created($"/api/courses/{course.Slug}", new { success = true, data = course });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CreateCourseRequest request, IFormFile? thumbnail)
    {
        var course = await _courseService.UpdateCourseAsync(id, request, thumbnail);
        return course == null ? NotFound() : Ok(new { success = true, data = course });
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

    [HttpGet("{courseId:int}/lessons")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetLessons(int courseId)
    {
        var lessons = await _courseService.GetLessonsAsync(courseId);
        return Ok(new { success = true, data = lessons });
    }

    [HttpPost("lessons")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddLesson([FromBody] CreateLessonRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var lesson = await _courseService.AddLessonAsync(request);
        return Created("", new { success = true, data = lesson });
    }

    [HttpPut("lessons/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLesson(int id, [FromBody] UpdateLessonRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var lesson = await _courseService.UpdateLessonAsync(id, request);
        return lesson == null ? NotFound() : Ok(new { success = true, data = lesson });
    }

    [HttpDelete("lessons/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        var result = await _courseService.DeleteLessonAsync(id);
        return result ? Ok(new { success = true }) : NotFound();
    }
}
