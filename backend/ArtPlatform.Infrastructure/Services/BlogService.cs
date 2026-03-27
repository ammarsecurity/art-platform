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
    private readonly IMediaUrlBuilder _mediaUrl;

    public BlogService(AppDbContext context, IFileService fileService, IMediaUrlBuilder mediaUrl)
    {
        _context = context;
        _fileService = fileService;
        _mediaUrl = mediaUrl;
    }

    public async Task<PagedResult<BlogPostDto>> GetPostsAsync(ArtworkListRequest request)
    {
        var query = _context.BlogPosts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => p.Title.Contains(request.Search));

        var statusRaw = request.Status?.Trim();
        if (string.Equals(statusRaw, "all", StringComparison.OrdinalIgnoreCase))
        {
            /* لا نفلتر بالحالة — لوحة الإدارة */
        }
        else if (!string.IsNullOrWhiteSpace(statusRaw) && Enum.TryParse<PostStatus>(statusRaw, true, out var status))
            query = query.Where(p => p.Status == status);
        else
            query = query.Where(p => p.Status == PostStatus.Published);

        if (request.IsFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == request.IsFeatured.Value);

        var sortDesc = string.IsNullOrWhiteSpace(request.SortOrder)
            || request.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);
        query = (request.SortBy?.ToLowerInvariant()) switch
        {
            "title" => sortDesc ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            "createdat" => sortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "publishedat" => sortDesc ? query.OrderByDescending(p => p.PublishedAt ?? p.CreatedAt) : query.OrderBy(p => p.PublishedAt ?? p.CreatedAt),
            "sortorder" or "order" => sortDesc
                ? query.OrderByDescending(p => p.SortOrder).ThenByDescending(p => p.PublishedAt ?? p.CreatedAt)
                : query.OrderBy(p => p.SortOrder).ThenBy(p => p.PublishedAt ?? p.CreatedAt),
            _ => sortDesc
                ? query.OrderByDescending(p => p.SortOrder).ThenByDescending(p => p.PublishedAt ?? p.CreatedAt)
                : query.OrderBy(p => p.SortOrder).ThenBy(p => p.PublishedAt ?? p.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new BlogPostDto
            {
                Id = p.Id, Title = p.Title, Slug = p.Slug, Excerpt = p.Excerpt,
                FeaturedImageUrl = p.FeaturedImageUrl, Status = p.Status.ToString(),
                IsFeatured = p.IsFeatured, ViewCount = p.ViewCount,
                PublishedAt = p.PublishedAt, CreatedAt = p.CreatedAt,
                SortOrder = p.SortOrder
            }).ToListAsync();

        foreach (var p in items)
            p.FeaturedImageUrl = _mediaUrl.ToAbsolute(p.FeaturedImageUrl);

        return new PagedResult<BlogPostDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
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

    public async Task<BlogPostDetailDto?> GetPostByIdAsync(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        return post == null ? null : MapToDetailDto(post);
    }

    public async Task<BlogPostDetailDto> CreatePostAsync(CreateBlogPostRequest request, IFormFile? image)
    {
        var slug = await EnsureUniqueSlugAsync(GenerateSlug(request.Title));
        var imageUrl = image != null ? await _fileService.UploadImageAsync(image, "blog") : null;
        var postStatus = Enum.TryParse<PostStatus>(request.Status, true, out var s) ? s : PostStatus.Draft;

        var post = new BlogPost
        {
            Title = request.Title, Slug = slug, Content = request.Content,
            Excerpt = request.Excerpt, FeaturedImageUrl = imageUrl,
            IsFeatured = request.IsFeatured, Status = postStatus,
            SortOrder = request.SortOrder,
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
        post.SortOrder = request.SortOrder;
        post.UpdatedAt = DateTime.UtcNow;

        if (Enum.TryParse<PostStatus>(request.Status, true, out var status))
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

    private BlogPostDetailDto MapToDetailDto(BlogPost p) => new()
    {
        Id = p.Id, Title = p.Title, Slug = p.Slug, Content = p.Content,
        Excerpt = p.Excerpt, FeaturedImageUrl = _mediaUrl.ToAbsolute(p.FeaturedImageUrl),
        Status = p.Status.ToString(), IsFeatured = p.IsFeatured,
        ViewCount = p.ViewCount, MetaTitle = p.MetaTitle,
        MetaDescription = p.MetaDescription, PublishedAt = p.PublishedAt, CreatedAt = p.CreatedAt,
        SortOrder = p.SortOrder
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
