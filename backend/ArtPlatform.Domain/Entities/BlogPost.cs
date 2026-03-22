using ArtPlatform.Domain.Enums;

namespace ArtPlatform.Domain.Entities;

public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public bool IsFeatured { get; set; } = false;
    public int ViewCount { get; set; } = 0;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
