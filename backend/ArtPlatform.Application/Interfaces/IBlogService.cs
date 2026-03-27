using ArtPlatform.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ArtPlatform.Application.Interfaces;

public interface IBlogService
{
    Task<PagedResult<BlogPostDto>> GetPostsAsync(ArtworkListRequest request);
    Task<BlogPostDetailDto?> GetPostBySlugAsync(string slug);
    /// <summary>جلب المقال بالمعرّف للتحرير (أي حالة مسودة أو منشور)</summary>
    Task<BlogPostDetailDto?> GetPostByIdAsync(int id);
    Task<BlogPostDetailDto> CreatePostAsync(CreateBlogPostRequest request, IFormFile? image);
    Task<BlogPostDetailDto?> UpdatePostAsync(int id, CreateBlogPostRequest request, IFormFile? image);
    Task<bool> DeletePostAsync(int id);
}

public interface IContactService
{
    Task<bool> SendMessageAsync(CreateContactRequest request);
    Task<PagedResult<ContactMessageDto>> GetMessagesAsync(int page, int pageSize);
    Task<bool> MarkAsReadAsync(int id);
    Task<bool> DeleteMessageAsync(int id);
}
