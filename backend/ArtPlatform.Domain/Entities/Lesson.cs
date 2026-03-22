namespace ArtPlatform.Domain.Entities;

public class Lesson
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int DurationMinutes { get; set; } = 0;
    public int SortOrder { get; set; } = 0;
    public bool IsPreview { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Course Course { get; set; } = null!;
    public ICollection<LessonProgress> Progresses { get; set; } = new List<LessonProgress>();
}
