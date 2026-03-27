<template>
  <div v-if="courseStore.loading" class="pt-32 flex justify-center">
    <div class="w-12 h-12 border-4 border-gold/30 border-t-gold rounded-full animate-spin"></div>
  </div>

  <div v-else-if="course" class="pt-24 pb-20">
    <!-- Hero -->
    <div class="relative bg-surface border-b border-line">
      <div class="max-w-7xl mx-auto px-4 py-16 grid md:grid-cols-2 gap-12 items-center">
        <div>
          <div class="flex items-center gap-3 mb-6">
            <span class="badge-gold">{{ course.categoryName }}</span>
            <span class="badge text-xs" :class="levelClass">{{ levelLabel }}</span>
          </div>
          <h1 class="text-4xl font-bold text-fg mb-4 leading-tight">{{ course.title }}</h1>
          <p class="text-fg-mute text-lg leading-relaxed mb-8">{{ course.shortDescription }}</p>

          <div class="flex flex-wrap gap-6 text-sm text-fg-mute mb-8">
            <span>📚 {{ course.lessonCount }} درس</span>
            <span>⏱ {{ Math.floor(course.durationMinutes / 60) }} ساعة</span>
            <span>👥 {{ course.enrollmentCount }} طالب</span>
            <span>💰 {{ course.price === 0 ? 'مجاني' : `${course.price} د.ع` }}</span>
          </div>

          <div class="flex gap-4">
            <button v-if="!course.isEnrolled" @click="handleEnroll" :disabled="enrolling"
              class="btn-primary text-lg px-8 py-4">
              {{ enrolling ? 'جارٍ التسجيل...' : (course.price === 0 ? 'سجل مجاناً' : 'سجل الآن') }}
            </button>
            <RouterLink v-else :to="`/courses/${course.slug}/learn`" class="btn-primary text-lg px-8 py-4">
              {{ course.progressPercent > 0 ? '▶ تابع التعلم' : '▶ ابدأ الدورة' }}
            </RouterLink>
          </div>

          <!-- Progress bar for enrolled users -->
          <div v-if="course.isEnrolled && course.progressPercent !== null" class="mt-6">
            <div class="flex justify-between text-sm text-fg-mute mb-2">
              <span>تقدمك في الدورة</span>
              <span class="text-gold font-semibold">{{ course.progressPercent }}%</span>
            </div>
            <div class="h-2 bg-line rounded-full overflow-hidden">
              <div class="h-full bg-gold rounded-full transition-all duration-700"
                   :style="{ width: `${course.progressPercent}%` }"></div>
            </div>
          </div>
        </div>

        <!-- Thumbnail -->
        <div class="relative">
          <div class="rounded-2xl overflow-hidden aspect-video">
            <img :src="thumbSrc" :alt="course.title" class="w-full h-full object-cover">
          </div>
          <div v-if="course.previewVideoUrl"
            class="absolute inset-0 flex items-center justify-center cursor-pointer"
            @click="showPreview = true">
            <div class="w-20 h-20 rounded-full bg-gold/90 flex items-center justify-center shadow-2xl shadow-gold/40 hover:scale-110 transition-transform">
              <svg class="w-8 h-8 text-dark ml-1" fill="currentColor" viewBox="0 0 24 24">
                <path d="M8 5v14l11-7z"/>
              </svg>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="max-w-7xl mx-auto px-4 py-16 grid lg:grid-cols-3 gap-10">
      <!-- Lessons list -->
      <div class="lg:col-span-2">
        <h2 class="text-2xl font-bold text-fg mb-8">محتوى الدورة</h2>
        <div class="space-y-3">
          <div v-for="(lesson, i) in course.lessons" :key="lesson.id"
            class="card p-5 flex items-center gap-4 cursor-pointer hover:border-gold/40 transition-colors"
            :class="lesson.isCompleted ? 'border-green-500/30' : ''">

            <div class="w-10 h-10 rounded-full flex items-center justify-center flex-shrink-0"
              :class="lesson.isCompleted ? 'bg-green-500/20 text-green-400' : 'bg-input text-fg-mute'">
              {{ lesson.isCompleted ? '✓' : i + 1 }}
            </div>

            <div class="flex-1 min-w-0">
              <div class="flex items-center gap-2 mb-1">
                <h4 class="text-fg font-medium text-sm line-clamp-1">{{ lesson.title }}</h4>
                <span v-if="lesson.isPreview" class="badge bg-gold/20 text-gold border-gold/30 text-xs">معاينة</span>
              </div>
              <span class="text-fg-dim text-xs">⏱ {{ lesson.durationMinutes }} دقيقة</span>
            </div>

            <svg class="w-5 h-5 text-fg-dim flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.828 14.828a4 4 0 01-5.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
        </div>
      </div>

      <!-- Course info card -->
      <div class="space-y-6">
        <div class="card p-6">
          <h3 class="text-fg font-bold mb-4">تفاصيل الدورة</h3>
          <div class="space-y-4 text-sm">
            <div class="flex justify-between">
              <span class="text-fg-mute">المستوى</span>
              <span class="text-fg">{{ levelLabel }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-fg-mute">عدد الدروس</span>
              <span class="text-fg">{{ course.lessonCount }} درس</span>
            </div>
            <div class="flex justify-between">
              <span class="text-fg-mute">المدة الكلية</span>
              <span class="text-fg">{{ Math.floor(course.durationMinutes / 60) }}س {{ course.durationMinutes % 60 }}د</span>
            </div>
            <div class="flex justify-between">
              <span class="text-fg-mute">الطلاب المسجلين</span>
              <span class="text-fg">{{ course.enrollmentCount }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-fg-mute">السعر</span>
              <span class="text-gold font-bold">{{ course.price === 0 ? 'مجاني' : `${course.price} د.ع` }}</span>
            </div>
          </div>
        </div>
      </div>
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

const levelMap = { Beginner: 'مبتدئ', Intermediate: 'متوسط', Advanced: 'متقدم' }
const levelClass = computed(() => ({
  Beginner: 'bg-green-500/20 text-green-400 border-green-500/30',
  Intermediate: 'bg-blue-500/20 text-blue-400 border-blue-500/30',
  Advanced: 'bg-red-500/20 text-red-400 border-red-500/30',
}[course.value?.level] || ''))
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
