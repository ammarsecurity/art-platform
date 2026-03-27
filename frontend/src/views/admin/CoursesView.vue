<template>
  <div>
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-fg">الدورات التعليمية</h1>
        <p class="text-fg-mute mt-1">إدارة الدورات — استخدم <span class="text-fg-soft">الدروس</span> لإضافة فيديوهات الدرس، أو <span class="text-fg-soft">تعديل</span> لبيانات الدورة</p>
      </div>
      <RouterLink to="/admin/courses/new" class="btn-primary">+ دورة جديدة</RouterLink>
    </div>

    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="i in 6" :key="i" class="card h-64 animate-pulse"></div>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="course in courses" :key="course.id" class="card group overflow-hidden">
        <div class="relative aspect-video overflow-hidden">
          <img
            :src="course.thumbnailUrl ? resolveMediaUrl(course.thumbnailUrl) : 'https://picsum.photos/400/225?grayscale'"
            :alt="course.title"
            class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
          >
          <div class="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent"></div>
          <span class="absolute top-3 left-3 badge text-xs"
            :class="course.status === 'Published' ? 'bg-green-500/80 text-white' : course.status === 'Archived' ? 'bg-gray-600/90 text-white' : 'bg-yellow-500/80 text-white'">
            {{ course.status === 'Published' ? 'منشور' : course.status === 'Archived' ? 'مؤرشف' : 'مسودة' }}
          </span>
        </div>
        <div class="p-5">
          <h3 class="text-fg font-bold mb-2 line-clamp-1">{{ course.title }}</h3>
          <div class="flex items-center justify-between text-sm text-fg-mute mb-4">
            <span>{{ course.lessonCount }} درس</span>
            <span>{{ course.enrollmentCount }} طالب</span>
            <span class="text-gold font-semibold">{{ course.price === 0 ? 'مجاني' : `${course.price} د.ع` }}</span>
          </div>
          <div class="flex flex-wrap gap-2 mb-2">
            <button
              v-if="course.status !== 'Published'"
              type="button"
              :disabled="statusLoadingId === course.id"
              class="flex-1 min-w-[6rem] py-2 text-sm bg-green-500/25 hover:bg-green-500/35 text-green-400 border border-green-500/30 rounded-lg transition-colors disabled:opacity-50"
              @click="setCourseStatus(course, 'Published')"
            >
              {{ statusLoadingId === course.id ? '…' : 'نشر' }}
            </button>
            <button
              v-if="course.status !== 'Draft'"
              type="button"
              :disabled="statusLoadingId === course.id"
              class="flex-1 min-w-[6rem] py-2 text-sm bg-yellow-500/20 hover:bg-yellow-500/30 text-yellow-400 border border-yellow-500/30 rounded-lg transition-colors disabled:opacity-50"
              @click="setCourseStatus(course, 'Draft')"
            >
              {{ statusLoadingId === course.id ? '…' : 'مسودة' }}
            </button>
          </div>
          <div class="flex flex-wrap gap-2">
            <RouterLink :to="`/admin/courses/${course.id}/lessons`"
              class="flex-1 min-w-[5.5rem] py-2 text-center text-sm bg-gold/20 hover:bg-gold/30 text-gold border border-gold/35 rounded-lg transition-colors">الدروس</RouterLink>
            <RouterLink :to="`/admin/courses/${course.id}/edit`"
              class="flex-1 min-w-[5.5rem] py-2 text-center text-sm bg-line hover:bg-input text-fg rounded-lg transition-colors">تعديل</RouterLink>
            <button @click="confirmDelete(course)"
              class="px-4 py-2 text-sm bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-lg transition-colors">حذف</button>
          </div>
        </div>
      </div>
    </div>

    <ConfirmDialog
      v-model="showDeleteDialog"
      title="تأكيد الحذف"
      :loading="deleting"
      loading-label="جارٍ الحذف..."
      @confirm="handleDelete"
    >
      هل أنت متأكد من حذف الدورة
      <span class="text-fg font-medium">{{ deleteTarget?.title }}</span>
      ؟ لا يمكن التراجع عن هذا الإجراء.
    </ConfirmDialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { courseApi } from '@/services/api'
import { toast } from 'vue3-toastify'
import { resolveMediaUrl } from '@/utils/mediaUrl'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'

const courses = ref([])
const loading = ref(false)
const deleteTarget = ref(null)
const showDeleteDialog = ref(false)
const deleting = ref(false)
const statusLoadingId = ref(null)

onMounted(fetchCourses)

async function fetchCourses() {
  loading.value = true
  try {
    const res = await courseApi.getAll({ page: 1, pageSize: 50, status: 'all' })
    courses.value = res.data.items
  } finally { loading.value = false }
}

async function setCourseStatus(course, status) {
  statusLoadingId.value = course.id
  try {
    await courseApi.setStatus(course.id, status)
    toast.success(status === 'Published' ? 'تم نشر الدورة' : 'تم تحويل الدورة إلى مسودة')
    await fetchCourses()
  } finally {
    statusLoadingId.value = null
  }
}

function confirmDelete(course) {
  deleteTarget.value = course
  showDeleteDialog.value = true
}

async function handleDelete() {
  if (!deleteTarget.value) return
  deleting.value = true
  try {
    await courseApi.delete(deleteTarget.value.id)
    toast.success('تم الحذف')
    showDeleteDialog.value = false
    deleteTarget.value = null
    fetchCourses()
  } finally { deleting.value = false }
}
</script>
