using System.ComponentModel.DataAnnotations;

namespace ArtPlatform.Application.DTOs;

public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BlogPostDetailDto : BlogPostDto
{
    public string Content { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class CreateBlogPostRequest
{
    [Required(ErrorMessage = "عنوان المقال مطلوب")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "محتوى المقال مطلوب")]
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsFeatured { get; set; } = false;
    public string Status { get; set; } = "Draft";
}

public class ContactMessageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateContactRequest
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    [Required(ErrorMessage = "الموضوع مطلوب")]
    public string Subject { get; set; } = string.Empty;
    [Required(ErrorMessage = "الرسالة مطلوبة")]
    [MinLength(10, ErrorMessage = "الرسالة يجب أن تكون 10 أحرف على الأقل")]
    public string Message { get; set; } = string.Empty;
}
