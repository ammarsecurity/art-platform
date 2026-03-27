<template>
  <div>
    <!-- سلايدر الصور (عند التفعيل ووجود صور) -->
    <HomeHeroSlider
      v-if="showHeroSlider"
      :slides="sliderSlidesValid"
      :interval-sec="sliderIntervalSec"
      :hero-badge="hp.heroBadge"
      :hero-title-line1="hp.heroTitleLine1"
      :hero-title-line2="hp.heroTitleLine2"
      :hero-paragraph="hp.heroParagraph"
      :stats="hp.stats"
    />

    <!-- البانر الكلاسيكي (بدون سلايدر أو عند إيقاف السلايدر) -->
    <section
      v-else
      class="relative min-h-screen flex items-center justify-center overflow-hidden"
    >
      <div class="absolute inset-0 home-bg-pattern opacity-50"></div>
      <div class="absolute inset-0 bg-gradient-to-b from-page/0 via-page/30 to-page"></div>

      <div class="absolute top-20 left-10 w-64 h-64 rounded-2xl bg-gold/5 border border-gold/10 animate-float hidden lg:block"
           style="animation-delay: 0s">
        <div class="absolute inset-4 rounded-xl bg-gradient-to-br from-gold/10 to-transparent"></div>
      </div>
      <div class="absolute bottom-40 right-10 w-48 h-48 rounded-2xl bg-gold/5 border border-gold/10 animate-float hidden lg:block"
           style="animation-delay: 1.5s"></div>

      <div class="relative z-10 text-center px-4 max-w-5xl mx-auto pt-24">
        <div class="badge-gold mb-6 inline-flex">{{ hp.heroBadge }}</div>

        <h1 class="text-5xl md:text-7xl font-bold text-fg mb-6 leading-tight">
          {{ hp.heroTitleLine1 }}
          <span class="text-gradient block mt-2">{{ hp.heroTitleLine2 }}</span>
        </h1>

        <p class="text-xl text-fg-mute mb-12 max-w-2xl mx-auto leading-relaxed">
          {{ hp.heroParagraph }}
        </p>

        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <RouterLink to="/portfolio" class="btn-primary text-lg px-8 py-4">
            🎨 استكشف المعرض
          </RouterLink>
          <RouterLink to="/courses" class="btn-outline text-lg px-8 py-4">
            🎓 ابدأ التعلم
          </RouterLink>
        </div>

        <div class="flex flex-wrap justify-center gap-12 mt-20 pb-8">
          <div v-for="(stat, idx) in hp.stats" :key="idx" class="text-center">
            <div class="text-4xl font-bold text-gold">{{ stat.value }}</div>
            <div class="text-fg-mute text-sm mt-1">{{ stat.label }}</div>
          </div>
        </div>
      </div>

      <div class="absolute bottom-8 left-1/2 -translate-x-1/2 text-gold animate-bounce">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
        </svg>
      </div>
    </section>

    <!-- أحدث الأعمال (آخر ما أُضيف للمعرض) -->
    <section class="relative py-24 home-bg-pattern">
      <div class="max-w-7xl mx-auto px-4">
      <div class="text-center mb-16">
        <div class="badge-gold mb-4">🖼️ آخر الإضافات</div>
        <h2 class="section-title">أحدث الأعمال الفنية</h2>
        <p class="section-subtitle">آخر الأعمال المنشورة في المعرض — مرتبة حسب تاريخ الإضافة</p>
      </div>

      <div v-if="artworkStore.loading" class="grid grid-cols-2 md:grid-cols-3 gap-6">
        <div v-for="i in 6" :key="i" class="aspect-square rounded-2xl bg-input animate-pulse"></div>
      </div>

      <div v-else-if="!artworkStore.featured.length" class="card p-12 text-center max-w-lg mx-auto">
        <p class="text-fg-mute mb-6">لا توجد أعمال منشورة في المعرض بعد. يمكنك تصفّح باقي الأقسام أو العودة لاحقاً.</p>
        <RouterLink to="/portfolio" class="btn-outline">الذهاب إلى المعرض</RouterLink>
      </div>

      <div v-else class="grid grid-cols-2 md:grid-cols-3 gap-4 md:gap-6">
        <ArtworkCard
          v-for="(artwork, i) in artworkStore.featured"
          :key="artwork.id"
          :artwork="artwork"
          :class="i === 0 ? 'row-span-2 col-span-1 md:col-span-1' : ''"
        />
      </div>

      <div v-if="artworkStore.featured.length" class="text-center mt-12">
        <RouterLink to="/portfolio" class="btn-outline">عرض جميع الأعمال ←</RouterLink>
      </div>
      </div>
    </section>

    <!-- About Section -->
    <section class="relative py-24 bg-surface home-bg-pattern">
      <div class="max-w-7xl mx-auto px-4">
        <div class="grid md:grid-cols-2 gap-16 items-center">
          <div class="relative">
            <div class="aspect-square rounded-3xl overflow-hidden bg-input">
              <img :src="resolveMediaUrl(hp.about.imageUrl)" alt="" class="w-full h-full object-cover">
            </div>
            <div class="absolute -bottom-6 -left-6 card p-6 max-w-xs shadow-2xl">
              <div class="text-3xl font-bold text-gold">{{ hp.about.cardValue }}</div>
              <div class="text-fg-mute">{{ hp.about.cardLabel }}</div>
            </div>
          </div>

          <div>
            <div class="badge-gold mb-6">{{ hp.about.badge }}</div>
            <h2 class="text-4xl font-bold text-fg mb-6 leading-tight">
              {{ hp.about.titleLine1 }}<br>
              <span class="text-gradient">{{ hp.about.titleLine2 }}</span>
            </h2>
            <p class="text-fg-mute leading-relaxed mb-6 text-lg">{{ hp.about.paragraph1 }}</p>
            <p class="text-fg-mute leading-relaxed mb-8 text-lg">{{ hp.about.paragraph2 }}</p>
            <RouterLink to="/about" class="btn-primary">اعرف المزيد عني</RouterLink>
          </div>
        </div>
      </div>
    </section>

    <!-- Featured Courses -->
    <section class="relative py-24 home-bg-pattern">
      <div class="max-w-7xl mx-auto px-4">
      <div class="text-center mb-16">
        <div class="badge-gold mb-4">🎓 التعلم الإبداعي</div>
        <h2 class="section-title">الدورات المميزة</h2>
        <p class="section-subtitle">تعلم من الصفر حتى الاحتراف</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <CourseCard v-for="course in courseStore.featured" :key="course.id" :course="course" />
      </div>

      <div class="text-center mt-12">
        <RouterLink to="/courses" class="btn-outline">عرض جميع الدورات ←</RouterLink>
      </div>
      </div>
    </section>

    <!-- Testimonials -->
    <section class="relative py-24 bg-surface home-bg-pattern">
      <div class="max-w-7xl mx-auto px-4">
        <div class="text-center mb-16">
          <h2 class="section-title">ماذا يقول طلابنا</h2>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div v-for="(t, i) in hp.testimonials" :key="i" class="card p-8">
            <div class="flex items-center gap-1 mb-4">
              <span v-for="s in 5" :key="s" class="text-gold text-lg">★</span>
            </div>
            <p class="text-fg-soft leading-relaxed mb-6">"{{ t.text }}"</p>
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-gold/20 flex items-center justify-center text-gold font-bold">
                {{ (t.name || '?').charAt(0) }}
              </div>
              <div>
                <div class="text-fg font-semibold">{{ t.name }}</div>
                <div class="text-fg-dim text-sm">{{ t.title }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA -->
    <section class="relative py-24 home-bg-pattern px-4">
      <div class="max-w-4xl mx-auto text-center card-glass p-16">
        <h2 class="text-4xl font-bold text-fg mb-6">{{ hp.cta.title }}</h2>
        <p class="text-fg-mute text-xl mb-10">{{ hp.cta.subtitle }}</p>
        <RouterLink to="/register" class="btn-primary text-lg px-10 py-4">سجل مجاناً الآن</RouterLink>
      </div>
    </section>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useArtworkStore } from '@/stores/artworks'
import { useCourseStore } from '@/stores/courses'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import ArtworkCard from '@/components/ui/ArtworkCard.vue'
import CourseCard from '@/components/ui/CourseCard.vue'
import HomeHeroSlider from '@/components/home/HomeHeroSlider.vue'
import { resolveMediaUrl } from '@/utils/mediaUrl'

const artworkStore = useArtworkStore()
const courseStore = useCourseStore()
const site = useSiteSettingsStore()

function fallbackHome() {
  return {
    heroBadge: '✨ منصة الفن العربية الأولى',
    heroTitleLine1: 'استكشف عالم',
    heroTitleLine2: 'الفن الإبداعي',
    heroParagraph: 'تعلم، استلهم، وأبدع مع أفضل الفنانين العرب. دورات تعليمية احترافية ومعرض فني استثنائي بين يديك.',
    stats: [
      { value: '+200', label: 'عمل فني' },
      { value: '+50', label: 'دورة تعليمية' },
      { value: '+5000', label: 'طالب' },
      { value: '15+', label: 'سنة خبرة' }
    ],
    about: {
      imageUrl: 'https://picsum.photos/600/600?grayscale',
      badge: '👨‍🎨 عن الفنان',
      titleLine1: 'رحلة إبداعية',
      titleLine2: 'لا حدود لها',
      paragraph1: 'فنان بصري عربي متخصص في الفنون التشكيلية والرقمية.',
      paragraph2: 'من خلال منصتي، أشاركك رحلتي الإبداعية وأعلمك أسرار الفن خطوة بخطوة.',
      cardValue: '+15',
      cardLabel: 'سنة خبرة في الفنون'
    },
    testimonials: [
      { name: 'أحمد الكندي', title: 'طالب فنون بصرية', text: 'منصة رائعة جداً!' },
      { name: 'سارة المطيري', title: 'مصممة جرافيك', text: 'الدورات ممتازة.' },
      { name: 'محمد العمري', title: 'فنان هاوي', text: 'المعرض الفني ملهم جداً!' }
    ],
    cta: {
      title: 'ابدأ رحلتك الإبداعية اليوم',
      subtitle: 'انضم لآلاف الطلاب وتعلم الفن من أفضل الأساتذة العرب'
    },
    slider: {
      enabled: true,
      intervalSeconds: 8,
      items: [
        {
          imageUrl: 'https://picsum.photos/seed/arthome1/1920/960',
          title: 'عالم الفن بين يديك',
          subtitle: 'تعلّم، استلهم، واطلق إبداعك مع محتوى عربي احترافي',
          linkUrl: '/courses',
          linkLabel: 'استكشف الدورات'
        },
        {
          imageUrl: 'https://picsum.photos/seed/arthome2/1920/960',
          title: 'معرض فني يُلهمك',
          subtitle: 'أعمال مختارة من فنانين عرب',
          linkUrl: '/portfolio',
          linkLabel: 'زيارة المعرض'
        },
        {
          imageUrl: 'https://picsum.photos/seed/arthome3/1920/960',
          title: 'انضم لمجتمعنا',
          subtitle: 'آلاف الطلاب يتعلمون الفن كل يوم',
          linkUrl: '/register',
          linkLabel: 'سجّل مجاناً'
        }
      ]
    }
  }
}

function mapSliderItems(items) {
  return (items || []).map((s) => ({
    imageUrl: String(s.imageUrl ?? ''),
    title: String(s.title ?? ''),
    subtitle: String(s.subtitle ?? ''),
    linkUrl: String(s.linkUrl ?? ''),
    linkLabel: String(s.linkLabel ?? 'اكتشف المزيد')
  }))
}

const hp = computed(() => {
  const h = site.data?.homePage
  if (!h) return fallbackHome()
  const f = fallbackHome()
  const aboutMerged = { ...f.about, ...(h.about || {}) }
  if (!String(aboutMerged.imageUrl || '').trim()) aboutMerged.imageUrl = f.about.imageUrl

  const intervalFromApi =
    h.slider != null && Number.isFinite(Number(h.slider.intervalSeconds))
      ? Number(h.slider.intervalSeconds)
      : null
  const sliderItemsResolved =
    h.slider != null && Array.isArray(h.slider.items)
      ? h.slider.items.length
        ? mapSliderItems(h.slider.items)
        : []
      : f.slider.items
  const sliderMerged = {
    enabled: h.slider == null ? f.slider.enabled : h.slider.enabled !== false,
    intervalSeconds: Math.min(
      120,
      Math.max(3, intervalFromApi != null ? intervalFromApi : f.slider.intervalSeconds)
    ),
    items: sliderItemsResolved
  }

  return {
    ...f,
    ...h,
    about: aboutMerged,
    cta: { ...f.cta, ...(h.cta || {}) },
    stats: Array.isArray(h.stats) && h.stats.length ? h.stats : f.stats,
    testimonials: Array.isArray(h.testimonials) && h.testimonials.length ? h.testimonials : f.testimonials,
    slider: sliderMerged
  }
})

const sliderIntervalSec = computed(() => hp.value.slider.intervalSeconds)

const sliderSlidesValid = computed(() =>
  (hp.value.slider.items || []).filter((s) => String(s.imageUrl || '').trim())
)

const showHeroSlider = computed(
  () => hp.value.slider.enabled !== false && sliderSlidesValid.value.length > 0
)

onMounted(async () => {
  await site.load()
  artworkStore.fetchFeatured()
  courseStore.fetchFeatured()
})
</script>
