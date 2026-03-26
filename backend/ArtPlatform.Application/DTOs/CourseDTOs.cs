using System.ComponentModel.DataAnnotations;

namespace ArtPlatform.Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int LessonCount { get; set; }
    public int EnrollmentCount { get; set; }
    public bool IsEnrolled { get; set; }
    public int? ProgressPercent { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CourseDetailDto : CourseDto
{
    public List<LessonDto> Lessons { get; set; } = new();
}

public class LessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationMinutes { get; set; }
    public int SortOrder { get; set; }
    public bool IsPreview { get; set; }
    public bool IsCompleted { get; set; }
    public int WatchedSeconds { get; set; }
}

public class CreateCourseRequest
{
    [Required(ErrorMessage = "عنوان الدورة مطلوب")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public decimal Price { get; set; } = 0;
    public string Level { get; set; } = "Beginner";
    public string Status { get; set; } = "Draft";
    public bool IsFeatured { get; set; } = false;
    [Required]
    public int CategoryId { get; set; }
}

public class CreateLessonRequest
{
    [Required(ErrorMessage = "عنوان الدرس مطلوب")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationMinutes { get; set; } = 0;
    public int SortOrder { get; set; } = 0;
    public bool IsPreview { get; set; } = false;
    [Required]
    public int CourseId { get; set; }
}

public class UpdateLessonRequest
{
    [Required(ErrorMessage = "عنوان الدرس مطلوب")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationMinutes { get; set; } = 0;
    public int SortOrder { get; set; } = 0;
    public bool IsPreview { get; set; } = false;
}

public class AdminLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationMinutes { get; set; }
    public int SortOrder { get; set; }
    public bool IsPreview { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateProgressRequest
{
    public int LessonId { get; set; }
    public int WatchedSeconds { get; set; }
    public bool IsCompleted { get; set; }
}
