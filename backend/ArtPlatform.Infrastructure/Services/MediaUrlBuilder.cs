using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ArtPlatform.Infrastructure.Services;

public sealed class MediaUrlBuilder : IMediaUrlBuilder
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MediaUrlBuilder(IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? ToAbsolute(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;

        if (Uri.TryCreate(path, UriKind.Absolute, out _))
            return path;

        var baseUrl = _config["Api:PublicBaseUrl"]?.Trim().TrimEnd('/');
        if (string.IsNullOrEmpty(baseUrl))
        {
            var req = _httpContextAccessor.HttpContext?.Request;
            if (req != null)
                baseUrl = $"{req.Scheme}://{req.Host.Value}".TrimEnd('/');
        }

        if (string.IsNullOrEmpty(baseUrl))
            return path;

        var relative = path.StartsWith('/') ? path : "/" + path;
        return $"{baseUrl}{relative}";
    }
}
