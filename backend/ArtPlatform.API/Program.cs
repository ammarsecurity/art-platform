using ArtPlatform.API.Extensions;
using ArtPlatform.API.Middleware;
using ArtPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// رفع فيديوهات الدروس حتى 500 ميجابايت (الافتراضي ~30 ميجابايت يقطع الاتصال)
const long maxUploadBytes = 524_288_000;
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = maxUploadBytes);
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = maxUploadBytes);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // إذا سُجِّل ترحيل SortOrder في السجل دون تنفيذ ALTER في MySQL (مثلاً بعد استعادة نسخة قديمة)
    try
    {
        await db.Database.ExecuteSqlRawAsync(
            "ALTER TABLE `BlogPosts` ADD COLUMN `SortOrder` INT NOT NULL DEFAULT 0;");
    }
    catch (Exception ex)
    {
        var full = ex.Message;
        for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
            full += " " + inner.Message;
        if (!full.Contains("Duplicate column", StringComparison.OrdinalIgnoreCase)
            && !full.Contains("1060", StringComparison.Ordinal))
            throw;
    }

    await DatabaseSeeder.SeedAsync(db);
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Art Platform API v1"));
}

app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
