<template>
  <div>
    <!-- Header -->
    <div class="flex items-center gap-4 mb-8">
      <RouterLink to="/admin/courses" class="text-gray-400 hover:text-white transition-colors">← رجوع</RouterLink>
      <div>
        <h1 class="text-3xl font-bold text-white">دروس الدورة</h1>
        <p class="text-gray-400 mt-1 text-sm">{{ courseName }}</p>
      </div>
    </div>

    <div class="grid lg:grid-cols-5 gap-8">
      <!-- Lessons list -->
      <div class="lg:col-span-3 space-y-3">
        <div v-if="loading" class="space-y-3">
          <div v-for="i in 4" :key="i" class="card h-20 animate-pulse"></div>
        </div>

        <div v-else-if="lessons.length === 0" class="card p-10 text-center text-gray-500">
          <div class="text-4xl mb-3">📚</div>
          <p>لا توجد دروس بعد. أضف أول درس من النموذج.</p>
        </div>

        <div v-for="(lesson, index) in lessons" :key="lesson.id"
          class="card p-4 flex items-center gap-4 group">
          <span class="w-8 h-8 rounded-full bg-dark-300 text-gold text-sm font-bold flex items-center justify-center shrink-0">
            {{ index + 1 }}
          </span>
          <div class="flex-1 min-w-0">
            <p class="text-white font-medium truncate">{{ lesson.title }}</p>
            <div class="flex items-center gap-3 text-xs text-gray-500 mt-1">
              <span>{{ lesson.durationMinutes || 0 }} دقيقة</span>
              <span v-if="lesson.isPreview" class="text-green-400">معاينة</span>
              <span v-if="lesson.videoUrl" class="text-blue-400">📹 فيديو</span>
            </div>
          </div>
          <div class="flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
            <button @click="editLesson(lesson)"
              class="px-3 py-1.5 text-xs bg-dark-300 hover:bg-dark-200 text-white rounded-lg transition-colors">تعديل</button>
            <button @click="handleDeleteLesson(lesson.id)"
              class="px-3 py-1.5 text-xs bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-lg transition-colors">حذف</button>
          </div>
        </div>
      </div>

      <!-- Add/Edit Lesson form -->
      <div class="lg:col-span-2">
        <div class="card p-6 sticky top-6">
          <h2 class="text-white font-bold text-lg mb-5">
            {{ editingLesson ? 'تعديل الدرس' : 'إضافة درس جديد' }}
          </h2>
          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">عنوان الدرس *</label>
              <input v-model="form.title" type="text" class="input-field" required placeholder="مثال: مقدمة في الرسم">
            </div>

            <div>
              <label class="block text-sm text-gray-400 mb-2">رابط الفيديو</label>
              <input v-model="form.videoUrl" type="url" class="input-field" placeholder="https://...">
              <p class="text-xs text-gray-500 mt-1">يوتيوب، فيميو، أو رابط مباشر</p>
            </div>

            <div>
              <label class="block text-sm text-gray-400 mb-2">وصف الدرس</label>
              <textarea v-model="form.description" class="textarea-field" rows="4"
                placeholder="وصف الدرس أو ملاحظات..."></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm text-gray-400 mb-2">المدة (دقيقة)</label>
                <input v-model.number="form.durationMinutes" type="number" class="input-field" min="0" placeholder="0">
              </div>
              <div>
                <label class="block text-sm text-gray-400 mb-2">الترتيب</label>
                <input v-model.number="form.sortOrder" type="number" class="input-field" min="1"
                  :placeholder="lessons.length + 1">
              </div>
            </div>

            <label class="flex items-center gap-3 cursor-pointer">
              <input v-model="form.isPreview" type="checkbox" class="accent-gold">
              <span class="text-sm text-gray-400">درس معاينة (متاح بدون اشتراك)</span>
            </label>

            <div class="flex gap-3 pt-2">
              <button type="submit" :disabled="saving" class="btn-primary flex-1 justify-center py-3">
                {{ saving ? 'جارٍ الحفظ...' : (editingLesson ? 'حفظ التعديلات' : 'إضافة الدرس') }}
              </button>
              <button v-if="editingLesson" type="button" @click="cancelEdit"
                class="px-4 py-3 text-sm bg-dark-300 hover:bg-dark-200 text-white rounded-xl transition-colors">إلغاء</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { courseApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const route = useRoute()
const courseId = route.params.id

const lessons = ref([])
const loading = ref(false)
const saving = ref(false)
const courseName = ref('')
const editingLesson = ref(null)

const defaultForm = { title: '', videoUrl: '', description: '', durationMinutes: 0, sortOrder: null, isPreview: false }
const form = reactive({ ...defaultForm })

onMounted(fetchLessons)

async function fetchLessons() {
  loading.value = true
  try {
    const res = await courseApi.getLessons(courseId)
    lessons.value = (Array.isArray(res) ? res : (res.data || []))
    lessons.value.sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0))
  } catch {
    // Backend may not be running; keep empty
  } finally {
    loading.value = false
  }
}

async function handleSubmit() {
  saving.value = true
  try {
    const payload = {
      ...form,
      courseId: Number(courseId),
      sortOrder: form.sortOrder || lessons.value.length + 1
    }
    if (editingLesson.value) {
      await courseApi.updateLesson(editingLesson.value.id, payload)
      toast.success('تم تحديث الدرس')
    } else {
      await courseApi.addLesson(payload)
      toast.success('تمت إضافة الدرس')
    }
    cancelEdit()
    fetchLessons()
  } finally {
    saving.value = false }
}

function editLesson(lesson) {
  editingLesson.value = lesson
  Object.assign(form, {
    title: lesson.title,
    videoUrl: lesson.videoUrl || '',
    description: lesson.description || '',
    durationMinutes: lesson.durationMinutes || 0,
    sortOrder: lesson.sortOrder || null,
    isPreview: lesson.isPreview || false,
  })
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function cancelEdit() {
  editingLesson.value = null
  Object.assign(form, { ...defaultForm })
}

async function handleDeleteLesson(id) {
  if (!confirm('حذف هذا الدرس؟')) return
  await courseApi.deleteLesson(id)
  toast.success('تم حذف الدرس')
  fetchLessons()
}
</script>
