namespace ArtPlatform.Application.Interfaces;

/// <summary>Converts stored relative paths to absolute URLs for API responses.</summary>
public interface IMediaUrlBuilder
{
    /// <summary>Returns absolute http(s) URL, or leaves already-absolute and external URLs unchanged.</summary>
    string? ToAbsolute(string? path);
}
