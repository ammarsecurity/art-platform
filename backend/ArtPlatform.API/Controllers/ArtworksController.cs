using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtworksController : ControllerBase
{
    private readonly IArtworkService _artworkService;

    public ArtworksController(IArtworkService artworkService) => _artworkService = artworkService;

    /// <summary>قائمة الأعمال الفنية</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ArtworkListRequest request)
    {
        var result = await _artworkService.GetArtworksAsync(request);
        return Ok(new { success = true, data = result });
    }

    /// <summary>الأعمال المميزة</summary>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int count = 6)
    {
        var result = await _artworkService.GetFeaturedArtworksAsync(count);
        return Ok(new { success = true, data = result });
    }

    /// <summary>تفاصيل عمل فني بالمعرّف (لوحة الإدارة) — لا يتعارض مع الـ slug الرقمي</summary>
    [HttpGet("by-id/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var artwork = await _artworkService.GetArtworkByIdAsync(id);
        return artwork == null ? NotFound(new { success = false, message = "العمل الفني غير موجود" })
                               : Ok(new { success = true, data = artwork });
    }

    /// <summary>تفاصيل عمل فني بالـ Slug</summary>
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var artwork = await _artworkService.GetArtworkBySlugAsync(slug);
        return artwork == null ? NotFound(new { success = false, message = "العمل الفني غير موجود" })
                               : Ok(new { success = true, data = artwork });
    }

    /// <summary>إضافة عمل فني جديد (Admin)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] CreateArtworkRequest request, IFormFile image)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var artwork = await _artworkService.CreateArtworkAsync(request, image);
        var location = $"/api/artworks/{Uri.EscapeDataString(artwork.Slug)}";
        return Created(location, new { success = true, data = artwork });
    }

    /// <summary>تحديث عمل فني (Admin)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateArtworkRequest request, IFormFile? image)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var artwork = await _artworkService.UpdateArtworkAsync(id, request, image);
        return artwork == null ? NotFound() : Ok(new { success = true, data = artwork });
    }

    /// <summary>حذف عمل فني (Admin)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _artworkService.DeleteArtworkAsync(id);
        return result ? Ok(new { success = true, message = "تم حذف العمل الفني" })
                      : NotFound(new { success = false, message = "العمل الفني غير موجود" });
    }
}
