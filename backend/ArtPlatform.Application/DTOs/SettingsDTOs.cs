namespace ArtPlatform.Application.DTOs;

/// <summary>إعدادات الصفحة الرئيسية (مخزنة كـ JSON في SiteSettings.home_page_config)</summary>
public class HomePageConfigDto
{
    public string HeroBadge { get; set; } = "✨ منصة الفن العربية الأولى";
    public string HeroTitleLine1 { get; set; } = "استكشف عالم";
    public string HeroTitleLine2 { get; set; } = "الفن الإبداعي";
    public string HeroParagraph { get; set; } =
        "تعلم، استلهم، وأبدع مع أفضل الفنانين العرب. دورات تعليمية احترافية ومعرض فني استثنائي بين يديك.";

    public List<HomeStatDto> Stats { get; set; } = new()
    {
        new HomeStatDto { Value = "+200", Label = "عمل فني" },
        new HomeStatDto { Value = "+50", Label = "دورة تعليمية" },
        new HomeStatDto { Value = "+5000", Label = "طالب" },
        new HomeStatDto { Value = "15+", Label = "سنة خبرة" }
    };

    public HomeAboutDto About { get; set; } = new();

    public List<HomeTestimonialDto> Testimonials { get; set; } = new()
    {
        new HomeTestimonialDto
        {
            Name = "أحمد الكندي",
            Title = "طالب فنون بصرية",
            Text = "منصة رائعة جداً! تعلمت في أسابيع ما لم أستطع تعلمه في سنوات. الأسلوب واضح ومحترف."
        },
        new HomeTestimonialDto
        {
            Name = "سارة المطيري",
            Title = "مصممة جرافيك",
            Text = "الدورات ممتازة وتغطي كل جوانب الفن الرقمي. المدرس يشرح بأسلوب عربي مفهوم."
        },
        new HomeTestimonialDto
        {
            Name = "محمد العمري",
            Title = "فنان هاوي",
            Text = "المعرض الفني ملهم جداً! الأعمال احترافية وتحفزني على التعلم أكثر كل يوم."
        }
    };

    public HomeCtaDto Cta { get; set; } = new();

    /// <summary>سلايدر الصور في أعلى الصفحة الرئيسية</summary>
    public HomeSliderConfigDto Slider { get; set; } = new();
}

/// <summary>إعدادات سلايدر الصور (صور كبيرة + نصوص اختيارية لكل شريحة)</summary>
public class HomeSliderConfigDto
{
    public bool Enabled { get; set; } = true;

    /// <summary>مدة عرض كل شريحة بالثواني (3–120)</summary>
    public int IntervalSeconds { get; set; } = 8;

    public List<HomeSliderItemDto> Items { get; set; } = new()
    {
        new HomeSliderItemDto
        {
            ImageUrl = "https://picsum.photos/seed/arthome1/1920/960",
            Title = "عالم الفن بين يديك",
            Subtitle = "تعلّم، استلهم، واطلق إبداعك مع محتوى عربي احترافي",
            LinkUrl = "/courses",
            LinkLabel = "استكشف الدورات"
        },
        new HomeSliderItemDto
        {
            ImageUrl = "https://picsum.photos/seed/arthome2/1920/960",
            Title = "معرض فني يُلهمك",
            Subtitle = "أعمال مختارة من فنانين عرب",
            LinkUrl = "/portfolio",
            LinkLabel = "زيارة المعرض"
        },
        new HomeSliderItemDto
        {
            ImageUrl = "https://picsum.photos/seed/arthome3/1920/960",
            Title = "انضم لمجتمعنا",
            Subtitle = "آلاف الطلاب يتعلمون الفن كل يوم",
            LinkUrl = "/register",
            LinkLabel = "سجّل مجاناً"
        }
    };
}

public class HomeSliderItemDto
{
    public string ImageUrl { get; set; } = "";
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    /// <summary>مسار داخلي مثل /portfolio أو رابط كامل</summary>
    public string LinkUrl { get; set; } = "";
    public string LinkLabel { get; set; } = "اكتشف المزيد";
}

public class HomeStatDto
{
    public string Value { get; set; } = "";
    public string Label { get; set; } = "";
}

public class HomeAboutDto
{
    public string ImageUrl { get; set; } = "https://picsum.photos/600/600?grayscale";
    public string Badge { get; set; } = "👨‍🎨 عن الفنان";
    public string TitleLine1 { get; set; } = "رحلة إبداعية";
    public string TitleLine2 { get; set; } = "لا حدود لها";
    public string Paragraph1 { get; set; } =
        "فنان بصري عربي متخصص في الفنون التشكيلية والرقمية. أعمل على توثيق الجمال العربي من خلال ريشتي، وأسعى لنشر ثقافة الفن وتعليمه للجميع.";
    public string Paragraph2 { get; set; } =
        "من خلال منصتي، أشاركك رحلتي الإبداعية وأعلمك أسرار الفن خطوة بخطوة.";
    public string CardValue { get; set; } = "+15";
    public string CardLabel { get; set; } = "سنة خبرة في الفنون";
}

public class HomeTestimonialDto
{
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
}

public class HomeCtaDto
{
    public string Title { get; set; } = "ابدأ رحلتك الإبداعية اليوم";
    public string Subtitle { get; set; } = "انضم لآلاف الطلاب وتعلم الفن من أفضل الأساتذة العرب";
}

/// <summary>محتوى صفحة التواصل (مخزن كـ JSON في SiteSettings.contact_page_config)</summary>
public class ContactPageConfigDto
{
    public string HeaderBadge { get; set; } = "📬 تواصل معنا";
    public string HeaderTitle { get; set; } = "نسعد بتواصلك";
    public string HeaderSubtitle { get; set; } =
        "هل لديك استفسار أو مشروع تريد التعاون فيه؟ أرسل لنا رسالتك";

    public string FormCardTitle { get; set; } = "أرسل رسالة";
    public string LabelName { get; set; } = "الاسم الكامل *";
    public string LabelPhone { get; set; } = "رقم الجوال";
    public string LabelEmail { get; set; } = "البريد الإلكتروني *";
    public string LabelSubject { get; set; } = "الموضوع *";
    public string LabelMessage { get; set; } = "الرسالة *";
    public string PlaceholderName { get; set; } = "محمد العمري";
    public string PlaceholderPhone { get; set; } = "+966 5x xxx xxxx";
    public string PlaceholderEmail { get; set; } = "example@email.com";
    public string PlaceholderMessage { get; set; } = "اكتب رسالتك هنا...";
    public string SubjectOptionPlaceholder { get; set; } = "اختر موضوع الرسالة";
    public List<string> SubjectOptions { get; set; } =
    [
        "استفسار عن الدورات",
        "طلب تعاون",
        "شراء عمل فني",
        "أخرى"
    ];

    public string SubmitButtonText { get; set; } = "إرسال الرسالة 📨";
    public string SubmitSendingText { get; set; } = "جارٍ الإرسال...";
    public string ToastSuccessMessage { get; set; } = "تم إرسال رسالتك بنجاح! سنتواصل معك قريباً 😊";

    public string InfoSectionTitle { get; set; } = "معلومات التواصل";
    public List<ContactInfoRowDto> InfoRows { get; set; } =
    [
        new ContactInfoRowDto { Icon = "📧", Label = "البريد الإلكتروني", Value = "info@artplatform.com" },
        new ContactInfoRowDto { Icon = "📱", Label = "رقم الجوال", Value = "+966 50 000 0000" },
        new ContactInfoRowDto { Icon = "🕐", Label = "ساعات العمل", Value = "السبت - الخميس، 9 ص - 6 م" },
        new ContactInfoRowDto { Icon = "📍", Label = "الموقع", Value = "الرياض، المملكة العربية السعودية" }
    ];

    public string SocialSectionTitle { get; set; } = "تابعني على منصات التواصل";
    public List<ContactSocialLinkDto> SocialLinks { get; set; } =
    [
        new ContactSocialLinkDto { Name = "Instagram", Icon = "📷", Url = "https://instagram.com" },
        new ContactSocialLinkDto { Name = "Twitter", Icon = "𝕏", Url = "https://twitter.com" },
        new ContactSocialLinkDto { Name = "YouTube", Icon = "▶", Url = "https://youtube.com" }
    ];
}

public class ContactInfoRowDto
{
    public string Icon { get; set; } = "📧";
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
}

public class ContactSocialLinkDto
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Url { get; set; } = "";
}

/// <summary>محتوى الفوتر (مخزن كـ JSON في SiteSettings.footer_config)</summary>
public class FooterConfigDto
{
    public string BrandLetter { get; set; } = "ف";
    public string BrandTitle { get; set; } = "منصة الفن";
    public string BrandDescription { get; set; } =
        "منصة عربية متخصصة للفنون البصرية والتعليم الإبداعي. استكشف أعمال الفنانين وتعلم مهاراتك من أفضل الأساتذة.";

    public List<FooterSocialDto> SocialIcons { get; set; } =
    [
        new FooterSocialDto { Icon = "📷", Url = "https://instagram.com", Name = "Instagram" },
        new FooterSocialDto { Icon = "𝕏", Url = "https://twitter.com", Name = "Twitter" },
        new FooterSocialDto { Icon = "▶", Url = "https://youtube.com", Name = "YouTube" },
        new FooterSocialDto { Icon = "Be", Url = "https://behance.net", Name = "Behance" }
    ];

    public List<FooterColumnDto> Columns { get; set; } =
    [
        new FooterColumnDto
        {
            Title = "الموقع",
            Links =
            [
                new FooterLinkDto { Label = "المعرض الفني", Href = "/portfolio" },
                new FooterLinkDto { Label = "الدورات التعليمية", Href = "/courses" },
                new FooterLinkDto { Label = "المدونة", Href = "/blog" },
                new FooterLinkDto { Label = "عن الفنان", Href = "/about" }
            ]
        },
        new FooterColumnDto
        {
            Title = "الدعم",
            Links =
            [
                new FooterLinkDto { Label = "تواصل معنا", Href = "/contact" },
                new FooterLinkDto { Label = "الأسئلة الشائعة", Href = "/faq" },
                new FooterLinkDto { Label = "تسجيل الدخول", Href = "/login" },
                new FooterLinkDto { Label = "إنشاء حساب", Href = "/register" }
            ]
        }
    ];

    /// <summary>يُعرض بعد © والسنة — مثال: منصة الفن. جميع الحقوق محفوظة.</summary>
    public string CopyrightLine { get; set; } = "منصة الفن. جميع الحقوق محفوظة.";

    public List<FooterLegalLinkDto> LegalLinks { get; set; } =
    [
        new FooterLegalLinkDto { Label = "سياسة الخصوصية", Href = "/privacy" },
        new FooterLegalLinkDto { Label = "شروط الاستخدام", Href = "/terms" }
    ];
}

public class FooterSocialDto
{
    public string Icon { get; set; } = "";
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
}

public class FooterColumnDto
{
    public string Title { get; set; } = "";
    public List<FooterLinkDto> Links { get; set; } = new();
}

public class FooterLinkDto
{
    public string Label { get; set; } = "";
    public string Href { get; set; } = "";
}

public class FooterLegalLinkDto
{
    public string Label { get; set; } = "";
    public string Href { get; set; } = "";
}

public class PublicSiteSettingsDto
{
    /// <summary>رابط صورة شعار الموقع (مسار نسبي أو URL كامل)</summary>
    public string SiteLogoUrl { get; set; } = "";
    public string SiteName { get; set; } = "";
    public string SiteDescription { get; set; } = "";
    public string ContactEmail { get; set; } = "";
    public string HeroTitle { get; set; } = "";
    public string HeroSubtitle { get; set; } = "";
    public HomePageConfigDto HomePage { get; set; } = new();
    public ContactPageConfigDto ContactPage { get; set; } = new();
    public FooterConfigDto Footer { get; set; } = new();
}

public class UpdateSettingRequest
{
    public string Value { get; set; } = string.Empty;
}
