using ArtPlatform.Application.DTOs;

namespace ArtPlatform.Application.Interfaces;

public interface ITestimonialService
{
    Task<List<TestimonialDto>> GetAllAsync(bool activeOnly = false);
    Task<TestimonialDto> CreateAsync(CreateTestimonialRequest request);
    Task<TestimonialDto?> UpdateAsync(int id, CreateTestimonialRequest request);
    Task<bool> DeleteAsync(int id);
    Task<TestimonialDto?> ToggleActiveAsync(int id);
}
