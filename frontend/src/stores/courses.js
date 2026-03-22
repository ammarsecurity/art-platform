import { defineStore } from 'pinia'
import { ref } from 'vue'
import { courseApi } from '@/services/api'
import { toast } from 'vue3-toastify'

export const useCourseStore = defineStore('courses', () => {
  const courses = ref([])
  const featured = ref([])
  const currentCourse = ref(null)
  const pagination = ref({ page: 1, pageSize: 9, totalCount: 0, totalPages: 0 })
  const loading = ref(false)

  async function fetchCourses(params = {}) {
    loading.value = true
    try {
      const res = await courseApi.getAll({ ...pagination.value, ...params })
      courses.value = res.data.items
      Object.assign(pagination.value, { totalCount: res.data.totalCount, totalPages: res.data.totalPages })
    } finally {
      loading.value = false
    }
  }

  async function fetchFeatured() {
    const res = await courseApi.getFeatured(4)
    featured.value = res.data
  }

  async function fetchBySlug(slug) {
    loading.value = true
    try {
      const res = await courseApi.getBySlug(slug)
      currentCourse.value = res.data
    } finally {
      loading.value = false
    }
  }

  async function enroll(courseId) {
    await courseApi.enroll(courseId)
    toast.success('تم التسجيل في الدورة بنجاح!')
    if (currentCourse.value?.id === courseId)
      currentCourse.value.isEnrolled = true
  }

  async function updateProgress(data) {
    await courseApi.updateProgress(data)
    if (currentCourse.value) {
      const lesson = currentCourse.value.lessons?.find(l => l.id === data.lessonId)
      if (lesson) {
        lesson.isCompleted = data.isCompleted
        lesson.watchedSeconds = data.watchedSeconds
      }
    }
  }

  return {
    courses, featured, currentCourse, pagination, loading,
    fetchCourses, fetchFeatured, fetchBySlug, enroll, updateProgress
  }
})
