<template>
  <div class="min-h-screen bg-dark flex flex-col" dir="rtl" v-if="course">

    <!-- ─── Header ──────────────────────────────────────────── -->
    <header class="bg-dark-100 border-b border-dark-300 sticky top-0 z-30 h-14 flex items-center px-4 gap-4">

      <!-- Logo + back -->
      <div class="flex items-center gap-3 flex-shrink-0">
        <RouterLink to="/" class="w-8 h-8 rounded-lg bg-gradient-to-br from-gold to-gold-dark flex items-center justify-center shadow-md">
          <span class="text-dark font-bold text-sm">ف</span>
        </RouterLink>
        <RouterLink :to="`/courses/${course.slug}`"
          class="hidden sm:flex items-center gap-1.5 text-gray-400 hover:text-gold transition-colors text-sm">
          <svg class="w-4 h-4 rotate-180" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/>
          </svg>
          العودة للدورة
        </RouterLink>
      </div>

      <!-- Divider -->
      <div class="hidden sm:block h-5 w-px bg-dark-300"></div>

      <!-- Course title + lesson -->
      <div class="flex-1 min-w-0">
        <p class="text-white font-semibold text-sm truncate leading-tight">{{ course.title }}</p>
        <p v-if="activeLesson" class="text-gray-500 text-xs truncate mt-0.5">{{ activeLesson.title }}</p>
      </div>

      <!-- Progress -->
      <div class="hidden md:flex items-center gap-3 flex-shrink-0">
        <div class="flex items-center gap-2">
          <div class="w-28 h-1.5 bg-dark-300 rounded-full overflow-hidden">
            <div class="h-full bg-gold rounded-full transition-all duration-500"
                 :style="{ width: `${progressPercent}%` }"></div>
          </div>
          <span class="text-xs text-gray-400 tabular-nums">{{ progressPercent }}%</span>
        </div>
        <span class="text-xs text-gray-500 bg-dark-200 px-2 py-0.5 rounded-full">
          {{ completedCount }}/{{ course.lessons?.length }} درس
        </span>
      </div>

      <!-- Admin link if admin -->
      <RouterLink v-if="auth.isAdmin" to="/admin"
        class="hidden sm:flex items-center gap-1.5 text-xs text-gray-400 hover:text-gold transition-colors border border-dark-300 rounded-lg px-2.5 py-1.5 flex-shrink-0">
        <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/>
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
        </svg>
        لوحة التحكم
      </RouterLink>

      <!-- Exit -->
      <button @click="auth.logout()"
        class="flex items-center gap-1.5 text-xs text-gray-400 hover:text-red-400 transition-colors border border-dark-300 hover:border-red-400/40 rounded-lg px-2.5 py-1.5 flex-shrink-0">
        <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
        </svg>
        خروج
      </button>
    </header>

    <!-- ─── Body ─────────────────────────────────────────────── -->
    <div class="flex flex-1 overflow-hidden">

      <!-- Video + info -->
      <div class="flex-1 flex flex-col overflow-auto">

        <!-- Video -->
        <div class="bg-black w-full" style="aspect-ratio:16/9; max-height:calc(100vh - 56px - 200px)">
          <video v-if="activeLesson?.videoUrl"
            :key="activeLesson.id"
            :src="activeLesson.videoUrl"
            controls class="w-full h-full"
            @timeupdate="handleTimeUpdate"
            @ended="handleLessonComplete"
            ref="videoRef">
          </video>
          <div v-else class="w-full h-full flex items-center justify-center">
            <div class="text-center text-gray-600">
              <div class="text-5xl mb-3">🎬</div>
              <p class="text-sm">اختر درساً للمشاهدة</p>
            </div>
          </div>
        </div>

        <!-- Lesson info -->
        <div class="p-6 border-b border-dark-300">
          <div class="flex items-start justify-between gap-4 max-w-4xl">
            <div class="flex-1 min-w-0">
              <h2 class="text-xl font-bold text-white mb-1">{{ activeLesson?.title || 'اختر درساً' }}</h2>
              <p v-if="activeLesson?.description" class="text-gray-400 text-sm leading-relaxed">{{ activeLesson.description }}</p>
            </div>
            <div class="flex-shrink-0">
              <button v-if="activeLesson && !activeLesson.isCompleted"
                @click="markComplete"
                class="flex items-center gap-2 text-sm border border-gold/50 text-gold hover:bg-gold hover:text-dark px-4 py-2 rounded-lg transition-all">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"/>
                </svg>
                وضع علامة مكتمل
              </button>
              <div v-else-if="activeLesson?.isCompleted"
                class="flex items-center gap-2 text-sm text-green-400 bg-green-500/10 border border-green-500/30 px-4 py-2 rounded-lg">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"/>
                </svg>
                مكتمل
              </div>
            </div>
          </div>
        </div>

        <!-- Mobile progress (shown below lesson info on small screens) -->
        <div class="md:hidden px-6 py-3 flex items-center gap-3 border-b border-dark-300">
          <div class="flex-1 h-1.5 bg-dark-300 rounded-full overflow-hidden">
            <div class="h-full bg-gold rounded-full transition-all duration-500" :style="{ width: `${progressPercent}%` }"></div>
          </div>
          <span class="text-xs text-gray-400">{{ completedCount }}/{{ course.lessons?.length }}</span>
        </div>
      </div>

      <!-- ─── Lessons Sidebar ─────────────────────────────── -->
      <aside class="w-72 bg-dark-100 border-r border-dark-300 flex flex-col flex-shrink-0 overflow-hidden">
        <div class="p-4 border-b border-dark-300 bg-dark-100 sticky top-0">
          <h3 class="text-white font-bold text-sm mb-3">محتوى الدورة</h3>
          <div class="flex justify-between text-xs text-gray-500 mb-1.5">
            <span>{{ completedCount }} من {{ course.lessons?.length }} درس</span>
            <span class="text-gold font-medium">{{ progressPercent }}%</span>
          </div>
          <div class="h-1.5 bg-dark-300 rounded-full overflow-hidden">
            <div class="h-full bg-gradient-to-l from-gold to-gold-light rounded-full transition-all duration-500"
                 :style="{ width: `${progressPercent}%` }"></div>
          </div>
        </div>

        <div class="overflow-y-auto flex-1">
          <div v-for="(lesson, i) in course.lessons" :key="lesson.id"
            @click="selectLesson(lesson)"
            class="flex items-center gap-3 px-4 py-3.5 cursor-pointer border-b border-dark-300/50 transition-colors"
            :class="activeLesson?.id === lesson.id
              ? 'bg-gold/8 border-r-2 border-r-gold'
              : 'hover:bg-dark-200'">

            <!-- Status circle -->
            <div class="w-7 h-7 rounded-full flex items-center justify-center text-xs flex-shrink-0 font-medium"
              :class="lesson.isCompleted
                ? 'bg-green-500/20 text-green-400'
                : activeLesson?.id === lesson.id
                  ? 'bg-gold text-dark font-bold'
                  : 'bg-dark-300 text-gray-400'">
              <svg v-if="lesson.isCompleted" class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7"/>
              </svg>
              <span v-else>{{ i + 1 }}</span>
            </div>

            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium truncate"
                :class="lesson.isCompleted ? 'text-gray-500 line-through' : activeLesson?.id === lesson.id ? 'text-white' : 'text-gray-300'">
                {{ lesson.title }}
              </p>
              <p class="text-xs text-gray-600 mt-0.5">{{ lesson.durationMinutes }} دقيقة</p>
            </div>

            <!-- Playing indicator -->
            <div v-if="activeLesson?.id === lesson.id && !lesson.isCompleted"
              class="flex gap-0.5 items-end flex-shrink-0 h-4">
              <span class="w-0.5 bg-gold rounded animate-bounce" style="height:40%;animation-delay:0s"></span>
              <span class="w-0.5 bg-gold rounded animate-bounce" style="height:80%;animation-delay:.15s"></span>
              <span class="w-0.5 bg-gold rounded animate-bounce" style="height:60%;animation-delay:.3s"></span>
            </div>
          </div>
        </div>
      </aside>
    </div>
  </div>

  <!-- Loading -->
  <div v-else class="min-h-screen bg-dark flex items-center justify-center" dir="rtl">
    <div class="text-center">
      <div class="w-10 h-10 border-2 border-gold border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
      <p class="text-gray-400">جارٍ تحميل الدورة...</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useCourseStore } from '@/stores/courses'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const courseStore = useCourseStore()
const auth = useAuthStore()
const course = computed(() => courseStore.currentCourse)
const activeLesson = ref(null)
const videoRef = ref(null)
let progressTimer = null

onMounted(async () => {
  await courseStore.fetchBySlug(route.params.slug)
  if (course.value?.lessons?.length) {
    const lastIncomplete = course.value.lessons.find(l => !l.isCompleted) || course.value.lessons[0]
    selectLesson(lastIncomplete)
  }
})

const completedCount = computed(() => course.value?.lessons?.filter(l => l.isCompleted).length || 0)
const progressPercent = computed(() => {
  if (!course.value?.lessons?.length) return 0
  return Math.round((completedCount.value / course.value.lessons.length) * 100)
})

function selectLesson(lesson) {
  activeLesson.value = lesson
}

function handleTimeUpdate() {
  clearTimeout(progressTimer)
  progressTimer = setTimeout(async () => {
    if (!activeLesson.value || !videoRef.value) return
    await courseStore.updateProgress({
      lessonId: activeLesson.value.id,
      watchedSeconds: Math.floor(videoRef.value.currentTime),
      isCompleted: false
    })
  }, 10000)
}

async function handleLessonComplete() {
  await markComplete()
  const idx = course.value.lessons.findIndex(l => l.id === activeLesson.value?.id)
  if (idx < course.value.lessons.length - 1)
    selectLesson(course.value.lessons[idx + 1])
}

async function markComplete() {
  if (!activeLesson.value) return
  await courseStore.updateProgress({
    lessonId: activeLesson.value.id,
    watchedSeconds: videoRef.value ? Math.floor(videoRef.value.currentTime) : 0,
    isCompleted: true
  })
}
</script>
