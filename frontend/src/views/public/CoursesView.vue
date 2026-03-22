<template>
  <div class="pt-24 pb-20 px-4 max-w-7xl mx-auto">
    <div class="text-center mb-16">
      <div class="badge-gold mb-4">🎓 التعليم الإبداعي</div>
      <h1 class="section-title">الدورات التعليمية</h1>
      <p class="section-subtitle">تعلم الفن خطوة بخطوة من الصفر حتى الاحتراف</p>
    </div>

    <!-- Filters -->
    <div class="flex flex-wrap gap-4 mb-10 items-center">
      <div class="flex flex-wrap gap-2">
        <button v-for="level in levels" :key="level.value"
          @click="activeLevel = level.value; fetchCourses()"
          class="px-4 py-2 rounded-full border text-sm font-medium transition-all"
          :class="activeLevel === level.value ? 'bg-gold border-gold text-dark' : 'border-dark-300 text-gray-400 hover:border-gold hover:text-gold'">
          {{ level.label }}
        </button>
      </div>
      <div class="mr-auto relative">
        <input v-model="search" @input="debouncedFetch" type="text" placeholder="ابحث عن دورة..."
          class="input-field pr-10 w-72 text-sm py-2">
        <span class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500">🔍</span>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="courseStore.loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="i in 6" :key="i" class="rounded-2xl bg-dark-200 animate-pulse h-80"></div>
    </div>

    <div v-else-if="!courseStore.courses.length" class="text-center py-24">
      <div class="text-6xl mb-4">🎓</div>
      <p class="text-gray-400 text-xl">لا توجد دورات متاحة حالياً</p>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <CourseCard v-for="course in courseStore.courses" :key="course.id" :course="course" />
    </div>

    <Pagination
      v-if="courseStore.pagination.totalPages > 1"
      :current="courseStore.pagination.page"
      :total="courseStore.pagination.totalPages"
      @change="changePage"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useCourseStore } from '@/stores/courses'
import CourseCard from '@/components/ui/CourseCard.vue'
import Pagination from '@/components/ui/Pagination.vue'

const courseStore = useCourseStore()
const activeLevel = ref(null)
const search = ref('')
let timer = null

const levels = [
  { value: null, label: 'الكل' },
  { value: 'Beginner', label: 'مبتدئ' },
  { value: 'Intermediate', label: 'متوسط' },
  { value: 'Advanced', label: 'متقدم' },
]

onMounted(() => fetchCourses())

function fetchCourses() {
  courseStore.fetchCourses({ search: search.value, level: activeLevel.value, page: 1 })
}

function debouncedFetch() {
  clearTimeout(timer)
  timer = setTimeout(fetchCourses, 400)
}

function changePage(page) {
  courseStore.fetchCourses({ page })
}
</script>
