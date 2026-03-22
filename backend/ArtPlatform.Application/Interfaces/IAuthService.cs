using ArtPlatform.Application.DTOs;

namespace ArtPlatform.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<List<CourseDto>> GetMyCoursesAsync(int userId);
}
