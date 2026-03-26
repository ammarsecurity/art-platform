using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using ArtPlatform.Domain.Entities;
using ArtPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Infrastructure.Services;

public class TestimonialService : ITestimonialService
{
    private readonly AppDbContext _context;

    public TestimonialService(AppDbContext context) => _context = context;

    public async Task<List<TestimonialDto>> GetAllAsync(bool activeOnly = false)
    {
        var query = _context.Testimonials.AsQueryable();
        if (activeOnly) query = query.Where(t => t.IsActive);

        return await query
            .OrderBy(t => t.SortOrder)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => ToDto(t))
            .ToListAsync();
    }

    public async Task<TestimonialDto> CreateAsync(CreateTestimonialRequest request)
    {
        var t = new Testimonial
        {
            Name = request.Name, Title = request.Title, Text = request.Text,
            Rating = request.Rating, SortOrder = request.SortOrder, IsActive = request.IsActive
        };
        _context.Testimonials.Add(t);
        await _context.SaveChangesAsync();
        return ToDto(t);
    }

    public async Task<TestimonialDto?> UpdateAsync(int id, CreateTestimonialRequest request)
    {
        var t = await _context.Testimonials.FindAsync(id);
        if (t == null) return null;

        t.Name = request.Name;
        t.Title = request.Title;
        t.Text = request.Text;
        t.Rating = request.Rating;
        t.SortOrder = request.SortOrder;
        t.IsActive = request.IsActive;
        await _context.SaveChangesAsync();
        return ToDto(t);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var t = await _context.Testimonials.FindAsync(id);
        if (t == null) return false;
        _context.Testimonials.Remove(t);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TestimonialDto?> ToggleActiveAsync(int id)
    {
        var t = await _context.Testimonials.FindAsync(id);
        if (t == null) return null;
        t.IsActive = !t.IsActive;
        await _context.SaveChangesAsync();
        return ToDto(t);
    }

    private static TestimonialDto ToDto(Testimonial t) => new()
    {
        Id = t.Id, Name = t.Name, Title = t.Title, Text = t.Text,
        Rating = t.Rating, IsActive = t.IsActive, SortOrder = t.SortOrder, CreatedAt = t.CreatedAt
    };
}
