using ArtPlatform.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ArtPlatform.Application.Interfaces;

public interface IArtworkService
{
    Task<PagedResult<ArtworkDto>> GetArtworksAsync(ArtworkListRequest request);
    Task<ArtworkDto?> GetArtworkBySlugAsync(string slug);
    Task<ArtworkDto?> GetArtworkByIdAsync(int id);
    Task<ArtworkDto> CreateArtworkAsync(CreateArtworkRequest request, IFormFile image);
    Task<ArtworkDto?> UpdateArtworkAsync(int id, UpdateArtworkRequest request, IFormFile? image);
    Task<bool> DeleteArtworkAsync(int id);
    Task<List<ArtworkDto>> GetFeaturedArtworksAsync(int count = 6);
}
