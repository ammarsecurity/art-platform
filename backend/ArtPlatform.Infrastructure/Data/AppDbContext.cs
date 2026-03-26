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
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();

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
            new SiteSettings { Id = 1, Key = "site_name", Value = "منصة مرتضى ثامر", Description = "اسم الموقع" },
            new SiteSettings { Id = 2, Key = "site_description", Value = "منصة للفنون والتعلم", Description = "وصف الموقع" },
            new SiteSettings { Id = 3, Key = "contact_email", Value = "info@artplatform.com", Description = "بريد التواصل" },
            new SiteSettings { Id = 4, Key = "hero_title", Value = "استكشف عالم الفن", Description = "عنوان البانر الرئيسي" },
            new SiteSettings { Id = 5, Key = "hero_subtitle", Value = "تعلم، استلهم، وأبدع مع أفضل الفنانين", Description = "نص البانر الفرعي" },
            new SiteSettings { Id = 6, Key = "terms_of_use", Value = "# شروط الاستخدام\n\nمرحباً بك في منصة مرتضى ثامر. باستخدامك للموقع فأنت توافق على الشروط التالية:\n\n## 1. قبول الشروط\nباستخدامك لهذه المنصة، فإنك توافق على الالتزام بهذه الشروط والأحكام.\n\n## 2. استخدام الخدمة\nيُتاح الوصول إلى المنصة للأفراد الذين أتمّوا تسجيل حسابات شخصية.\n\n## 3. الملكية الفكرية\nجميع المحتويات المنشورة على المنصة محمية بموجب قوانين حقوق الملكية الفكرية.\n\n## 4. المسؤولية\nلا تتحمل المنصة أي مسؤولية عن أي أضرار ناجمة عن استخدام الموقع.", Description = "شروط الاستخدام" },
            new SiteSettings { Id = 7, Key = "privacy_policy", Value = "# سياسة الخصوصية\n\nنحن نحترم خصوصيتك ونلتزم بحماية بياناتك الشخصية.\n\n## 1. المعلومات التي نجمعها\nنجمع المعلومات التي تقدمها عند التسجيل مثل الاسم ورقم الهاتف.\n\n## 2. كيف نستخدم معلوماتك\nنستخدم معلوماتك لتقديم الخدمات وتحسين تجربتك على المنصة.\n\n## 3. مشاركة البيانات\nلا نبيع أو نشارك بياناتك الشخصية مع أطراف ثالثة دون موافقتك.\n\n## 4. حماية البيانات\nنستخدم تقنيات تشفير متقدمة لحماية بياناتك.", Description = "سياسة الخصوصية" },
            new SiteSettings { Id = 8, Key = "contact_phone", Value = "+966 50 000 0000", Description = "رقم الجوال" },
            new SiteSettings { Id = 9, Key = "contact_hours", Value = "السبت - الخميس، 9 ص - 6 م", Description = "ساعات العمل" },
            new SiteSettings { Id = 10, Key = "contact_location", Value = "الرياض، المملكة العربية السعودية", Description = "الموقع الجغرافي" },
            new SiteSettings { Id = 11, Key = "social_instagram", Value = "#", Description = "رابط انستغرام" },
            new SiteSettings { Id = 12, Key = "social_twitter", Value = "#", Description = "رابط تويتر X" },
            new SiteSettings { Id = 13, Key = "social_youtube", Value = "#", Description = "رابط يوتيوب" },
            // Home page content
            new SiteSettings { Id = 14, Key = "hero_badge", Value = "✨ منصة مرتضى ثامر العربية الأولى", Description = "نص الشارة في البانر الرئيسي" },
            new SiteSettings { Id = 15, Key = "stat_artworks", Value = "+200", Description = "إحصائية: عدد الأعمال الفنية" },
            new SiteSettings { Id = 16, Key = "stat_courses", Value = "+50", Description = "إحصائية: عدد الدورات" },
            new SiteSettings { Id = 17, Key = "stat_students", Value = "+5000", Description = "إحصائية: عدد الطلاب" },
            new SiteSettings { Id = 18, Key = "stat_experience", Value = "15+", Description = "إحصائية: سنوات الخبرة" },
            new SiteSettings { Id = 19, Key = "cta_title", Value = "ابدأ رحلتك الإبداعية اليوم", Description = "عنوان قسم CTA" },
            new SiteSettings { Id = 20, Key = "cta_subtitle", Value = "انضم لآلاف الطلاب وتعلم الفن من أفضل الأساتذة العرب", Description = "وصف قسم CTA" },
            // About section on home page
            new SiteSettings { Id = 21, Key = "home_about_bio_1", Value = "فنان بصري عربي متخصص في الفنون التشكيلية والرقمية. أعمل على توثيق الجمال العربي من خلال ريشتي، وأسعى لنشر ثقافة الفن وتعليمه للجميع.", Description = "الفقرة الأولى في قسم عن الفنان (الرئيسية)" },
            new SiteSettings { Id = 22, Key = "home_about_bio_2", Value = "من خلال منصتي، أشاركك رحلتي الإبداعية وأعلمك أسرار الفن خطوة بخطوة.", Description = "الفقرة الثانية في قسم عن الفنان (الرئيسية)" },
            new SiteSettings { Id = 23, Key = "home_about_experience", Value = "+15", Description = "قيمة سنوات الخبرة في الرئيسية" },
            new SiteSettings { Id = 24, Key = "artist_image_url", Value = "https://picsum.photos/600/600?grayscale", Description = "رابط صورة الفنان" },
            // About page content
            new SiteSettings { Id = 25, Key = "about_bio_1", Value = "بدأت رحلتي مع الفن منذ أكثر من 15 عاماً، عندما اكتشفت أن اللوحة البيضاء هي أفضل صديق يمكنك أن تجد.", Description = "الفقرة الأولى في صفحة عن الفنان" },
            new SiteSettings { Id = 26, Key = "about_bio_2", Value = "درست الفنون الجميلة وتخصصت في الرسم الزيتي والفن الرقمي، ثم انتقلت إلى تعليم الفن للأجيال الجديدة.", Description = "الفقرة الثانية في صفحة عن الفنان" },
            new SiteSettings { Id = 27, Key = "about_experience", Value = "15+", Description = "سنوات الخبرة في صفحة عن الفنان" },
            new SiteSettings { Id = 28, Key = "about_skills", Value = "[{\"icon\":\"🖌️\",\"name\":\"رسم زيتي\",\"level\":95},{\"icon\":\"💻\",\"name\":\"فن رقمي\",\"level\":88},{\"icon\":\"📷\",\"name\":\"تصوير\",\"level\":75},{\"icon\":\"✏️\",\"name\":\"رسم يدوي\",\"level\":92}]", Description = "مهارات الفنان (JSON)" },
            new SiteSettings { Id = 29, Key = "about_awards", Value = "[{\"title\":\"أفضل فنان عربي\",\"org\":\"مهرجان الفنون العربية\",\"year\":\"2024\"},{\"title\":\"جائزة الإبداع\",\"org\":\"وزارة الثقافة\",\"year\":\"2023\"},{\"title\":\"المعلم المميز\",\"org\":\"منصة تعليمية دولية\",\"year\":\"2022\"}]", Description = "جوائز الفنان (JSON)" },
            // Footer
            new SiteSettings { Id = 30, Key = "footer_description", Value = "منصة عربية متخصصة للفنون البصرية والتعليم الإبداعي. استكشف أعمال الفنانين وتعلم مهاراتك من أفضل الأساتذة.", Description = "وصف الفوتر" }
        );

        modelBuilder.Entity<Testimonial>().HasData(
            new Testimonial { Id = 1, Name = "أحمد الكندي", Title = "طالب فنون بصرية", Text = "منصة رائعة جداً! تعلمت في أسابيع ما لم أستطع تعلمه في سنوات. الأسلوب واضح ومحترف.", Rating = 5, IsActive = true, SortOrder = 1, CreatedAt = new DateTime(2024, 1, 1) },
            new Testimonial { Id = 2, Name = "سارة المطيري", Title = "مصممة جرافيك", Text = "الدورات ممتازة وتغطي كل جوانب الفن الرقمي. المدرس يشرح بأسلوب عربي مفهوم.", Rating = 5, IsActive = true, SortOrder = 2, CreatedAt = new DateTime(2024, 1, 1) },
            new Testimonial { Id = 3, Name = "محمد العمري", Title = "فنان هاوي", Text = "المعرض الفني ملهم جداً! الأعمال احترافية وتحفزني على التعلم أكثر كل يوم.", Rating = 5, IsActive = true, SortOrder = 3, CreatedAt = new DateTime(2024, 1, 1) }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "لوحات زيتية", Slug = "oil-paintings", SortOrder = 1, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 2, Name = "رسم رقمي", Slug = "digital-art", SortOrder = 2, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 3, Name = "تصوير فوتوغرافي", Slug = "photography", SortOrder = 3, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
            new Category { Id = 4, Name = "فن تجريدي", Slug = "abstract-art", SortOrder = 4, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) }
        );
    }
}
