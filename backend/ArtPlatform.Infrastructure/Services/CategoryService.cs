using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using ArtPlatform.Domain.Entities;
using ArtPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IMediaUrlBuilder _mediaUrl;

    public CategoryService(AppDbContext context, IMediaUrlBuilder mediaUrl)
    {
        _context = context;
        _mediaUrl = mediaUrl;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var list = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryDto
            {
                Id = c.Id, Name = c.Name, Slug = c.Slug,
                Description = c.Description, ImageUrl = c.ImageUrl,
                SortOrder = c.SortOrder, IsActive = c.IsActive,
                ArtworkCount = c.Artworks.Count(a => a.Status.ToString() == "Published"),
                CourseCount = c.Courses.Count(co => co.Status.ToString() == "Published")
            }).ToListAsync();

        foreach (var item in list)
            item.ImageUrl = _mediaUrl.ToAbsolute(item.ImageUrl);
        return list;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var c = await _context.Categories.Include(x => x.Artworks).Include(x => x.Courses).FirstOrDefaultAsync(x => x.Id == id);
        if (c == null) return null;
        return new CategoryDto
        {
            Id = c.Id, Name = c.Name, Slug = c.Slug,
            Description = c.Description, ImageUrl = _mediaUrl.ToAbsolute(c.ImageUrl),
            SortOrder = c.SortOrder, IsActive = c.IsActive,
            ArtworkCount = c.Artworks.Count, CourseCount = c.Courses.Count
        };
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        var slug = request.Name.ToLowerInvariant().Replace(" ", "-");
        var category = new Category
        {
            Name = request.Name, Slug = slug,
            Description = request.Description, SortOrder = request.SortOrder
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(category.Id))!;
    }

    public async Task<CategoryDto?> UpdateAsync(int id, CreateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;
        category.Name = request.Name;
        category.Description = request.Description;
        category.SortOrder = request.SortOrder;
        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context) => _context = context;

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var recentArtworks = await _context.Artworks
            .OrderByDescending(a => a.CreatedAt).Take(3)
            .Select(a => new RecentActivityDto { Type = "artwork", Title = a.Title, Date = a.CreatedAt }).ToListAsync();

        var recentCourses = await _context.Courses
            .OrderByDescending(c => c.CreatedAt).Take(3)
            .Select(c => new RecentActivityDto { Type = "course", Title = c.Title, Date = c.CreatedAt }).ToListAsync();

        return new DashboardStatsDto
        {
            TotalArtworks = await _context.Artworks.CountAsync(),
            TotalCourses = await _context.Courses.CountAsync(),
            TotalUsers = await _context.Users.CountAsync(),
            TotalEnrollments = await _context.Enrollments.CountAsync(),
            TotalBlogPosts = await _context.BlogPosts.CountAsync(),
            UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsRead),
            RecentActivity = recentArtworks.Concat(recentCourses).OrderByDescending(a => a.Date).Take(5).ToList()
        };
    }
}
