<template>
  <div>
    <!-- Hero Section -->
    <section class="relative min-h-screen flex items-center justify-center overflow-hidden">
      <!-- Background -->
      <div class="absolute inset-0 grid-overlay opacity-40"></div>
      <div class="absolute inset-0 bg-gradient-to-b from-dark/0 via-dark/30 to-dark"></div>

      <!-- Floating art elements -->
      <div class="absolute top-20 left-10 w-64 h-64 rounded-2xl bg-gold/5 border border-gold/10 animate-float hidden lg:block"
           style="animation-delay: 0s">
        <div class="absolute inset-4 rounded-xl bg-gradient-to-br from-gold/10 to-transparent"></div>
      </div>
      <div class="absolute bottom-40 right-10 w-48 h-48 rounded-2xl bg-gold/5 border border-gold/10 animate-float hidden lg:block"
           style="animation-delay: 1.5s"></div>

      <!-- Content -->
      <div class="relative z-10 text-center px-4 max-w-5xl mx-auto pt-24">
        <div class="badge-gold mb-6 inline-flex">{{ s('hero_badge', '✨ منصة مرتضى ثامر العربية الأولى') }}</div>

        <h1 class="text-5xl md:text-7xl font-bold text-white mb-6 leading-tight">
          استكشف عالم
          <span class="text-gradient block mt-2">الفن الإبداعي</span>
        </h1>

        <p class="text-xl text-gray-400 mb-12 max-w-2xl mx-auto leading-relaxed">
          {{ s('hero_subtitle', 'تعلم، استلهم، وأبدع مع أفضل الفنانين العرب. دورات تعليمية احترافية ومعرض فني استثنائي بين يديك.') }}
        </p>

        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <RouterLink to="/portfolio" class="btn-primary text-lg px-8 py-4">
            🎨 استكشف المعرض
          </RouterLink>
          <RouterLink to="/courses" class="btn-outline text-lg px-8 py-4">
            🎓 ابدأ التعلم
          </RouterLink>
        </div>

        <!-- Stats -->
        <div class="flex flex-wrap justify-center gap-12 mt-20 pb-8">
          <div v-for="stat in stats" :key="stat.label" class="text-center">
            <div class="text-4xl font-bold text-gold">{{ stat.value }}</div>
            <div class="text-gray-400 text-sm mt-1">{{ stat.label }}</div>
          </div>
        </div>
      </div>

      <!-- Scroll indicator -->
      <div class="absolute bottom-8 left-1/2 -translate-x-1/2 text-gold animate-bounce">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
        </svg>
      </div>
    </section>

    <!-- Featured Artworks -->
    <section class="py-24 px-4 max-w-7xl mx-auto">
      <div class="text-center mb-16">
        <div class="badge-gold mb-4">🖼️ المعرض المميز</div>
        <h2 class="section-title">أحدث الأعمال الفنية</h2>
        <p class="section-subtitle">استكشف مجموعة مختارة من أجمل الأعمال</p>
      </div>

      <div v-if="artworkStore.loading" class="grid grid-cols-2 md:grid-cols-3 gap-6">
        <div v-for="i in 6" :key="i" class="aspect-square rounded-2xl bg-dark-200 animate-pulse"></div>
      </div>

      <div v-else class="grid grid-cols-2 md:grid-cols-3 gap-4 md:gap-6">
        <ArtworkCard
          v-for="(artwork, i) in artworkStore.featured"
          :key="artwork.id"
          :artwork="artwork"
          :class="i === 0 ? 'row-span-2 col-span-1 md:col-span-1' : ''"
        />
      </div>

      <div class="text-center mt-12">
        <RouterLink to="/portfolio" class="btn-outline">عرض جميع الأعمال ←</RouterLink>
      </div>
    </section>

    <!-- About Section -->
    <section class="py-24 bg-dark-100">
      <div class="max-w-7xl mx-auto px-4">
        <div class="grid md:grid-cols-2 gap-16 items-center">
          <div class="relative">
            <div class="aspect-square rounded-3xl overflow-hidden bg-dark-200">
              <img :src="s('artist_image_url', 'https://picsum.photos/600/600?grayscale')" alt="الفنان" class="w-full h-full object-cover" loading="lazy">
            </div>
            <div class="absolute -bottom-6 -left-6 card p-6 max-w-xs shadow-2xl">
              <div class="text-3xl font-bold text-gold">{{ s('home_about_experience', '+15') }}</div>
              <div class="text-gray-400">سنة خبرة في الفنون</div>
            </div>
          </div>

          <div>
            <div class="badge-gold mb-6">👨‍🎨 عن الفنان</div>
            <h2 class="text-4xl font-bold text-white mb-6 leading-tight">
              رحلة إبداعية<br>
              <span class="text-gradient">لا حدود لها</span>
            </h2>
            <p class="text-gray-400 leading-relaxed mb-6 text-lg">
              {{ s('home_about_bio_1', 'فنان بصري عربي متخصص في الفنون التشكيلية والرقمية. أعمل على توثيق الجمال العربي من خلال ريشتي، وأسعى لنشر ثقافة الفن وتعليمه للجميع.') }}
            </p>
            <p class="text-gray-400 leading-relaxed mb-8 text-lg">
              {{ s('home_about_bio_2', 'من خلال منصتي، أشاركك رحلتي الإبداعية وأعلمك أسرار الفن خطوة بخطوة.') }}
            </p>
            <RouterLink to="/about" class="btn-primary">اعرف المزيد عني</RouterLink>
          </div>
        </div>
      </div>
    </section>

    <!-- Featured Courses -->
    <section class="py-24 px-4 max-w-7xl mx-auto">
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
    </section>

    <!-- Testimonials -->
    <section class="py-24 bg-dark-100">
      <div class="max-w-7xl mx-auto px-4">
        <div class="text-center mb-16">
          <h2 class="section-title">ماذا يقول طلابنا</h2>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div v-for="t in testimonials" :key="t.id" class="card p-8">
            <div class="flex items-center gap-1 mb-4">
              <span v-for="star in t.rating" :key="star" class="text-gold text-lg">★</span>
            </div>
            <p class="text-gray-300 leading-relaxed mb-6">"{{ t.text }}"</p>
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-gold/20 flex items-center justify-center text-gold font-bold">
                {{ t.name.charAt(0) }}
              </div>
              <div>
                <div class="text-white font-semibold">{{ t.name }}</div>
                <div class="text-gray-500 text-sm">{{ t.title }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA -->
    <section class="py-24 px-4">
      <div class="max-w-4xl mx-auto text-center card-glass p-16">
        <h2 class="text-4xl font-bold text-white mb-6">{{ s('cta_title', 'ابدأ رحلتك الإبداعية اليوم') }}</h2>
        <p class="text-gray-400 text-xl mb-10">{{ s('cta_subtitle', 'انضم لآلاف الطلاب وتعلم الفن من أفضل الأساتذة العرب') }}</p>
        <RouterLink to="/register" class="btn-primary text-lg px-10 py-4">سجل مجاناً الآن</RouterLink>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useArtworkStore } from '@/stores/artworks'
import { useCourseStore } from '@/stores/courses'
import { testimonialApi } from '@/services/api'
import { useSiteSettings } from '@/composables/useSiteSettings'
import ArtworkCard from '@/components/ui/ArtworkCard.vue'
import CourseCard from '@/components/ui/CourseCard.vue'

const artworkStore = useArtworkStore()
const courseStore = useCourseStore()
const testimonials = ref([])
const { fetchSettings, get: s } = useSiteSettings()

onMounted(async () => {
  artworkStore.fetchFeatured()
  courseStore.fetchFeatured()
  fetchSettings()
  try {
    const res = await testimonialApi.getActive()
    testimonials.value = res.data || []
  } catch {
    testimonials.value = [
      { id: 1, name: 'أحمد الكندي', title: 'طالب فنون بصرية', text: 'منصة رائعة جداً! تعلمت في أسابيع ما لم أستطع تعلمه في سنوات. الأسلوب واضح ومحترف.', rating: 5 },
      { id: 2, name: 'سارة المطيري', title: 'مصممة جرافيك', text: 'الدورات ممتازة وتغطي كل جوانب الفن الرقمي. المدرس يشرح بأسلوب عربي مفهوم.', rating: 5 },
      { id: 3, name: 'محمد العمري', title: 'فنان هاوي', text: 'المعرض الفني ملهم جداً! الأعمال احترافية وتحفزني على التعلم أكثر كل يوم.', rating: 5 },
    ]
  }
})

const stats = computed(() => [
  { value: s('stat_artworks', '+200'), label: 'عمل فني' },
  { value: s('stat_courses', '+50'), label: 'دورة تعليمية' },
  { value: s('stat_students', '+5000'), label: 'طالب' },
  { value: s('stat_experience', '15+'), label: 'سنة خبرة' },
])
</script>
