<template>
  <div v-if="courseStore.loading" class="pt-32 flex justify-center min-h-[50vh]">
    <div class="w-12 h-12 border-4 border-gold/30 border-t-gold rounded-full animate-spin"></div>
  </div>

  <div v-else-if="course" class="pb-20">
    <!-- Hero -->
    <header class="relative overflow-hidden border-b border-line bg-surface">
      <div class="absolute inset-0 home-bg-pattern opacity-[0.35] pointer-events-none" aria-hidden="true" />
      <div class="relative max-w-7xl mx-auto px-4 pt-20 pb-14 md:pt-24 md:pb-20">
        <RouterLink
          to="/courses"
          class="inline-flex items-center gap-2 text-sm text-fg-mute hover:text-gold transition-colors mb-8"
        >
          <span class="text-lg leading-none">›</span>
          العودة إلى الدورات
        </RouterLink>

        <div class="grid lg:grid-cols-12 gap-10 lg:gap-14 items-start">
          <div class="lg:col-span-7">
            <div class="flex flex-wrap items-center gap-3 mb-5">
              <span class="badge-gold">{{ course.categoryName }}</span>
              <span class="badge text-xs border border-line" :class="levelClass">{{ levelLabel }}</span>
            </div>

            <h1 class="text-3xl sm:text-4xl md:text-5xl font-bold text-fg mb-6 leading-[1.2] text-balance">
              {{ course.title }}
            </h1>

            <!-- وصف مختصر — بارز في الهيرو -->
            <div
              v-if="shortExcerpt"
              class="relative rounded-2xl border border-gold/25 bg-gold/5 dark:bg-gold/[0.07] px-5 py-4 md:px-6 md:py-5 mb-8"
            >
              <p class="text-xs font-semibold uppercase tracking-wide text-gold mb-2">نبذة عن الدورة</p>
              <p class="text-fg text-base md:text-lg leading-relaxed font-medium">
                {{ shortExcerpt }}
              </p>
            </div>

            <div class="flex flex-wrap gap-x-8 gap-y-3 text-sm text-fg-mute mb-8">
              <span class="inline-flex items-center gap-2">📚 {{ course.lessonCount }} درس</span>
              <span class="inline-flex items-center gap-2">⏱ {{ durationLabel }}</span>
              <span class="inline-flex items-center gap-2">👥 {{ course.enrollmentCount }} طالب</span>
              <span class="inline-flex items-center gap-2 font-semibold text-gold">
                💰 {{ course.price === 0 ? 'مجاني' : `${course.price} د.ع` }}
              </span>
            </div>

            <div class="flex flex-wrap gap-4">
              <button
                v-if="!course.isEnrolled"
                type="button"
                class="btn-primary text-lg px-8 py-4"
                :disabled="enrolling"
                @click="handleEnroll"
              >
                {{ enrolling ? 'جارٍ التسجيل...' : (course.price === 0 ? 'سجل مجاناً' : 'سجل الآن') }}
              </button>
              <RouterLink v-else :to="`/courses/${course.slug}/learn`" class="btn-primary text-lg px-8 py-4">
                {{ course.progressPercent > 0 ? '▶ تابع التعلم' : '▶ ابدأ الدورة' }}
              </RouterLink>
            </div>

            <div v-if="course.isEnrolled && course.progressPercent !== null" class="mt-8 max-w-md">
              <div class="flex justify-between text-sm text-fg-mute mb-2">
                <span>تقدمك في الدورة</span>
                <span class="text-gold font-semibold">{{ course.progressPercent }}%</span>
              </div>
              <div class="h-2 bg-line rounded-full overflow-hidden">
                <div
                  class="h-full bg-gold rounded-full transition-all duration-700"
                  :style="{ width: `${course.progressPercent}%` }"
                />
              </div>
            </div>
          </div>

          <!-- Thumbnail -->
          <div class="lg:col-span-5 lg:sticky lg:top-28">
            <div class="relative rounded-2xl overflow-hidden aspect-video shadow-xl shadow-black/10 dark:shadow-black/40 ring-1 ring-line">
              <img :src="thumbSrc" :alt="course.title" class="w-full h-full object-cover">
              <div
                v-if="course.previewVideoUrl"
                class="absolute inset-0 flex items-center justify-center cursor-pointer bg-black/20 hover:bg-black/30 transition-colors"
                role="button"
                tabindex="0"
                @click="showPreview = true"
                @keydown.enter="showPreview = true"
              >
                <div
                  class="w-16 h-16 md:w-20 md:h-20 rounded-full bg-gold/95 flex items-center justify-center shadow-2xl shadow-gold/40 hover:scale-110 transition-transform"
                >
                  <svg class="w-7 h-7 md:w-8 md:h-8 text-dark mr-[-2px]" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M8 5v14l11-7z" />
                  </svg>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- المحتوى -->
    <div class="max-w-7xl mx-auto px-4 pt-12 md:pt-16 grid lg:grid-cols-3 gap-10 lg:gap-12">
      <div class="lg:col-span-2 space-y-12">
        <!-- وصف تفصيلي -->
        <section v-if="hasLongDescription" class="scroll-mt-8">
          <h2 class="text-xl md:text-2xl font-bold text-fg mb-4 flex items-center gap-3">
            <span class="w-1 h-8 rounded-full bg-gold shrink-0" aria-hidden="true" />
            نظرة عامة وتفاصيل الدورة
          </h2>
          <div
            class="rounded-2xl border border-line bg-surface p-6 md:p-8 shadow-sm dark:shadow-none"
          >
            <p class="text-fg-soft leading-[1.85] text-base md:text-[1.05rem] whitespace-pre-line text-pretty">
              {{ longDescriptionText }}
            </p>
          </div>
        </section>

        <!-- الدروس -->
        <section>
          <h2 class="text-xl md:text-2xl font-bold text-fg mb-6">محتوى الدورة</h2>
          <div class="space-y-3">
            <div
              v-for="(lesson, i) in course.lessons"
              :key="lesson.id"
              class="card p-4 md:p-5 flex items-center gap-4 cursor-pointer hover:border-gold/40 transition-colors"
              :class="lesson.isCompleted ? 'border-green-500/30' : ''"
            >
              <div
                class="w-10 h-10 rounded-full flex items-center justify-center flex-shrink-0 text-sm font-semibold"
                :class="lesson.isCompleted ? 'bg-green-500/20 text-green-600 dark:text-green-400' : 'bg-input text-fg-mute'"
              >
                {{ lesson.isCompleted ? '✓' : i + 1 }}
              </div>

              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2 mb-1 flex-wrap">
                  <h3 class="text-fg font-medium text-sm md:text-base line-clamp-2">{{ lesson.title }}</h3>
                  <span
                    v-if="lesson.isPreview"
                    class="badge bg-gold/20 text-gold border border-gold/30 text-xs shrink-0"
                  >معاينة</span>
                </div>
                <span class="text-fg-dim text-xs">⏱ {{ lesson.durationMinutes }} دقيقة</span>
              </div>

              <svg class="w-5 h-5 text-fg-dim flex-shrink-0 opacity-60" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
              </svg>
            </div>
          </div>
        </section>
      </div>

      <!-- الشريط الجانبي -->
      <aside class="space-y-6 lg:pt-2">
        <div class="card p-6 lg:sticky lg:top-28">
          <h3 class="text-fg font-bold mb-5 text-lg">تفاصيل الدورة</h3>
          <dl class="space-y-4 text-sm">
            <div class="flex justify-between gap-4 py-2 border-b border-line/80 last:border-0">
              <dt class="text-fg-mute shrink-0">المستوى</dt>
              <dd class="text-fg font-medium text-end">{{ levelLabel }}</dd>
            </div>
            <div class="flex justify-between gap-4 py-2 border-b border-line/80 last:border-0">
              <dt class="text-fg-mute shrink-0">عدد الدروس</dt>
              <dd class="text-fg font-medium">{{ course.lessonCount }} درس</dd>
            </div>
            <div class="flex justify-between gap-4 py-2 border-b border-line/80 last:border-0">
              <dt class="text-fg-mute shrink-0">المدة الكلية</dt>
              <dd class="text-fg font-medium">{{ durationLabel }}</dd>
            </div>
            <div class="flex justify-between gap-4 py-2 border-b border-line/80 last:border-0">
              <dt class="text-fg-mute shrink-0">الطلاب المسجلين</dt>
              <dd class="text-fg font-medium">{{ course.enrollmentCount }}</dd>
            </div>
            <div class="flex justify-between gap-4 pt-1">
              <dt class="text-fg-mute shrink-0">السعر</dt>
              <dd class="text-gold font-bold text-lg">{{ course.price === 0 ? 'مجاني' : `${course.price} د.ع` }}</dd>
            </div>
          </dl>
        </div>
      </aside>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useCourseStore } from '@/stores/courses'
import { useAuthStore } from '@/stores/auth'
import { resolveMediaUrl } from '@/utils/mediaUrl'

const route = useRoute()
const router = useRouter()
const courseStore = useCourseStore()
const auth = useAuthStore()
const enrolling = ref(false)
const showPreview = ref(false)

const course = computed(() => courseStore.currentCourse)

const thumbSrc = computed(() => resolveMediaUrl(course.value?.thumbnailUrl))

const shortExcerpt = computed(() => (course.value?.shortDescription || '').trim())

const longDescriptionText = computed(() => (course.value?.description || '').trim())

/** يظهر القسم التفصيلي إذا كان الوصف الطويل موجوداً ومختلفاً عن المختصر */
const hasLongDescription = computed(() => {
  const long = longDescriptionText.value
  const short = shortExcerpt.value
  if (!long) return false
  if (!short) return true
  return long !== short
})

const durationLabel = computed(() => {
  const m = course.value?.durationMinutes ?? 0
  const h = Math.floor(m / 60)
  const min = m % 60
  if (h <= 0) return `${min} دقيقة`
  if (min === 0) return `${h} ساعة`
  return `${h}س ${min}د`
})

const levelMap = { Beginner: 'مبتدئ', Intermediate: 'متوسط', Advanced: 'متقدم' }
const levelClass = computed(
  () =>
    ({
      Beginner: 'bg-green-500/15 text-green-700 dark:text-green-400 border-green-500/30',
      Intermediate: 'bg-blue-500/15 text-blue-700 dark:text-blue-400 border-blue-500/30',
      Advanced: 'bg-red-500/15 text-red-700 dark:text-red-400 border-red-500/30',
    }[course.value?.level] || '')
)
const levelLabel = computed(() => levelMap[course.value?.level] || '')

onMounted(() => courseStore.fetchBySlug(route.params.slug))

async function handleEnroll() {
  if (!auth.isAuthenticated) return router.push('/login')
  enrolling.value = true
  try {
    await courseStore.enroll(course.value.id)
  } finally {
    enrolling.value = false
  }
}
</script>
