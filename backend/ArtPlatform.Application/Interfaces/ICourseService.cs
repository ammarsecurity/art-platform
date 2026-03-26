using ArtPlatform.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ArtPlatform.Application.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseDto>> GetCoursesAsync(ArtworkListRequest request, int? userId = null);
    Task<CourseDetailDto?> GetCourseBySlugAsync(string slug, int? userId = null);
    Task<CourseDto> CreateCourseAsync(CreateCourseRequest request, IFormFile? thumbnail);
    Task<CourseDto?> UpdateCourseAsync(int id, CreateCourseRequest request, IFormFile? thumbnail);
    Task<bool> DeleteCourseAsync(int id);
    Task<bool> EnrollAsync(int courseId, int userId);
    Task<bool> UpdateProgressAsync(int userId, UpdateProgressRequest request);
    Task<List<AdminLessonDto>> GetLessonsAsync(int courseId);
    Task<LessonDto> AddLessonAsync(CreateLessonRequest request);
    Task<AdminLessonDto?> UpdateLessonAsync(int id, UpdateLessonRequest request);
    Task<bool> DeleteLessonAsync(int id);
    Task<List<CourseDto>> GetFeaturedCoursesAsync(int count = 4);
}
