using ArtPlatform.Application.DTOs;
using ArtPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(new { success = true, data = result });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var category = await _categoryService.CreateAsync(request);
        return Created("", new { success = true, data = category });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryRequest request)
    {
        var category = await _categoryService.UpdateAsync(id, request);
        return category == null ? NotFound() : Ok(new { success = true, data = category });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        return result ? Ok(new { success = true }) : NotFound();
    }
}

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public AdminController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var stats = await _dashboardService.GetStatsAsync();
        return Ok(new { success = true, data = stats });
    }
}
