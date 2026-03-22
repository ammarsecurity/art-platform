using ArtPlatform.Application.DTOs;

namespace ArtPlatform.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task<CategoryDto?> UpdateAsync(int id, CreateCategoryRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}

public interface IFileService
{
    Task<string> UploadImageAsync(Microsoft.AspNetCore.Http.IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath);
}
