<template>
  <div class="w-full max-w-none">
    <div class="flex flex-wrap items-start justify-between gap-4 mb-8">
      <div>
        <RouterLink :to="`/admin/courses/${courseId}/edit`" class="text-fg-mute hover:text-fg transition-colors text-sm block mb-2">
          ← العودة لتعديل الدورة
        </RouterLink>
        <h1 class="text-3xl font-bold text-fg">دروس الدورة</h1>
        <p v-if="courseTitle" class="text-gold/90 mt-1 font-medium">{{ courseTitle }}</p>
        <p class="text-fg-mute text-sm mt-2">
          لكل درس: رابط يوتيوب أو فيديو مباشر، أو رفع ملف (حتى 500 ميجابايت). يمكن جعل درس «معاينة» للجميع قبل التسجيل.
        </p>
      </div>
      <RouterLink to="/admin/courses" class="text-fg-mute hover:text-fg transition-colors text-sm shrink-0">
        كل الدورات ←
      </RouterLink>
    </div>

    <div v-if="pageLoading" class="card p-12 text-center text-fg-dim">جارٍ التحميل...</div>

    <template v-else>
      <div class="card overflow-hidden divide-y divide-line mb-8">
        <div v-if="!lessons.length" class="p-8 text-center text-fg-dim">لا توجد دروس بعد — أضف الدرس الأول بالأسفل.</div>
        <div
          v-for="(l, idx) in lessons"
          :key="l.id"
          class="p-4 flex flex-wrap items-start gap-4 justify-between hover:bg-input/40"
        >
          <div class="flex gap-3 min-w-0">
            <span class="flex-shrink-0 w-8 h-8 rounded-lg bg-gold/15 text-gold text-sm flex items-center justify-center font-bold">{{ idx + 1 }}</span>
            <div class="min-w-0">
              <p class="text-fg font-medium">{{ l.title }}</p>
              <p v-if="l.description" class="text-fg-dim text-xs line-clamp-2 mt-1">{{ l.description }}</p>
              <div class="flex flex-wrap gap-2 mt-2 text-xs text-fg-dim">
                <span>⏱ {{ l.durationMinutes }} د</span>
                <span v-if="l.isPreview" class="badge bg-gold/15 text-gold border-gold/30">معاينة</span>
              </div>
              <a
                v-if="l.videoUrl"
                :href="resolveMediaUrl(l.videoUrl)"
                target="_blank"
                rel="noopener"
                class="text-gold text-xs hover:underline mt-1 inline-block truncate max-w-md"
              >رابط الفيديو</a>
            </div>
          </div>
          <div class="flex flex-wrap gap-2 shrink-0">
            <button
              type="button"
              class="text-sm px-3 py-1.5 bg-gold/15 hover:bg-gold/25 text-gold rounded-lg"
              :disabled="lessonDeleteInProgress || lessonEditSaving"
              @click="openEditLesson(l)"
            >
              تعديل
            </button>
            <button
              type="button"
              class="text-sm px-3 py-1.5 bg-red-500/15 hover:bg-red-500/25 text-red-400 rounded-lg"
              :disabled="lessonDeleteInProgress"
              @click="confirmDeleteLesson(l)"
            >
              حذف
            </button>
          </div>
        </div>
      </div>

      <div class="card p-6">
        <h2 class="text-lg font-semibold text-fg mb-4">إضافة درس جديد</h2>
        <div class="grid md:grid-cols-2 gap-4">
          <div class="md:col-span-2">
            <label class="block text-sm text-fg-mute mb-2">عنوان الدرس *</label>
            <input v-model="lessonForm.title" type="text" class="input-field" placeholder="مثال: مقدمة في الرسم" required>
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm text-fg-mute mb-2">الوصف</label>
            <textarea v-model="lessonForm.description" class="textarea-field" rows="2" placeholder="نبذة قصيرة عن الدرس"></textarea>
          </div>
          <div class="md:col-span-2">
            <label class="block text-sm text-fg-mute mb-2">الفيديو *</label>
            <div class="flex flex-wrap gap-4 mb-3">
              <label class="flex items-center gap-2 cursor-pointer">
                <input v-model="lessonSource" type="radio" value="url" class="accent-gold">
                <span class="text-sm text-fg-soft">رابط (يوتيوب أو فيديو مباشر)</span>
              </label>
              <label class="flex items-center gap-2 cursor-pointer">
                <input v-model="lessonSource" type="radio" value="file" class="accent-gold">
                <span class="text-sm text-fg-soft">رفع ملف</span>
              </label>
            </div>
            <input
              v-if="lessonSource === 'url'"
              v-model="lessonForm.videoUrl"
              type="text"
              class="input-field"
              placeholder="https://www.youtube.com/watch?v=..."
            >
            <div v-else class="border-2 border-dashed border-line rounded-xl p-6 text-center">
              <input ref="lessonVideoInput" type="file" class="hidden" accept="video/*" @change="onLessonVideoChange">
              <button type="button" class="btn-outline text-sm" @click="() => lessonVideoInput?.click()">اختر ملف فيديو</button>
              <p v-if="lessonVideoFile" class="text-gold text-sm mt-2 truncate">{{ lessonVideoFile.name }}</p>
              <p class="text-fg-dim text-xs mt-2">MP4, WebM, MOV — حتى 500 ميجابايت</p>
            </div>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">المدة (دقيقة)</label>
            <input v-model.number="lessonForm.durationMinutes" type="number" class="input-field" min="0" step="1">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">ترتيب العرض</label>
            <input v-model.number="lessonForm.sortOrder" type="number" class="input-field" min="0" step="1">
          </div>
          <div class="md:col-span-2">
            <label class="flex items-center gap-3 cursor-pointer">
              <input v-model="lessonForm.isPreview" type="checkbox" class="accent-gold">
              <span class="text-sm text-fg-mute">درس معاينة (يظهر بدون تسجيل في الدورة)</span>
            </label>
          </div>
        </div>
        <button
          type="button"
          :disabled="lessonSaving"
          class="btn-primary mt-6"
          @click="handleAddLesson"
        >
          {{ lessonSaving ? 'جارٍ الإضافة...' : '+ إضافة الدرس' }}
        </button>
      </div>
    </template>

    <ConfirmDialog
      v-model="showLessonDeleteDialog"
      title="حذف الدرس"
      :loading="lessonDeleteInProgress"
      loading-label="جارٍ الحذف..."
      @confirm="executeDeleteLesson"
    >
      حذف الدرس
      <span class="text-fg font-medium">{{ lessonDeleteTarget?.title }}</span>
      ؟ لا يمكن التراجع.
    </ConfirmDialog>

    <!-- تعديل درس -->
    <Teleport to="body">
      <div
        v-if="showEditModal"
        class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/70 backdrop-blur-sm"
        role="dialog"
        aria-modal="true"
        @click.self="closeEditModal"
      >
        <div class="card w-full max-w-2xl max-h-[90vh] overflow-y-auto p-6 border border-gold/20 shadow-xl">
          <div class="flex items-start justify-between gap-4 mb-4">
            <h2 class="text-xl font-bold text-fg">تعديل الدرس</h2>
            <button type="button" class="text-fg-mute hover:text-fg text-2xl leading-none" aria-label="إغلاق" @click="closeEditModal">×</button>
          </div>
          <div class="grid md:grid-cols-2 gap-4">
            <div class="md:col-span-2">
              <label class="block text-sm text-fg-mute mb-2">عنوان الدرس *</label>
              <input v-model="editForm.title" type="text" class="input-field" required>
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm text-fg-mute mb-2">الوصف</label>
              <textarea v-model="editForm.description" class="textarea-field" rows="2"></textarea>
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm text-fg-mute mb-2">الفيديو</label>
              <div class="flex flex-wrap gap-4 mb-3">
                <label class="flex items-center gap-2 cursor-pointer">
                  <input v-model="editLessonSource" type="radio" value="url" class="accent-gold">
                  <span class="text-sm text-fg-soft">رابط</span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer">
                  <input v-model="editLessonSource" type="radio" value="file" class="accent-gold">
                  <span class="text-sm text-fg-soft">استبدال بملف مرفوع</span>
                </label>
              </div>
              <input
                v-if="editLessonSource === 'url'"
                v-model="editForm.videoUrl"
                type="text"
                class="input-field"
                placeholder="رابط يوتيوب أو فيديو مباشر"
              >
              <div v-else class="border-2 border-dashed border-line rounded-xl p-6 text-center">
                <input ref="editLessonVideoInput" type="file" class="hidden" accept="video/*" @change="onEditLessonVideoChange">
                <button type="button" class="btn-outline text-sm" @click="() => editLessonVideoInput?.click()">اختر ملف فيديو جديد</button>
                <p v-if="editLessonVideoFile" class="text-gold text-sm mt-2 truncate">{{ editLessonVideoFile.name }}</p>
                <p class="text-fg-dim text-xs mt-2">يُستبدل الفيديو الحالي عند الرفع</p>
              </div>
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">المدة (دقيقة)</label>
              <input v-model.number="editForm.durationMinutes" type="number" class="input-field" min="0" step="1">
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">ترتيب العرض</label>
              <input v-model.number="editForm.sortOrder" type="number" class="input-field" min="0" step="1">
            </div>
            <div class="md:col-span-2">
              <label class="flex items-center gap-3 cursor-pointer">
                <input v-model="editForm.isPreview" type="checkbox" class="accent-gold">
                <span class="text-sm text-fg-mute">درس معاينة</span>
              </label>
            </div>
          </div>
          <div class="flex flex-wrap gap-3 justify-end mt-6">
            <button type="button" class="btn-outline" :disabled="lessonEditSaving" @click="closeEditModal">إلغاء</button>
            <button type="button" class="btn-primary" :disabled="lessonEditSaving" @click="handleSaveEdit">
              {{ lessonEditSaving ? 'جارٍ الحفظ...' : 'حفظ التعديلات' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { courseApi } from '@/services/api'
import { toast } from 'vue3-toastify'
import { asciiFilenameForUpload } from '@/utils/safeUploadFile'
import { resolveMediaUrl } from '@/utils/mediaUrl'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'

const route = useRoute()
const router = useRouter()
const courseId = computed(() => route.params.id)

const pageLoading = ref(true)
const courseTitle = ref('')
const lessons = ref([])
const lessonForm = reactive({
  title: '', description: '', videoUrl: '', durationMinutes: 0, sortOrder: 1, isPreview: false
})
const lessonSaving = ref(false)
const lessonSource = ref('url')
const lessonVideoFile = ref(null)
const lessonVideoInput = ref(null)
const lessonDeleteTarget = ref(null)
const showLessonDeleteDialog = ref(false)
const lessonDeleteInProgress = ref(false)

const showEditModal = ref(false)
const lessonEditSaving = ref(false)
const editLessonId = ref(null)
const editLessonSource = ref('url')
const editLessonVideoFile = ref(null)
const editLessonVideoInput = ref(null)
const editForm = reactive({
  title: '', description: '', videoUrl: '', durationMinutes: 0, sortOrder: 0, isPreview: false
})

onMounted(async () => {
  await loadCourse()
})

watch(lessonSource, (v) => {
  if (v === 'url') {
    lessonVideoFile.value = null
    if (lessonVideoInput.value) lessonVideoInput.value.value = ''
  } else {
    lessonForm.videoUrl = ''
  }
})

watch(editLessonSource, (v) => {
  if (v === 'url') {
    editLessonVideoFile.value = null
    if (editLessonVideoInput.value) editLessonVideoInput.value.value = ''
  } else {
    editForm.videoUrl = ''
  }
})

function openEditLesson(l) {
  editLessonId.value = l.id
  Object.assign(editForm, {
    title: l.title ?? '',
    description: l.description ?? '',
    videoUrl: l.videoUrl ?? '',
    durationMinutes: Number(l.durationMinutes) || 0,
    sortOrder: Number(l.sortOrder) || 0,
    isPreview: !!l.isPreview
  })
  editLessonSource.value = 'url'
  editLessonVideoFile.value = null
  if (editLessonVideoInput.value) editLessonVideoInput.value.value = ''
  showEditModal.value = true
}

function closeEditModal() {
  showEditModal.value = false
  editLessonId.value = null
}

function onEditLessonVideoChange(e) {
  const f = e.target.files?.[0]
  editLessonVideoFile.value = f || null
}

async function handleSaveEdit() {
  if (!editLessonId.value) return
  if (!editForm.title?.trim()) {
    toast.error('عنوان الدرس مطلوب')
    return
  }
  const urlTrim = editForm.videoUrl?.trim() || ''
  const hasFile = editLessonSource.value === 'file' && editLessonVideoFile.value
  const hasUrl = editLessonSource.value === 'url' && urlTrim
  if (editLessonSource.value === 'file' && !hasFile) {
    toast.error('اختر ملف فيديو جديداً أو انتقل إلى «رابط» للإبقاء على الفيديو الحالي')
    return
  }
  lessonEditSaving.value = true
  try {
    const fd = new FormData()
    fd.append('title', editForm.title.trim())
    if (editForm.description?.trim()) fd.append('description', editForm.description.trim())
    if (hasUrl) fd.append('videoUrl', urlTrim)
    fd.append('durationMinutes', String(Number(editForm.durationMinutes) || 0))
    fd.append('sortOrder', String(Number(editForm.sortOrder) || 0))
    fd.append('isPreview', editForm.isPreview ? 'true' : 'false')
    fd.append('courseId', String(courseId.value))
    if (hasFile) fd.append('video', editLessonVideoFile.value, asciiFilenameForUpload(editLessonVideoFile.value))
    await courseApi.updateLesson(editLessonId.value, fd)
    toast.success('تم تحديث الدرس')
    closeEditModal()
    await refreshLessons()
  } finally {
    lessonEditSaving.value = false
  }
}

async function loadCourse() {
  pageLoading.value = true
  try {
    const cr = await courseApi.getById(courseId.value)
    const c = cr.data
    courseTitle.value = c.title ?? ''
    lessons.value = c.lessons || []
    lessonForm.sortOrder = (lessons.value.length || 0) + 1
  } catch {
    toast.error('تعذر تحميل الدورة أو غير موجودة')
    router.push('/admin/courses')
  } finally {
    pageLoading.value = false
  }
}

function onLessonVideoChange(e) {
  const f = e.target.files?.[0]
  lessonVideoFile.value = f || null
}

async function refreshLessons() {
  const cr = await courseApi.getById(courseId.value)
  lessons.value = cr.data.lessons || []
  lessonForm.sortOrder = (lessons.value.length || 0) + 1
}

async function handleAddLesson() {
  if (!lessonForm.title?.trim()) {
    toast.error('عنوان الدرس مطلوب')
    return
  }
  const urlTrim = lessonForm.videoUrl?.trim() || ''
  const hasFile = lessonSource.value === 'file' && lessonVideoFile.value
  const hasUrl = lessonSource.value === 'url' && urlTrim
  if (!hasUrl && !hasFile) {
    toast.error('أدخل رابط يوتيوب/فيديو أو ارفع ملفاً')
    return
  }
  lessonSaving.value = true
  try {
    const fd = new FormData()
    fd.append('title', lessonForm.title.trim())
    if (lessonForm.description?.trim()) fd.append('description', lessonForm.description.trim())
    if (hasUrl) fd.append('videoUrl', urlTrim)
    fd.append('durationMinutes', String(Number(lessonForm.durationMinutes) || 0))
    fd.append('sortOrder', String(Number(lessonForm.sortOrder) || 0))
    fd.append('isPreview', lessonForm.isPreview ? 'true' : 'false')
    fd.append('courseId', String(courseId.value))
    if (hasFile) fd.append('video', lessonVideoFile.value, asciiFilenameForUpload(lessonVideoFile.value))
    await courseApi.addLesson(fd)
    toast.success('تم إضافة الدرس')
    Object.assign(lessonForm, {
      title: '', description: '', videoUrl: '', durationMinutes: 0, isPreview: false
    })
    lessonSource.value = 'url'
    lessonVideoFile.value = null
    if (lessonVideoInput.value) lessonVideoInput.value.value = ''
    await refreshLessons()
  } finally {
    lessonSaving.value = false
  }
}

function confirmDeleteLesson(l) {
  lessonDeleteTarget.value = l
  showLessonDeleteDialog.value = true
}

async function executeDeleteLesson() {
  if (!lessonDeleteTarget.value) return
  lessonDeleteInProgress.value = true
  try {
    await courseApi.deleteLesson(lessonDeleteTarget.value.id)
    toast.success('تم حذف الدرس')
    showLessonDeleteDialog.value = false
    lessonDeleteTarget.value = null
    await refreshLessons()
  } finally {
    lessonDeleteInProgress.value = false
  }
}
</script>
