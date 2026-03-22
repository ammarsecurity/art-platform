using ArtPlatform.Domain.Entities;
using ArtPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArtPlatform.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // ── Users ────────────────────────────────────────
        if (!await context.Users.AnyAsync())
        {
            context.Users.AddRange(
                new User
                {
                    Name = "المدير",
                    Phone = "0501234567",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@12345"),
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                },
                new User
                {
                    Name = "أحمد الكندي",
                    Phone = "0551234568",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                    Role = UserRole.Student,
                    Bio = "طالب فنون بصرية متحمس لتعلم الرسم الرقمي",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 2, 1)
                },
                new User
                {
                    Name = "سارة المطيري",
                    Phone = "0561234569",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                    Role = UserRole.Student,
                    Bio = "مصممة جرافيك تسعى لتطوير مهاراتها في الفنون الرقمية",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 3, 1)
                }
            );
            await context.SaveChangesAsync();
        }

        // ── Artworks ──────────────────────────────────────
        if (!await context.Artworks.AnyAsync())
        {
            context.Artworks.AddRange(
                new Artwork
                {
                    Title = "أضواء المدينة",
                    Slug = "city-lights",
                    Description = "لوحة زيتية تجسّد جمال المدينة ليلاً، بألوان دافئة تعكس أنوار الشوارع والحياة الليلية.",
                    ImageUrl = "https://picsum.photos/seed/art1/800/600",
                    ThumbnailUrl = "https://picsum.photos/seed/art1/400/300",
                    Medium = "ألوان زيتية على قماش",
                    Dimensions = "80 × 60 سم",
                    Year = 2023,
                    IsFeatured = true,
                    Status = ArtworkStatus.Published,
                    SortOrder = 1,
                    CategoryId = 1,
                    CreatedAt = new DateTime(2024, 1, 15)
                },
                new Artwork
                {
                    Title = "صمت الصحراء",
                    Slug = "desert-silence",
                    Description = "عمل رقمي يستلهم هدوء الصحراء وشساعتها، ألوان دافئة من الذهبي والبرتقالي تمتزج في تناغم رائع.",
                    ImageUrl = "https://picsum.photos/seed/art2/800/600",
                    ThumbnailUrl = "https://picsum.photos/seed/art2/400/300",
                    Medium = "فن رقمي",
                    Dimensions = "4000 × 3000 بكسل",
                    Year = 2023,
                    IsFeatured = true,
                    Status = ArtworkStatus.Published,
                    SortOrder = 2,
                    CategoryId = 2,
                    CreatedAt = new DateTime(2024, 2, 10)
                },
                new Artwork
                {
                    Title = "أحلام الطفولة",
                    Slug = "childhood-dreams",
                    Description = "لوحة مائية تعبّر عن براءة الطفولة وأحلامها اللامحدودة، بألوان زاهية وخطوط طليقة.",
                    ImageUrl = "https://picsum.photos/seed/art3/800/600",
                    ThumbnailUrl = "https://picsum.photos/seed/art3/400/300",
                    Medium = "ألوان مائية",
                    Dimensions = "50 × 40 سم",
                    Year = 2022,
                    IsFeatured = true,
                    Status = ArtworkStatus.Published,
                    SortOrder = 3,
                    CategoryId = 1,
                    CreatedAt = new DateTime(2024, 3, 5)
                },
                new Artwork
                {
                    Title = "وجوه المدينة",
                    Slug = "city-faces",
                    Description = "سلسلة تصوير فوتوغرافي توثّق تنوع وجوه المدينة وقصصها المخفية.",
                    ImageUrl = "https://picsum.photos/seed/art4/800/600",
                    ThumbnailUrl = "https://picsum.photos/seed/art4/400/300",
                    Medium = "تصوير فوتوغرافي",
                    Dimensions = "30 × 45 سم",
                    Year = 2023,
                    IsFeatured = true,
                    Status = ArtworkStatus.Published,
                    SortOrder = 4,
                    CategoryId = 3,
                    CreatedAt = new DateTime(2024, 4, 1)
                },
                new Artwork
                {
                    Title = "دوامة الزمن",
                    Slug = "time-vortex",
                    Description = "عمل تجريدي يعبّر عن تدفق الزمن وتحولاته، بحركات دائرية وألوان متدرجة بين الأزرق والبنفسجي.",
                    ImageUrl = "https://picsum.photos/seed/art5/800/600",
                    ThumbnailUrl = "https://picsum.photos/seed/art5/400/300",
                    Medium = "أكريليك على قماش",
                    Dimensions = "100 × 100 سم",
                    Year = 2024,
                    IsFeatured = true,
                    Status = ArtworkStatus.Published,
                    SortOrder = 5,
                    CategoryId = 4,
                    CreatedAt = new DateTime(2024, 5, 20)
                },
                new Artwork
                {
                    Title = "بحر الذكريات",
                    Slug = "memory-sea",
                    Description = "لوحة رقمية تجمع بين الواقعية والحلمية، موج البحر يحمل صور من الماضي.",
                    ImageUrl = "https://picsum.photos/seed/art6/800/600",
                    ThumbnailUrl = "https://picsum.photos/seed/art6/400/300",
                    Medium = "فن رقمي",
                    Dimensions = "5000 × 3500 بكسل",
                    Year = 2024,
                    IsFeatured = false,
                    Status = ArtworkStatus.Published,
                    SortOrder = 6,
                    CategoryId = 2,
                    CreatedAt = new DateTime(2024, 6, 10)
                }
            );
            await context.SaveChangesAsync();
        }

        // ── Courses ───────────────────────────────────────
        if (!await context.Courses.AnyAsync())
        {
            context.Courses.AddRange(
                new Course
                {
                    Title = "أساسيات الرسم للمبتدئين",
                    Slug = "drawing-basics",
                    ShortDescription = "تعلم أسس الرسم من الصفر — الخطوط، الأشكال، الظل والنور",
                    Description = "دورة شاملة للمبتدئين تغطي جميع أساسيات الرسم. ستتعلم كيفية رسم الخطوط الصحيحة، بناء الأشكال الهندسية، إضافة الظل والنور، وإنشاء عمق في رسوماتك. الدورة تحتوي على تمارين عملية يومية ومشاريع تطبيقية.",
                    ThumbnailUrl = "https://picsum.photos/seed/course1/600/400",
                    Price = 0,
                    DurationMinutes = 480,
                    Level = CourseLevel.Beginner,
                    Status = CourseStatus.Published,
                    IsFeatured = true,
                    CategoryId = 1,
                    CreatedAt = new DateTime(2024, 1, 1)
                },
                new Course
                {
                    Title = "الرسم الرقمي مع Procreate",
                    Slug = "digital-art-procreate",
                    ShortDescription = "احترف الرسم الرقمي باستخدام تطبيق Procreate على iPad",
                    Description = "تعلم كيفية استخدام تطبيق Procreate لإنشاء أعمال فنية رقمية احترافية. تشمل الدورة: إعداد مساحة العمل، استخدام الفرش المختلفة، تقنيات التلوين والظلال، وإنشاء مشاريع كاملة من الصفر.",
                    ThumbnailUrl = "https://picsum.photos/seed/course2/600/400",
                    Price = 149,
                    DurationMinutes = 720,
                    Level = CourseLevel.Intermediate,
                    Status = CourseStatus.Published,
                    IsFeatured = true,
                    CategoryId = 2,
                    CreatedAt = new DateTime(2024, 2, 1)
                },
                new Course
                {
                    Title = "التصوير الفوتوغرافي الاحترافي",
                    Slug = "professional-photography",
                    ShortDescription = "من المبتدئ إلى المحترف — أتقن التصوير بكاميرتك",
                    Description = "دورة متكاملة في التصوير الفوتوغرافي تبدأ من أساسيات الكاميرا وتصل إلى تقنيات التصوير الاحترافي. ستتعلم: إعدادات الكاميرا، قواعد التكوين، التعامل مع الإضاءة الطبيعية والصناعية، ومعالجة الصور باحترافية.",
                    ThumbnailUrl = "https://picsum.photos/seed/course3/600/400",
                    Price = 199,
                    DurationMinutes = 960,
                    Level = CourseLevel.Beginner,
                    Status = CourseStatus.Published,
                    IsFeatured = true,
                    CategoryId = 3,
                    CreatedAt = new DateTime(2024, 3, 1)
                },
                new Course
                {
                    Title = "الفن التجريدي — التعبير بلا قيود",
                    Slug = "abstract-art",
                    ShortDescription = "اكتشف حرية التعبير في الفن التجريدي",
                    Description = "استكشف عالم الفن التجريدي وتعلم كيف تعبّر عن مشاعرك وأفكارك من خلال الألوان والأشكال. الدورة تشمل: تاريخ الفن التجريدي، مختلف الأساليب والتقنيات، واستخدام الألوان الأكريليكية وتقنيات الطلاء المختلفة.",
                    ThumbnailUrl = "https://picsum.photos/seed/course4/600/400",
                    Price = 129,
                    DurationMinutes = 600,
                    Level = CourseLevel.Intermediate,
                    Status = CourseStatus.Published,
                    IsFeatured = true,
                    CategoryId = 4,
                    CreatedAt = new DateTime(2024, 4, 1)
                }
            );
            await context.SaveChangesAsync();

            // ── Lessons ───────────────────────────────────
            var courses = await context.Courses.ToListAsync();
            var drawingCourse = courses.First(c => c.Slug == "drawing-basics");
            var digitalCourse = courses.First(c => c.Slug == "digital-art-procreate");

            context.Lessons.AddRange(
                // Drawing course lessons
                new Lesson { Title = "مقدمة ومستلزمات الرسم", CourseId = drawingCourse.Id, DurationMinutes = 15, SortOrder = 1, IsPreview = true, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Lesson { Title = "أنواع الخطوط وتقنياتها", CourseId = drawingCourse.Id, DurationMinutes = 25, SortOrder = 2, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Lesson { Title = "رسم الأشكال الهندسية الأساسية", CourseId = drawingCourse.Id, DurationMinutes = 35, SortOrder = 3, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Lesson { Title = "مبادئ الظل والنور", CourseId = drawingCourse.Id, DurationMinutes = 40, SortOrder = 4, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Lesson { Title = "رسم الأجسام ثلاثية الأبعاد", CourseId = drawingCourse.Id, DurationMinutes = 45, SortOrder = 5, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Lesson { Title = "مشروع نهائي — رسم طبيعة صامتة", CourseId = drawingCourse.Id, DurationMinutes = 60, SortOrder = 6, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 1, 1) },

                // Digital course lessons
                new Lesson { Title = "التعرف على واجهة Procreate", CourseId = digitalCourse.Id, DurationMinutes = 20, SortOrder = 1, IsPreview = true, IsActive = true, CreatedAt = new DateTime(2024, 2, 1) },
                new Lesson { Title = "الطبقات (Layers) وكيفية استخدامها", CourseId = digitalCourse.Id, DurationMinutes = 30, SortOrder = 2, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 2, 1) },
                new Lesson { Title = "الفرش الاحترافية والإعدادات", CourseId = digitalCourse.Id, DurationMinutes = 35, SortOrder = 3, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 2, 1) },
                new Lesson { Title = "تقنيات التلوين الرقمي", CourseId = digitalCourse.Id, DurationMinutes = 50, SortOrder = 4, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 2, 1) },
                new Lesson { Title = "رسم بورتريه رقمي كامل", CourseId = digitalCourse.Id, DurationMinutes = 90, SortOrder = 5, IsPreview = false, IsActive = true, CreatedAt = new DateTime(2024, 2, 1) }
            );
            await context.SaveChangesAsync();
        }

        // ── Blog Posts ────────────────────────────────────
        if (!await context.BlogPosts.AnyAsync())
        {
            context.BlogPosts.AddRange(
                new BlogPost
                {
                    Title = "10 نصائح لتحسين مهاراتك في الرسم",
                    Slug = "10-drawing-tips",
                    Excerpt = "سواء كنت مبتدئاً أو متقدماً، هذه النصائح العشر ستساعدك على الارتقاء بمستواك الفني بشكل ملحوظ.",
                    Content = "## مقدمة\n\nالرسم مهارة يمكن لأي شخص تعلمها وتطويرها. في هذا المقال نشارككم 10 نصائح مجربة لتحسين مهاراتكم.\n\n## 1. الممارسة اليومية\nخصص 15 دقيقة يومياً للرسم. الاستمرارية أهم من الجلسات الطويلة المتقطعة.\n\n## 2. ارسم ما تراه\nتدرّب على الرسم من الحياة الواقعية — الأشياء المحيطة بك خير معلم.\n\n## 3. تعلم من الأساتذة\nادرس أعمال الفنانين الكبار وحاول فهم أسلوبهم.\n\n## 4. لا تخشَ الأخطاء\nكل خطأ هو فرصة للتعلم. الفنانون الكبار مروا بآلاف الأخطاء.\n\n## 5. جرّب أدوات مختلفة\nجرب الرصاص، الفحم، الألوان المائية — كل أداة تعلمك شيئاً جديداً.",
                    FeaturedImageUrl = "https://picsum.photos/seed/blog1/800/400",
                    Status = PostStatus.Published,
                    IsFeatured = true,
                    ViewCount = 1250,
                    PublishedAt = new DateTime(2024, 3, 15),
                    CreatedAt = new DateTime(2024, 3, 15),
                    MetaTitle = "10 نصائح لتحسين مهارات الرسم",
                    MetaDescription = "نصائح عملية لتطوير مهاراتك في الرسم سواء كنت مبتدئاً أو محترفاً"
                },
                new BlogPost
                {
                    Title = "دليلك الشامل للبدء في الفن الرقمي",
                    Slug = "digital-art-guide",
                    Excerpt = "الفن الرقمي عالم واسع ومثير. تعرف على أفضل الأدوات والبرامج للبدء رحلتك الإبداعية الرقمية.",
                    Content = "## الفن الرقمي — البداية الصحيحة\n\nيتساءل كثيرون: من أين أبدأ في الفن الرقمي؟ هذا الدليل يجيب على تساؤلاتكم.\n\n## الأدوات الأساسية\n\n### اللوحات الرقمية\n- **iPad مع Apple Pencil** — الأفضل للمبتدئين\n- **Wacom Intuos** — خيار ممتاز للحاسوب\n- **Huion** — بديل اقتصادي جيد\n\n### البرامج\n- **Procreate** — الأشهر لمستخدمي iPad\n- **Adobe Fresco** — مميزات رائعة للرسم الطبيعي\n- **Krita** — مجاني ومفتوح المصدر\n\n## الخطوات الأولى\nابدأ بتعلم واجهة البرنامج، ثم مارس الرسم البسيط قبل الانتقال للمشاريع المعقدة.",
                    FeaturedImageUrl = "https://picsum.photos/seed/blog2/800/400",
                    Status = PostStatus.Published,
                    IsFeatured = true,
                    ViewCount = 890,
                    PublishedAt = new DateTime(2024, 4, 1),
                    CreatedAt = new DateTime(2024, 4, 1),
                    MetaTitle = "دليل الفن الرقمي للمبتدئين",
                    MetaDescription = "كل ما تحتاج معرفته للبدء في الفن الرقمي — الأدوات والبرامج والخطوات الأولى"
                },
                new BlogPost
                {
                    Title = "كيف تبني أسلوبك الفني الخاص",
                    Slug = "find-your-art-style",
                    Excerpt = "الأسلوب الفني الشخصي لا يأتي بين ليلة وضحاها — إنه رحلة اكتشاف ذات وإبداع.",
                    Content = "## ما هو الأسلوب الفني؟\n\nالأسلوب الفني هو بصمتك الشخصية في عالم الفن — طريقتك الفريدة في التعبير.\n\n## كيف تطور أسلوبك\n\n### 1. ادرس فنانين متعددين\nلا تقيد نفسك بفنان واحد. استلهم من عشرات الأساليب.\n\n### 2. جرّب باستمرار\nكل تجربة تضيف شيئاً لأسلوبك.\n\n### 3. ارسم ما تحب\nالشغف يظهر في الأعمال. ارسم الموضوعات التي تحبها.\n\n### 4. تقبّل التطور\nأسلوبك سيتغير مع الوقت وهذا طبيعي وصحي.\n\n## الخلاصة\nالأسلوب الفني يأتي تلقائياً عندما ترسم بانتظام وتبحث دائماً عن التطوير.",
                    FeaturedImageUrl = "https://picsum.photos/seed/blog3/800/400",
                    Status = PostStatus.Published,
                    IsFeatured = false,
                    ViewCount = 620,
                    PublishedAt = new DateTime(2024, 5, 10),
                    CreatedAt = new DateTime(2024, 5, 10),
                    MetaTitle = "كيف تطور أسلوبك الفني الخاص",
                    MetaDescription = "نصائح لاكتشاف وتطوير أسلوبك الفني الشخصي الفريد"
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
