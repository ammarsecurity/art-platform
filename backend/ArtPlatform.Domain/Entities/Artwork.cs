using ArtPlatform.Domain.Enums;

namespace ArtPlatform.Domain.Entities;

public class Artwork
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Medium { get; set; }
    public string? Dimensions { get; set; }
    public int? Year { get; set; }
    public bool IsFeatured { get; set; } = false;
    public ArtworkStatus Status { get; set; } = ArtworkStatus.Published;
    public int SortOrder { get; set; } = 0;
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<ArtworkTag> ArtworkTags { get; set; } = new List<ArtworkTag>();
}
