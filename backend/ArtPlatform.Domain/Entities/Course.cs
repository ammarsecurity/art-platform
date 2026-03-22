using ArtPlatform.Domain.Enums;

namespace ArtPlatform.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public decimal Price { get; set; } = 0;
    public int DurationMinutes { get; set; } = 0;
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public bool IsFeatured { get; set; } = false;
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
