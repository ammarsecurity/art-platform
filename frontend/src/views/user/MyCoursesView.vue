<template>
  <div class="min-h-screen py-24 px-4">
    <div class="max-w-4xl mx-auto">

      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-white mb-2">دوراتي</h1>
        <p class="text-gray-400">الدورات التي سجّلت فيها</p>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 gap-5">
        <div v-for="i in 4" :key="i" class="card p-5 animate-pulse">
          <div class="h-40 bg-dark-300 rounded-xl mb-4"></div>
          <div class="h-4 bg-dark-300 rounded w-3/4 mb-2"></div>
          <div class="h-3 bg-dark-300 rounded w-1/2"></div>
        </div>
      </div>

      <!-- Empty state -->
      <div v-else-if="!courses.length" class="text-center py-20">
        <div class="text-6xl mb-4">🎨</div>
        <h2 class="text-xl font-semibold text-white mb-2">لم تسجّل في أي دورة بعد</h2>
        <p class="text-gray-400 mb-6">اكتشف دوراتنا وابدأ رحلتك الفنية</p>
        <RouterLink to="/courses" class="btn-primary">تصفح الدورات</RouterLink>
      </div>

      <!-- Courses grid -->
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-5">
        <div v-for="course in courses" :key="course.id" class="card overflow-hidden group">
          <!-- Thumbnail -->
          <div class="relative h-44 bg-dark-300 overflow-hidden">
            <img v-if="course.thumbnailUrl" :src="course.thumbnailUrl" :alt="course.title"
              class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
            <div v-else class="w-full h-full flex items-center justify-center text-4xl">🎨</div>
            <!-- Level badge -->
            <span class="absolute top-3 right-3 badge-gold text-xs">{{ levelLabel(course.level) }}</span>
          </div>

          <div class="p-5">
            <h3 class="text-white font-semibold text-lg mb-1 line-clamp-1">{{ course.title }}</h3>
            <p class="text-gray-400 text-sm mb-4 line-clamp-2">{{ course.shortDescription || course.description }}</p>

            <!-- Progress -->
            <div class="mb-4">
              <div class="flex justify-between text-xs text-gray-400 mb-1.5">
                <span>التقدم</span>
                <span class="text-gold font-medium">{{ course.progressPercent ?? 0 }}%</span>
              </div>
              <div class="h-1.5 bg-dark-300 rounded-full overflow-hidden">
                <div class="h-full bg-gradient-to-l from-gold to-gold-dark rounded-full transition-all duration-500"
                  :style="{ width: (course.progressPercent ?? 0) + '%' }"></div>
              </div>
            </div>

            <!-- Stats row -->
            <div class="flex items-center gap-4 text-xs text-gray-500 mb-4">
              <span>{{ course.lessonCount ?? 0 }} درس</span>
              <span v-if="course.progressPercent === 100" class="text-green-400 font-medium">✓ مكتملة</span>
            </div>

            <!-- CTA -->
            <RouterLink :to="`/courses/${course.slug}/learn`" class="btn-primary w-full justify-center text-sm">
              {{ course.progressPercent > 0 ? 'متابعة التعلم' : 'ابدأ الدورة' }}
            </RouterLink>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { authApi } from '@/services/api'

const loading = ref(true)
const courses = ref([])

onMounted(async () => {
  try {
    const res = await authApi.myCourses()
    courses.value = res.data
  } catch {
    courses.value = []
  } finally {
    loading.value = false
  }
})

function levelLabel(level) {
  return { Beginner: 'مبتدئ', Intermediate: 'متوسط', Advanced: 'متقدم' }[level] ?? level
}
</script>
