using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService) => _blogService = blogService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ArtworkListRequest request)
    {
        var result = await _blogService.GetPostsAsync(request);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var post = await _blogService.GetPostBySlugAsync(slug);
        return post == null ? NotFound(new { success = false, message = "المقال غير موجود" })
                            : Ok(new { success = true, data = post });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] CreateBlogPostRequest request, IFormFile? image)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var post = await _blogService.CreatePostAsync(request, image);
        return Created($"/api/blog/{post.Slug}", new { success = true, data = post });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CreateBlogPostRequest request, IFormFile? image)
    {
        var post = await _blogService.UpdatePostAsync(id, request, image);
        return post == null ? NotFound() : Ok(new { success = true, data = post });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _blogService.DeletePostAsync(id);
        return result ? Ok(new { success = true }) : NotFound();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService) => _contactService = contactService;

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] CreateContactRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _contactService.SendMessageAsync(request);
        return Ok(new { success = true, message = "تم إرسال رسالتك بنجاح، سنتواصل معك قريباً" });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _contactService.GetMessagesAsync(page, pageSize);
        return Ok(new { success = true, data = result });
    }

    [HttpPatch("{id:int}/read")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _contactService.MarkAsReadAsync(id);
        return Ok(new { success = true });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _contactService.DeleteMessageAsync(id);
        return Ok(new { success = true });
    }
}
