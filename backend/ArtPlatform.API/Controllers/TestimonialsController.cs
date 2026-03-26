using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestimonialsController : ControllerBase
{
    private readonly ITestimonialService _service;

    public TestimonialsController(ITestimonialService service) => _service = service;

    /// <summary>عام — يُعيد التقييمات النشطة فقط</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync(activeOnly: true);
        return Ok(new { success = true, data = items });
    }

    /// <summary>أدمن — يُعيد جميع التقييمات</summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdmin()
    {
        var items = await _service.GetAllAsync(activeOnly: false);
        return Ok(new { success = true, data = items });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTestimonialRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var item = await _service.CreateAsync(request);
        return Created("", new { success = true, data = item });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateTestimonialRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var item = await _service.UpdateAsync(id, request);
        return item == null ? NotFound() : Ok(new { success = true, data = item });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? Ok(new { success = true }) : NotFound();
    }

    [HttpPut("{id:int}/toggle")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Toggle(int id)
    {
        var item = await _service.ToggleActiveAsync(id);
        return item == null ? NotFound() : Ok(new { success = true, data = item, message = item.IsActive ? "تم التفعيل" : "تم الإخفاء" });
    }
}
