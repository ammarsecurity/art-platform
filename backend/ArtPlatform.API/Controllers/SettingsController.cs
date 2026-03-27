using System.Text.Json;
using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using ArtPlatform.Domain.Entities;
using ArtPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    public const string HomePageConfigKey = "home_page_config";
    public const string ContactPageConfigKey = "contact_page_config";
    public const string FooterConfigKey = "footer_config";

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private static readonly JsonSerializerOptions JsonReadOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly AppDbContext _context;
    private readonly IFileService _fileService;

    public SettingsController(AppDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    /// <summary>رفع صورة لقسم «عن الفنان» في الصفحة الرئيسية</summary>
    [HttpPost("upload-image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAboutImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "يرجى اختيار ملف صورة" });

        try
        {
            var url = await _fileService.UploadImageAsync(file, "site");
            return Ok(new { success = true, data = new { url } });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>إعدادات العرض للموقع العام والصفحة الرئيسية</summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublic()
    {
        var data = await BuildPublicDtoAsync();
        return Ok(new { success = true, data });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _context.SiteSettings
            .OrderBy(s => s.Key)
            .Select(s => new { s.Id, s.Key, s.Value, s.Description })
            .ToListAsync();
        return Ok(new { success = true, data = items });
    }

    [HttpPut("{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string key, [FromBody] UpdateSettingRequest request)
    {
        if (string.IsNullOrWhiteSpace(key)) return BadRequest();
        var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null) return NotFound(new { success = false, message = "المفتاح غير موجود" });
        setting.Value = request.Value ?? string.Empty;
        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "تم الحفظ" });
    }

    /// <summary>تحديث عدة مفاتيح دفعة واحدة (مفاتيح مسموحة فقط)</summary>
    [HttpPut("batch")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBatch([FromBody] Dictionary<string, string> items)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "site_name", "site_description", "contact_email", "hero_title", "hero_subtitle", "site_logo_url",
            HomePageConfigKey,
            ContactPageConfigKey,
            FooterConfigKey
        };

        foreach (var kv in items)
        {
            if (!allowed.Contains(kv.Key)) continue;
            var setting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.Key == kv.Key);
            if (setting == null)
            {
                _context.SiteSettings.Add(new SiteSettings
                {
                    Key = kv.Key,
                    Value = kv.Value ?? string.Empty,
                    Description = kv.Key switch
                    {
                        ContactPageConfigKey => "محتوى صفحة التواصل",
                        FooterConfigKey => "محتوى الفوتر",
                        _ => null
                    }
                });
            }
            else
                setting.Value = kv.Value ?? string.Empty;
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "تم الحفظ" });
    }

    private async Task<PublicSiteSettingsDto> BuildPublicDtoAsync()
    {
        async Task<string> Get(string key, string fallback) =>
            (await _context.SiteSettings.FirstOrDefaultAsync(s => s.Key == key))?.Value ?? fallback;

        var homeJson = await Get(HomePageConfigKey, "");
        HomePageConfigDto home;
        if (string.IsNullOrWhiteSpace(homeJson))
            home = new HomePageConfigDto();
        else
        {
            try
            {
                home = JsonSerializer.Deserialize<HomePageConfigDto>(homeJson, JsonReadOpts) ?? new HomePageConfigDto();
            }
            catch
            {
                home = new HomePageConfigDto();
            }
        }

        home.Slider ??= new HomeSliderConfigDto();
        home.Slider.Items ??= new List<HomeSliderItemDto>();

        var contactJson = await Get(ContactPageConfigKey, "");
        ContactPageConfigDto contact;
        if (string.IsNullOrWhiteSpace(contactJson))
            contact = new ContactPageConfigDto();
        else
        {
            try
            {
                contact = JsonSerializer.Deserialize<ContactPageConfigDto>(contactJson, JsonReadOpts) ?? new ContactPageConfigDto();
            }
            catch
            {
                contact = new ContactPageConfigDto();
            }
        }

        contact.InfoRows ??= new List<ContactInfoRowDto>();
        contact.SocialLinks ??= new List<ContactSocialLinkDto>();
        contact.SubjectOptions ??= new List<string>();

        var footerJson = await Get(FooterConfigKey, "");
        FooterConfigDto footer;
        if (string.IsNullOrWhiteSpace(footerJson))
            footer = new FooterConfigDto();
        else
        {
            try
            {
                footer = JsonSerializer.Deserialize<FooterConfigDto>(footerJson, JsonReadOpts) ?? new FooterConfigDto();
            }
            catch
            {
                footer = new FooterConfigDto();
            }
        }

        footer.SocialIcons ??= new List<FooterSocialDto>();
        footer.Columns ??= new List<FooterColumnDto>();
        foreach (var col in footer.Columns)
            col.Links ??= new List<FooterLinkDto>();
        footer.LegalLinks ??= new List<FooterLegalLinkDto>();

        return new PublicSiteSettingsDto
        {
            SiteLogoUrl = await Get("site_logo_url", ""),
            SiteName = await Get("site_name", "منصة الفن"),
            SiteDescription = await Get("site_description", "منصة للفنون والتعلم"),
            ContactEmail = await Get("contact_email", "info@artplatform.com"),
            HeroTitle = await Get("hero_title", "استكشف عالم الفن"),
            HeroSubtitle = await Get("hero_subtitle", "تعلم، استلهم، وأبدع مع أفضل الفنانين"),
            HomePage = home,
            ContactPage = contact,
            Footer = footer
        };
    }
}
