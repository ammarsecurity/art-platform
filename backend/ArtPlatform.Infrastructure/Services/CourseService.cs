using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using ArtPlatform.Domain.Entities;
using ArtPlatform.Domain.Enums;
using ArtPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly AppDbContext _context;
    private readonly IFileService _fileService;

    public CourseService(AppDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<PagedResult<CourseDto>> GetCoursesAsync(ArtworkListRequest request, int? userId = null)
    {
        var query = _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .AsQueryable();

        if (request.CategoryId.HasValue)
            query = query.Where(c => c.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Title.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Status) && request.Status != "all")
        {
            if (Enum.TryParse<CourseStatus>(request.Status, out var status))
                query = query.Where(c => c.Status == status);
            else
                query = query.Where(c => c.Status == CourseStatus.Published);
        }
        else if (string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(c => c.Status == CourseStatus.Published);

        if (!string.IsNullOrWhiteSpace(request.Level) && Enum.TryParse<CourseLevel>(request.Level, out var level))
            query = query.Where(c => c.Level == level);

        if (request.IsFeatured.HasValue)
            query = query.Where(c => c.IsFeatured == request.IsFeatured.Value);

        var total = await query.CountAsync();
        var courses = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var enrolledIds = userId.HasValue
            ? (await _context.Enrollments.Where(e => e.UserId == userId.Value).Select(e => e.CourseId).ToListAsync())
            : new List<int>();

        var progressMap = userId.HasValue
            ? (await _context.Enrollments.Where(e => e.UserId == userId.Value)
                .ToDictionaryAsync(e => e.CourseId, e => e.ProgressPercent))
            : new Dictionary<int, int>();

        var items = courses.Select(c => new CourseDto
        {
            Id = c.Id, Title = c.Title, Slug = c.Slug,
            Description = c.Description,
            ShortDescription = c.ShortDescription, ThumbnailUrl = c.ThumbnailUrl,
            PreviewVideoUrl = c.PreviewVideoUrl, Price = c.Price,
            DurationMinutes = c.DurationMinutes, Level = c.Level.ToString(),
            Status = c.Status.ToString(), IsFeatured = c.IsFeatured,
            CategoryId = c.CategoryId, CategoryName = c.Category?.Name ?? "",
            LessonCount = c.Lessons.Count(l => l.IsActive),
            EnrollmentCount = c.Enrollments.Count,
            IsEnrolled = enrolledIds.Contains(c.Id),
            ProgressPercent = progressMap.TryGetValue(c.Id, out var p) ? p : null,
            CreatedAt = c.CreatedAt
        }).ToList();

        return new PagedResult<CourseDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
    }

    public async Task<CourseDetailDto?> GetCourseBySlugAsync(string slug, int? userId = null)
    {
        var course = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Slug == slug && c.Status == CourseStatus.Published);

        if (course == null) return null;

        var isEnrolled = userId.HasValue && course.Enrollments.Any(e => e.UserId == userId.Value);
        var progressMap = new Dictionary<int, LessonProgress>();

        if (userId.HasValue)
        {
            var lessonIds = course.Lessons.Select(l => l.Id).ToList();
            var progresses = await _context.LessonProgresses
                .Where(lp => lp.UserId == userId.Value && lessonIds.Contains(lp.LessonId))
                .ToListAsync();
            progressMap = progresses.ToDictionary(lp => lp.LessonId);
        }

        return new CourseDetailDto
        {
            Id = course.Id, Title = course.Title, Slug = course.Slug,
            Description = course.Description, ShortDescription = course.ShortDescription,
            ThumbnailUrl = course.ThumbnailUrl, PreviewVideoUrl = course.PreviewVideoUrl,
            Price = course.Price, DurationMinutes = course.DurationMinutes,
            Level = course.Level.ToString(), Status = course.Status.ToString(),
            IsFeatured = course.IsFeatured, CategoryId = course.CategoryId,
            CategoryName = course.Category?.Name ?? "",
            LessonCount = course.Lessons.Count(l => l.IsActive),
            EnrollmentCount = course.Enrollments.Count,
            IsEnrolled = isEnrolled,
            CreatedAt = course.CreatedAt,
            Lessons = course.Lessons
                .Where(l => l.IsActive && (l.IsPreview || isEnrolled))
                .OrderBy(l => l.SortOrder)
                .Select(l => new LessonDto
                {
                    Id = l.Id, Title = l.Title, Description = l.Description,
                    VideoUrl = l.VideoUrl, DurationMinutes = l.DurationMinutes,
                    SortOrder = l.SortOrder, IsPreview = l.IsPreview,
                    IsCompleted = progressMap.TryGetValue(l.Id, out var lp) && lp.IsCompleted,
                    WatchedSeconds = progressMap.TryGetValue(l.Id, out var lp2) ? lp2.WatchedSeconds : 0
                }).ToList()
        };
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseRequest request, IFormFile? thumbnail)
    {
        var slug = await EnsureUniqueSlugAsync(GenerateSlug(request.Title));
        var thumbnailUrl = thumbnail != null ? await _fileService.UploadImageAsync(thumbnail, "courses") : null;
        var level = Enum.TryParse<CourseLevel>(request.Level, out var l) ? l : CourseLevel.Beginner;

        var courseStatus = Enum.TryParse<CourseStatus>(request.Status, out var cs) ? cs : CourseStatus.Draft;

        var course = new Course
        {
            Title = request.Title, Slug = slug,
            Description = request.Description, ShortDescription = request.ShortDescription,
            ThumbnailUrl = thumbnailUrl, PreviewVideoUrl = request.PreviewVideoUrl,
            Price = request.Price, Level = level,
            IsFeatured = request.IsFeatured, CategoryId = request.CategoryId,
            Status = courseStatus
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return (await GetCoursesAsync(new ArtworkListRequest { Status = "all" })).Items.First(c => c.Id == course.Id);
    }

    public async Task<CourseDto?> UpdateCourseAsync(int id, CreateCourseRequest request, IFormFile? thumbnail)
    {
        var course = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (course == null) return null;

        if (thumbnail != null)
            course.ThumbnailUrl = await _fileService.UploadImageAsync(thumbnail, "courses");

        course.Title = request.Title;
        course.Description = request.Description;
        course.ShortDescription = request.ShortDescription;
        course.PreviewVideoUrl = request.PreviewVideoUrl;
        course.Price = request.Price;
        course.IsFeatured = request.IsFeatured;
        course.CategoryId = request.CategoryId;
        course.UpdatedAt = DateTime.UtcNow;

        if (Enum.TryParse<CourseLevel>(request.Level, out var level))
            course.Level = level;

        if (Enum.TryParse<CourseStatus>(request.Status, out var status))
            course.Status = status;

        await _context.SaveChangesAsync();

        return new CourseDto
        {
            Id = course.Id, Title = course.Title, Slug = course.Slug,
            ShortDescription = course.ShortDescription, ThumbnailUrl = course.ThumbnailUrl,
            PreviewVideoUrl = course.PreviewVideoUrl, Price = course.Price,
            DurationMinutes = course.DurationMinutes, Level = course.Level.ToString(),
            Status = course.Status.ToString(), IsFeatured = course.IsFeatured,
            CategoryId = course.CategoryId, CategoryName = course.Category?.Name ?? "",
            LessonCount = course.Lessons.Count(l => l.IsActive),
            EnrollmentCount = course.Enrollments.Count,
            CreatedAt = course.CreatedAt
        };
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EnrollAsync(int courseId, int userId)
    {
        if (await _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId))
            return true;

        _context.Enrollments.Add(new Enrollment { CourseId = courseId, UserId = userId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProgressAsync(int userId, UpdateProgressRequest request)
    {
        var progress = await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == request.LessonId);

        if (progress == null)
        {
            progress = new LessonProgress { UserId = userId, LessonId = request.LessonId };
            _context.LessonProgresses.Add(progress);
        }

        progress.WatchedSeconds = request.WatchedSeconds;
        progress.IsCompleted = request.IsCompleted;
        progress.UpdatedAt = DateTime.UtcNow;

        if (request.IsCompleted && progress.CompletedAt == null)
            progress.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await RecalculateCourseProgressAsync(userId, request.LessonId);
        return true;
    }

    private async Task RecalculateCourseProgressAsync(int userId, int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return;

        var totalLessons = await _context.Lessons.CountAsync(l => l.CourseId == lesson.CourseId && l.IsActive);
        var completedLessons = await _context.LessonProgresses
            .CountAsync(lp => lp.UserId == userId && lp.IsCompleted &&
                _context.Lessons.Any(l => l.Id == lp.LessonId && l.CourseId == lesson.CourseId));

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);

        if (enrollment != null)
        {
            enrollment.ProgressPercent = totalLessons > 0 ? (completedLessons * 100 / totalLessons) : 0;
            if (enrollment.ProgressPercent == 100) enrollment.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<AdminLessonDto>> GetLessonsAsync(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.SortOrder)
            .Select(l => new AdminLessonDto
            {
                Id = l.Id, Title = l.Title, Description = l.Description,
                VideoUrl = l.VideoUrl, DurationMinutes = l.DurationMinutes,
                SortOrder = l.SortOrder, IsPreview = l.IsPreview,
                IsActive = l.IsActive, CreatedAt = l.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<AdminLessonDto?> UpdateLessonAsync(int id, UpdateLessonRequest request)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) return null;

        lesson.Title = request.Title;
        lesson.Description = request.Description;
        lesson.VideoUrl = request.VideoUrl;
        lesson.DurationMinutes = request.DurationMinutes;
        lesson.SortOrder = request.SortOrder;
        lesson.IsPreview = request.IsPreview;

        await _context.SaveChangesAsync();

        return new AdminLessonDto
        {
            Id = lesson.Id, Title = lesson.Title, Description = lesson.Description,
            VideoUrl = lesson.VideoUrl, DurationMinutes = lesson.DurationMinutes,
            SortOrder = lesson.SortOrder, IsPreview = lesson.IsPreview,
            IsActive = lesson.IsActive, CreatedAt = lesson.CreatedAt
        };
    }

    public async Task<LessonDto> AddLessonAsync(CreateLessonRequest request)
    {
        var lesson = new Lesson
        {
            Title = request.Title, Description = request.Description,
            VideoUrl = request.VideoUrl, DurationMinutes = request.DurationMinutes,
            SortOrder = request.SortOrder, IsPreview = request.IsPreview,
            CourseId = request.CourseId
        };
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        return new LessonDto
        {
            Id = lesson.Id, Title = lesson.Title, Description = lesson.Description,
            VideoUrl = lesson.VideoUrl, DurationMinutes = lesson.DurationMinutes,
            SortOrder = lesson.SortOrder, IsPreview = lesson.IsPreview
        };
    }

    public async Task<bool> DeleteLessonAsync(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) return false;
        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CourseDto>> GetFeaturedCoursesAsync(int count = 4)
    {
        var result = await GetCoursesAsync(new ArtworkListRequest { IsFeatured = true, PageSize = count });
        return result.Items;
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug)
    {
        var exists = await _context.Courses.AnyAsync(c => c.Slug == slug);
        return exists ? $"{slug}-{Guid.NewGuid().ToString()[..6]}" : slug;
    }

    private static string GenerateSlug(string text) =>
        text.ToLowerInvariant().Replace(" ", "-").Replace("_", "-").Replace("/", "-");
}
