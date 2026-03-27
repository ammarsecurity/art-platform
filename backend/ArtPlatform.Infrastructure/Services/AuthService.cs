using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using ArtPlatform.Domain.Entities;
using ArtPlatform.Domain.Enums;
using ArtPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ArtPlatform.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IMediaUrlBuilder _mediaUrl;

    public AuthService(AppDbContext context, IConfiguration config, IMediaUrlBuilder mediaUrl)
    {
        _context = context;
        _config = config;
        _mediaUrl = mediaUrl;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Phone == request.Phone && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("بيانات الدخول غير صحيحة");

        return GenerateTokens(user);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Phone == request.Phone))
            throw new InvalidOperationException("رقم الهاتف مستخدم بالفعل");

        var user = new User
        {
            Name = request.Name,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return GenerateTokens(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Role = user.Role.ToString(),
            AvatarUrl = _mediaUrl.ToAbsolute(user.AvatarUrl),
            Bio = user.Bio,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        // Validate phone uniqueness if changed
        if (!string.IsNullOrEmpty(request.Phone) && request.Phone != user.Phone)
        {
            if (await _context.Users.AnyAsync(u => u.Phone == request.Phone && u.Id != userId))
                throw new InvalidOperationException("رقم الهاتف مستخدم بالفعل");
            user.Phone = request.Phone;
        }

        // Change password if provided
        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("كلمة المرور الحالية غير صحيحة");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        }

        user.Name = request.Name;
        user.Bio = request.Bio;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Role = user.Role.ToString(),
            AvatarUrl = _mediaUrl.ToAbsolute(user.AvatarUrl),
            Bio = user.Bio,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<List<CourseDto>> GetMyCoursesAsync(int userId)
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Course).ThenInclude(c => c.Category)
            .Include(e => e.Course).ThenInclude(c => c.Lessons)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        return enrollments.Select(e =>
        {
            var course = e.Course;
            var totalLessons = course.Lessons.Count;
            var completedLessons = _context.LessonProgresses
                .Count(lp => lp.UserId == userId && lp.IsCompleted && course.Lessons.Select(l => l.Id).Contains(lp.LessonId));

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Slug = course.Slug,
                ShortDescription = course.ShortDescription,
                ThumbnailUrl = _mediaUrl.ToAbsolute(course.ThumbnailUrl),
                Price = course.Price,
                DurationMinutes = course.DurationMinutes,
                Level = course.Level.ToString(),
                Status = course.Status.ToString(),
                IsFeatured = course.IsFeatured,
                CategoryId = course.CategoryId,
                CategoryName = course.Category.Name,
                LessonCount = totalLessons,
                IsEnrolled = true,
                ProgressPercent = totalLessons == 0 ? 0 : (int)Math.Round((double)completedLessons / totalLessons * 100),
                CreatedAt = course.CreatedAt
            };
        }).ToList();
    }

    private AuthResponse GenerateTokens(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.Phone),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                Role = user.Role.ToString(),
                AvatarUrl = _mediaUrl.ToAbsolute(user.AvatarUrl)
            }
        };
    }
}
