using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ArtPlatform.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IConfiguration _config;
    private readonly string _uploadPath;
    private readonly string _baseUrl;

    public FileService(IConfiguration config)
    {
        _config = config;
        _uploadPath = _config["FileStorage:UploadPath"] ?? "wwwroot/uploads";
        _baseUrl = _config["FileStorage:BaseUrl"] ?? "/uploads";
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("الملف غير صالح");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("نوع الملف غير مسموح به. يُسمح فقط بـ JPG, PNG, WebP, GIF");

        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("حجم الملف يتجاوز الحد الأقصى (10 ميجابايت)");

        var uploadDir = Path.Combine(_uploadPath, folder);
        Directory.CreateDirectory(uploadDir);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"{_baseUrl}/{folder}/{fileName}";
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = filePath.Replace(_baseUrl, _uploadPath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public string GetFileUrl(string filePath) => filePath;
}
