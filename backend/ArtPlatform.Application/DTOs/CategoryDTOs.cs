using System.ComponentModel.DataAnnotations;

namespace ArtPlatform.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ArtworkCount { get; set; }
    public int CourseCount { get; set; }
}

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "اسم التصنيف مطلوب")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class DashboardStatsDto
{
    public int TotalArtworks { get; set; }
    public int TotalCourses { get; set; }
    public int TotalUsers { get; set; }
    public int TotalEnrollments { get; set; }
    public int TotalBlogPosts { get; set; }
    public int UnreadMessages { get; set; }
    public List<RecentActivityDto> RecentActivity { get; set; } = new();
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
