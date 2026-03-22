<template>
  <div>
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-white">الدورات التعليمية</h1>
        <p class="text-gray-400 mt-1">إدارة الدورات والدروس</p>
      </div>
      <RouterLink to="/admin/courses/new" class="btn-primary">+ دورة جديدة</RouterLink>
    </div>

    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="i in 6" :key="i" class="card h-64 animate-pulse"></div>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="course in courses" :key="course.id" class="card group overflow-hidden">
        <div class="relative aspect-video overflow-hidden">
          <img :src="course.thumbnailUrl || 'https://picsum.photos/400/225?grayscale'" :alt="course.title"
            class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
          <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
          <span class="absolute top-3 left-3 badge text-xs"
            :class="course.status === 'Published' ? 'bg-green-500/80 text-white' : 'bg-yellow-500/80 text-white'">
            {{ course.status === 'Published' ? 'منشور' : 'مسودة' }}
          </span>
        </div>
        <div class="p-5">
          <h3 class="text-white font-bold mb-2 line-clamp-1">{{ course.title }}</h3>
          <div class="flex items-center justify-between text-sm text-gray-400 mb-4">
            <span>{{ course.lessonCount }} درس</span>
            <span>{{ course.enrollmentCount }} طالب</span>
            <span class="text-gold font-semibold">{{ course.price === 0 ? 'مجاني' : `${course.price} ر.س` }}</span>
          </div>
          <div class="flex gap-2">
            <RouterLink :to="`/admin/courses/${course.id}/edit`"
              class="flex-1 py-2 text-center text-sm bg-dark-300 hover:bg-dark-200 text-white rounded-lg transition-colors">تعديل</RouterLink>
            <button @click="handleDelete(course.id)"
              class="px-4 py-2 text-sm bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-lg transition-colors">حذف</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { courseApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const courses = ref([])
const loading = ref(false)

onMounted(fetchCourses)

async function fetchCourses() {
  loading.value = true
  try {
    const res = await courseApi.getAll({ page: 1, pageSize: 50, status: 'all' })
    courses.value = res.data.items
  } finally { loading.value = false }
}

async function handleDelete(id) {
  if (!confirm('حذف هذه الدورة؟')) return
  await courseApi.delete(id)
  toast.success('تم الحذف')
  fetchCourses()
}
</script>
