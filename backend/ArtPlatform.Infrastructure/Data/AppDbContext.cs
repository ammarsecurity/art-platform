using ArtPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Artwork> Artworks => Set<Artwork>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ArtworkTag> ArtworkTags => Set<ArtworkTag>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ArtworkTag composite key
        modelBuilder.Entity<ArtworkTag>()
            .HasKey(at => new { at.ArtworkId, at.TagId });

        modelBuilder.Entity<ArtworkTag>()
            .HasOne(at => at.Artwork)
            .WithMany(a => a.ArtworkTags)
            .HasForeignKey(at => at.ArtworkId);

        modelBuilder.Entity<ArtworkTag>()
            .HasOne(at => at.Tag)
            .WithMany(t => t.ArtworkTags)
            .HasForeignKey(at => at.TagId);

        // Unique constraints
        modelBuilder.Entity<User>().HasIndex(u => u.Phone).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => c.Slug).IsUnique();
        modelBuilder.Entity<Artwork>().HasIndex(a => a.Slug).IsUnique();
        modelBuilder.Entity<Course>().HasIndex(c => c.Slug).IsUnique();
        modelBuilder.Entity<BlogPost>().HasIndex(b => b.Slug).IsUnique();
        modelBuilder.Entity<Tag>().HasIndex(t => t.Slug).IsUnique();
        modelBuilder.Entity<SiteSettings>().HasIndex(s => s.Key).IsUnique();

        // Enrollment: unique user+course
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

        // LessonProgress: unique user+lesson
        modelBuilder.Entity<LessonProgress>()
            .HasIndex(lp => new { lp.UserId, lp.LessonId }).IsUnique();

        // Decimal precision
        modelBuilder.Entity<Course>()
            .Property(c => c.Price)
            .HasPrecision(10, 2);

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SiteSettings>().HasData(
            new SiteSettings { Id = 1, Key = "site_name", Value = "منصة الفن", Description = "اسم الموقع" },
            new SiteSettings { Id = 2, Key = "site_description", Value = "منصة للفنون والتعلم", Description = "وصف الموقع" },
            new SiteSettings { Id = 3, Key = "contact_email", Value = "info@artplatform.com", Description = "بريد التواصل" },
            new SiteSettings { Id = 4, Key = "hero_title", Value = "استكشف عالم الفن", Description = "عنوان البانر الرئيسي" },
            new SiteSettings { Id = 5, Key = "hero_subtitle", Value = "تعلم، استلهم، وأبدع مع أفضل الفنانين", Description = "نص البانر الفرعي" },
            new SiteSettings { Id = 6, Key = "terms_of_use", Value = "# شروط الاستخدام\n\nمرحباً بك في منصة الفن. باستخدامك للموقع فأنت توافق على الشروط التالية:\n\n## 1. قبول الشروط\nباستخدامك لهذه المنصة، فإنك توافق على الالتزام بهذه الشروط والأحكام.\n\n## 2. استخدام الخدمة\nيُتاح الوصول إلى المنصة للأفراد الذين أتمّوا تسجيل حسابات شخصية.\n\n## 3. الملكية الفكرية\nجميع المحتويات المنشورة على المنصة محمية بموجب قوانين حقوق الملكية الفكرية.\n\n## 4. المسؤولية\nلا تتحمل المنصة أي مسؤولية عن أي أضرار ناجمة عن استخدام الموقع.", Description = "شروط الاستخدام" },
            new SiteSettings { Id = 7, Key = "privacy_policy", Value = "# سياسة الخصوصية\n\nنحن نحترم خصوصيتك ونلتزم بحماية بياناتك الشخصية.\n\n## 1. المعلومات التي نجمعها\nنجمع المعلومات التي تقدمها عند التسجيل مثل الاسم ورقم الهاتف.\n\n## 2. كيف نستخدم معلوماتك\nنستخدم معلوماتك لتقديم الخدمات وتحسين تجربتك على المنصة.\n\n## 3. مشاركة البيانات\nلا نبيع أو نشارك بياناتك الشخصية مع أطراف ثالثة دون موافقتك.\n\n## 4. حماية البيانات\nنستخدم تقنيات تشفير متقدمة لحماية بياناتك.", Description = "سياسة الخصوصية" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "لوحات زيتية", Slug = "oil-paintings", SortOrder = 1, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 2, Name = "رسم رقمي", Slug = "digital-art", SortOrder = 2, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 3, Name = "تصوير فوتوغرافي", Slug = "photography", SortOrder = 3, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 4, Name = "فن تجريدي", Slug = "abstract-art", SortOrder = 4, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) }
        );
    }
}
