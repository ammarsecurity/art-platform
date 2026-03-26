using System.ComponentModel.DataAnnotations;

namespace ArtPlatform.Application.DTOs;

public class ArtworkDto
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
    public bool IsFeatured { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateArtworkRequest
{
    [Required(ErrorMessage = "العنوان مطلوب")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Medium { get; set; }
    public string? Dimensions { get; set; }
    public int? Year { get; set; }
    public bool IsFeatured { get; set; } = false;
    [Required]
    public int CategoryId { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class UpdateArtworkRequest : CreateArtworkRequest
{
    public string? Status { get; set; }
}

public class ArtworkListRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int? CategoryId { get; set; }
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Level { get; set; }
    public bool? IsFeatured { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortOrder { get; set; } = "desc";
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
