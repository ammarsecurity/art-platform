<template>
  <RouterLink :to="`/courses/${course.slug}`" class="card group flex flex-col">
    <!-- Thumbnail -->
    <div class="relative overflow-hidden aspect-video">
      <img
        :src="thumbSrc"
        :alt="course.title"
        class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
      >
      <div class="absolute inset-0 bg-black/20 group-hover:bg-black/10 transition-colors"></div>

      <!-- Play button -->
      <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300">
        <div class="w-14 h-14 rounded-full bg-gold/90 flex items-center justify-center shadow-lg shadow-gold/30">
          <svg class="w-6 h-6 text-dark mr-0.5" fill="currentColor" viewBox="0 0 24 24">
            <path d="M8 5v14l11-7z"/>
          </svg>
        </div>
      </div>

      <!-- Level badge -->
      <div class="absolute top-3 right-3">
        <span class="badge text-xs font-medium px-2 py-1 rounded-md" :class="levelClass">{{ levelLabel }}</span>
      </div>

      <!-- Price -->
      <div class="absolute top-3 left-3">
        <span class="badge bg-black/65 text-white border-0 text-sm font-bold shadow-sm">
          {{ course.price === 0 ? 'مجاني' : `${course.price} د.ع` }}
        </span>
      </div>
    </div>

    <!-- Content -->
    <div class="p-5 flex flex-col flex-1">
      <div class="flex items-center gap-2 mb-3">
        <span class="text-gold text-xs">{{ course.categoryName }}</span>
        <span class="text-dark-300">•</span>
        <span class="text-fg-dim text-xs">{{ course.lessonCount }} درس</span>
      </div>

      <h3 class="text-fg font-bold mb-2 line-clamp-2 group-hover:text-gold transition-colors leading-snug">
        {{ course.title }}
      </h3>

      <p v-if="course.shortDescription" class="text-fg-mute text-sm line-clamp-2 mb-4 flex-1">
        {{ course.shortDescription }}
      </p>

      <div class="mt-auto pt-4 border-t border-line flex items-center justify-between text-sm">
        <span class="text-fg-dim">⏱ {{ Math.floor(course.durationMinutes / 60) }}س {{ course.durationMinutes % 60 }}د</span>
        <span v-if="course.isEnrolled" class="text-green-400 font-medium">✓ مسجل</span>
        <span v-else class="text-gold font-medium">سجل الآن</span>
      </div>

      <!-- Progress bar -->
      <div v-if="course.isEnrolled && course.progressPercent !== null" class="mt-3">
        <div class="flex justify-between text-xs text-fg-dim mb-1">
          <span>التقدم</span>
          <span>{{ course.progressPercent }}%</span>
        </div>
        <div class="h-1.5 bg-line rounded-full overflow-hidden">
          <div class="h-full bg-gold rounded-full transition-all duration-500"
               :style="{ width: `${course.progressPercent}%` }"></div>
        </div>
      </div>
    </div>
  </RouterLink>
</template>

<script setup>
import { computed } from 'vue'
import { resolveMediaUrl } from '@/utils/mediaUrl'

const props = defineProps({ course: { type: Object, required: true } })

const thumbSrc = computed(() =>
  resolveMediaUrl(props.course.thumbnailUrl) || 'https://picsum.photos/400/225?grayscale'
)

const levelMap = {
  Beginner: { label: 'مبتدئ', class: 'bg-green-500/20 text-green-400 border border-green-500/30' },
  Intermediate: { label: 'متوسط', class: 'bg-blue-500/20 text-blue-400 border border-blue-500/30' },
  Advanced: { label: 'متقدم', class: 'bg-red-500/20 text-red-400 border border-red-500/30' },
}
const levelLabel = computed(() => levelMap[props.course.level]?.label || props.course.level)
const levelClass = computed(() => levelMap[props.course.level]?.class || '')
</script>
