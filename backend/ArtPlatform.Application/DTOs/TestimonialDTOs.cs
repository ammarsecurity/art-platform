using System.ComponentModel.DataAnnotations;

namespace ArtPlatform.Application.DTOs;

public class TestimonialDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTestimonialRequest
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "المسمى الوظيفي مطلوب")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "نص التقييم مطلوب")]
    public string Text { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
