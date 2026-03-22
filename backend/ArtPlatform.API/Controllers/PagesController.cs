using ArtPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PagesController(AppDbContext context) => _context = context;

    /// <summary>جلب صفحة عامة (terms_of_use أو privacy_policy)</summary>
    [HttpGet("{key}")]
    public async Task<IActionResult> GetPage(string key)
    {
        var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null) return NotFound();
        return Ok(new { success = true, data = new { key = setting.Key, content = setting.Value } });
    }

    /// <summary>تحديث محتوى صفحة (Admin فقط)</summary>
    [HttpPut("{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePage(string key, [FromBody] UpdatePageRequest request)
    {
        var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null) return NotFound();

        setting.Value = request.Content;
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "تم تحديث الصفحة بنجاح" });
    }
}

public class UpdatePageRequest
{
    public string Content { get; set; } = string.Empty;
}
