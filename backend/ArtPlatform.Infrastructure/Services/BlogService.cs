using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using ArtPlatform.Domain.Entities;
using ArtPlatform.Domain.Enums;
using ArtPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Infrastructure.Services;

public class BlogService : IBlogService
{
    private readonly AppDbContext _context;
    private readonly IFileService _fileService;

    public BlogService(AppDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<PagedResult<BlogPostDto>> GetPostsAsync(ArtworkListRequest request)
    {
        var query = _context.BlogPosts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => p.Title.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Status) && request.Status != "all")
        {
            if (Enum.TryParse<PostStatus>(request.Status, out var status))
                query = query.Where(p => p.Status == status);
            else
                query = query.Where(p => p.Status == PostStatus.Published);
        }
        else if (string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(p => p.Status == PostStatus.Published);

        if (request.IsFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == request.IsFeatured.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new BlogPostDto
            {
                Id = p.Id, Title = p.Title, Slug = p.Slug, Excerpt = p.Excerpt,
                FeaturedImageUrl = p.FeaturedImageUrl, Status = p.Status.ToString(),
                IsFeatured = p.IsFeatured, ViewCount = p.ViewCount,
                PublishedAt = p.PublishedAt, CreatedAt = p.CreatedAt
            }).ToListAsync();

        return new PagedResult<BlogPostDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
    }

    public async Task<BlogPostDetailDto?> GetPostByIdAsync(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        return post == null ? null : MapToDetailDto(post);
    }

    public async Task<BlogPostDetailDto?> GetPostBySlugAsync(string slug)
    {
        var post = await _context.BlogPosts
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == PostStatus.Published);

        if (post == null) return null;

        post.ViewCount++;
        await _context.SaveChangesAsync();

        return MapToDetailDto(post);
    }

    public async Task<BlogPostDetailDto> CreatePostAsync(CreateBlogPostRequest request, IFormFile? image)
    {
        var slug = await EnsureUniqueSlugAsync(GenerateSlug(request.Title));
        var imageUrl = image != null ? await _fileService.UploadImageAsync(image, "blog") : null;
        var postStatus = Enum.TryParse<PostStatus>(request.Status, out var s) ? s : PostStatus.Draft;

        var post = new BlogPost
        {
            Title = request.Title, Slug = slug, Content = request.Content,
            Excerpt = request.Excerpt, FeaturedImageUrl = imageUrl,
            IsFeatured = request.IsFeatured, Status = postStatus,
            MetaTitle = request.MetaTitle, MetaDescription = request.MetaDescription,
            PublishedAt = postStatus == PostStatus.Published ? DateTime.UtcNow : null
        };

        _context.BlogPosts.Add(post);
        await _context.SaveChangesAsync();
        return MapToDetailDto(post);
    }

    public async Task<BlogPostDetailDto?> UpdatePostAsync(int id, CreateBlogPostRequest request, IFormFile? image)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null) return null;

        if (image != null)
            post.FeaturedImageUrl = await _fileService.UploadImageAsync(image, "blog");

        post.Title = request.Title;
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.IsFeatured = request.IsFeatured;
        post.MetaTitle = request.MetaTitle;
        post.MetaDescription = request.MetaDescription;
        post.UpdatedAt = DateTime.UtcNow;

        if (Enum.TryParse<PostStatus>(request.Status, out var status))
        {
            if (post.Status != PostStatus.Published && status == PostStatus.Published)
                post.PublishedAt = DateTime.UtcNow;
            post.Status = status;
        }

        await _context.SaveChangesAsync();
        return MapToDetailDto(post);
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post == null) return false;
        _context.BlogPosts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug)
    {
        var exists = await _context.BlogPosts.AnyAsync(p => p.Slug == slug);
        return exists ? $"{slug}-{Guid.NewGuid().ToString()[..6]}" : slug;
    }

    private static string GenerateSlug(string text) =>
        text.ToLowerInvariant().Replace(" ", "-").Replace("_", "-").Replace("/", "-");

    private static BlogPostDetailDto MapToDetailDto(BlogPost p) => new()
    {
        Id = p.Id, Title = p.Title, Slug = p.Slug, Content = p.Content,
        Excerpt = p.Excerpt, FeaturedImageUrl = p.FeaturedImageUrl,
        Status = p.Status.ToString(), IsFeatured = p.IsFeatured,
        ViewCount = p.ViewCount, MetaTitle = p.MetaTitle,
        MetaDescription = p.MetaDescription, PublishedAt = p.PublishedAt, CreatedAt = p.CreatedAt
    };
}

public class ContactService : IContactService
{
    private readonly AppDbContext _context;

    public ContactService(AppDbContext context) => _context = context;

    public async Task<bool> SendMessageAsync(CreateContactRequest request)
    {
        _context.ContactMessages.Add(new ContactMessage
        {
            Name = request.Name, Email = request.Email, Phone = request.Phone,
            Subject = request.Subject, Message = request.Message
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<ContactMessageDto>> GetMessagesAsync(int page, int pageSize)
    {
        var total = await _context.ContactMessages.CountAsync();
        var items = await _context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(m => new ContactMessageDto
            {
                Id = m.Id, Name = m.Name, Email = m.Email, Phone = m.Phone,
                Subject = m.Subject, Message = m.Message,
                IsRead = m.IsRead, CreatedAt = m.CreatedAt
            }).ToListAsync();

        return new PagedResult<ContactMessageDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var msg = await _context.ContactMessages.FindAsync(id);
        if (msg == null) return false;
        msg.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMessageAsync(int id)
    {
        var msg = await _context.ContactMessages.FindAsync(id);
        if (msg == null) return false;
        _context.ContactMessages.Remove(msg);
        await _context.SaveChangesAsync();
        return true;
    }
}
