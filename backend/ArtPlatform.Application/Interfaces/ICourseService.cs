using ArtPlatform.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ArtPlatform.Application.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseDto>> GetCoursesAsync(ArtworkListRequest request, int? userId = null);
    Task<CourseDetailDto?> GetCourseBySlugAsync(string slug, int? userId = null);
    /// <summary>جلب الدورة بالمعرّف لإدارة المحتوى (أي حالة نشر)</summary>
    Task<CourseDetailDto?> GetCourseByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateCourseRequest request, IFormFile? thumbnail);
    Task<CourseDto?> UpdateCourseAsync(int id, CreateCourseRequest request, IFormFile? thumbnail);
    Task<bool> SetCourseStatusAsync(int id, string status);
    Task<bool> DeleteCourseAsync(int id);
    Task<bool> EnrollAsync(int courseId, int userId);
    Task<bool> UpdateProgressAsync(int userId, UpdateProgressRequest request);
    Task<LessonDto> AddLessonAsync(CreateLessonRequest request, IFormFile? videoFile = null);
    Task<LessonDto?> UpdateLessonAsync(int lessonId, UpdateLessonRequest request, IFormFile? videoFile = null);
    Task<bool> DeleteLessonAsync(int id);
    Task<List<CourseDto>> GetFeaturedCoursesAsync(int count = 4);
}
