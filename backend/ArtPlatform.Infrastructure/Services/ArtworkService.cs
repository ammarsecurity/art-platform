using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using ArtPlatform.Domain.Entities;
using ArtPlatform.Domain.Enums;
using ArtPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Infrastructure.Services;

public class ArtworkService : IArtworkService
{
    private readonly AppDbContext _context;
    private readonly IFileService _fileService;
    private readonly IMediaUrlBuilder _mediaUrl;

    public ArtworkService(AppDbContext context, IFileService fileService, IMediaUrlBuilder mediaUrl)
    {
        _context = context;
        _fileService = fileService;
        _mediaUrl = mediaUrl;
    }

    public async Task<PagedResult<ArtworkDto>> GetArtworksAsync(ArtworkListRequest request)
    {
        var query = _context.Artworks
            .Include(a => a.Category)
            .Include(a => a.ArtworkTags).ThenInclude(at => at.Tag)
            .AsQueryable();

        if (request.CategoryId.HasValue)
            query = query.Where(a => a.CategoryId == request.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(a => a.Title.Contains(request.Search) || (a.Description != null && a.Description.Contains(request.Search)));

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<ArtworkStatus>(request.Status, out var status))
            query = query.Where(a => a.Status == status);
        else
            query = query.Where(a => a.Status == ArtworkStatus.Published);

        if (request.IsFeatured.HasValue)
            query = query.Where(a => a.IsFeatured == request.IsFeatured.Value);

        query = request.SortBy switch
        {
            "title" => request.SortOrder == "asc" ? query.OrderBy(a => a.Title) : query.OrderByDescending(a => a.Title),
            "year" => request.SortOrder == "asc" ? query.OrderBy(a => a.Year) : query.OrderByDescending(a => a.Year),
            _ => request.SortOrder == "asc" ? query.OrderBy(a => a.CreatedAt) : query.OrderByDescending(a => a.CreatedAt)
        };

        var total = await query.CountAsync();
        var rows = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        var items = rows.Select(MapToDto).ToList();

        return new PagedResult<ArtworkDto>
        {
            Items = items, TotalCount = total,
            Page = request.Page, PageSize = request.PageSize
        };
    }

    public async Task<ArtworkDto?> GetArtworkBySlugAsync(string slug)
    {
        var artwork = await _context.Artworks
            .Include(a => a.Category)
            .Include(a => a.ArtworkTags).ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Slug == slug && a.Status == ArtworkStatus.Published);

        return artwork == null ? null : MapToDto(artwork);
    }

    public async Task<ArtworkDto?> GetArtworkByIdAsync(int id)
    {
        var artwork = await _context.Artworks
            .Include(a => a.Category)
            .Include(a => a.ArtworkTags).ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Id == id);

        return artwork == null ? null : MapToDto(artwork);
    }

    public async Task<ArtworkDto> CreateArtworkAsync(CreateArtworkRequest request, IFormFile image)
    {
        var imageUrl = await _fileService.UploadImageAsync(image, "artworks");
        var slug = GenerateSlug(request.Title);

        var artwork = new Artwork
        {
            Title = request.Title,
            Slug = await EnsureUniqueSlugAsync(slug),
            Description = request.Description,
            ImageUrl = imageUrl,
            Medium = request.Medium,
            Dimensions = request.Dimensions,
            Year = request.Year,
            IsFeatured = request.IsFeatured,
            CategoryId = request.CategoryId,
            Status = ArtworkStatus.Published
        };

        _context.Artworks.Add(artwork);
        await _context.SaveChangesAsync();

        await UpdateTagsAsync(artwork.Id, request.Tags);

        return (await GetArtworkByIdAsync(artwork.Id))!;
    }

    public async Task<ArtworkDto?> UpdateArtworkAsync(int id, UpdateArtworkRequest request, IFormFile? image)
    {
        var artwork = await _context.Artworks.FindAsync(id);
        if (artwork == null) return null;

        if (image != null)
            artwork.ImageUrl = await _fileService.UploadImageAsync(image, "artworks");

        artwork.Title = request.Title;
        artwork.Description = request.Description;
        artwork.Medium = request.Medium;
        artwork.Dimensions = request.Dimensions;
        artwork.Year = request.Year;
        artwork.IsFeatured = request.IsFeatured;
        artwork.CategoryId = request.CategoryId;
        artwork.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ArtworkStatus>(request.Status, out var status))
            artwork.Status = status;

        await _context.SaveChangesAsync();
        await UpdateTagsAsync(id, request.Tags);

        return await GetArtworkByIdAsync(id);
    }

    public async Task<bool> DeleteArtworkAsync(int id)
    {
        var artwork = await _context.Artworks.FindAsync(id);
        if (artwork == null) return false;
        _context.Artworks.Remove(artwork);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ArtworkDto>> GetFeaturedArtworksAsync(int count = 6)
    {
        var rows = await _context.Artworks
            .Include(a => a.Category)
            .Include(a => a.ArtworkTags).ThenInclude(at => at.Tag)
            .Where(a => a.IsFeatured && a.Status == ArtworkStatus.Published)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
        return rows.Select(MapToDto).ToList();
    }

    private async Task UpdateTagsAsync(int artworkId, List<string> tagNames)
    {
        var existing = await _context.ArtworkTags.Where(at => at.ArtworkId == artworkId).ToListAsync();
        _context.ArtworkTags.RemoveRange(existing);

        foreach (var tagName in tagNames.Distinct())
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName)
                ?? new Tag { Name = tagName, Slug = GenerateSlug(tagName) };

            if (tag.Id == 0)
            {
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
            _context.ArtworkTags.Add(new ArtworkTag { ArtworkId = artworkId, TagId = tag.Id });
        }
        await _context.SaveChangesAsync();
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug)
    {
        var exists = await _context.Artworks.AnyAsync(a => a.Slug == slug);
        return exists ? $"{slug}-{Guid.NewGuid().ToString()[..6]}" : slug;
    }

    private static string GenerateSlug(string text)
    {
        return text.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace("/", "-");
    }

    private ArtworkDto MapToDto(Artwork a) => new()
    {
        Id = a.Id, Title = a.Title, Slug = a.Slug,
        Description = a.Description,
        ImageUrl = _mediaUrl.ToAbsolute(a.ImageUrl) ?? "",
        ThumbnailUrl = _mediaUrl.ToAbsolute(a.ThumbnailUrl),
        Medium = a.Medium,
        Dimensions = a.Dimensions, Year = a.Year,
        IsFeatured = a.IsFeatured, Status = a.Status.ToString(),
        CategoryId = a.CategoryId, CategoryName = a.Category?.Name ?? "",
        Tags = a.ArtworkTags?.Select(at => at.Tag.Name).ToList() ?? new(),
        CreatedAt = a.CreatedAt
    };
}
