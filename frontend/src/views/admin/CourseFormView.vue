<template>
  <div>
    <div class="flex items-center gap-4 mb-8">
      <RouterLink to="/admin/courses" class="text-fg-mute hover:text-fg transition-colors">← رجوع</RouterLink>
      <h1 class="text-3xl font-bold text-fg">{{ isEdit ? 'تعديل الدورة' : 'دورة جديدة' }}</h1>
    </div>

    <form @submit.prevent="handleSubmit" class="grid lg:grid-cols-3 gap-8">
      <div class="lg:col-span-2 space-y-6">
        <div class="card p-6 space-y-5">
          <div>
            <label class="block text-sm text-fg-mute mb-2">عنوان الدورة *</label>
            <input v-model="form.title" type="text" class="input-field" required>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">وصف مختصر</label>
            <textarea v-model="form.shortDescription" class="textarea-field" rows="3"></textarea>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">الوصف التفصيلي</label>
            <textarea v-model="form.description" class="textarea-field" rows="6"></textarea>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">رابط فيديو المعاينة</label>
            <input v-model="form.previewVideoUrl" type="url" class="input-field" placeholder="https://...">
          </div>
        </div>
      </div>

      <div class="space-y-6">
        <div class="card p-6">
          <label class="block text-sm text-fg-mute mb-3">صورة الدورة</label>
          <div class="border-2 border-dashed border-line rounded-xl p-6 text-center cursor-pointer hover:border-gold/40 transition-colors"
               @click="$refs.thumbInput.click()">
            <img v-if="previewUrl" :src="previewUrl" class="w-full rounded-lg aspect-video object-cover" alt="">
            <div v-else class="text-fg-dim">
              <div class="text-3xl mb-2">🖼️</div>
              <p class="text-sm">رفع صورة الغلاف</p>
            </div>
          </div>
          <input ref="thumbInput" type="file" class="hidden" accept="image/*" @change="onThumbChange">
        </div>

        <div class="card p-6 space-y-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">التصنيف *</label>
            <select v-model="form.categoryId" class="input-field" required>
              <option value="">اختر تصنيفاً</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">المستوى</label>
            <select v-model="form.level" class="input-field">
              <option value="Beginner">مبتدئ</option>
              <option value="Intermediate">متوسط</option>
              <option value="Advanced">متقدم</option>
            </select>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">السعر بالدينار العراقي (0 = مجاني)</label>
            <input v-model.number="form.price" type="number" class="input-field" min="0" step="0.01">
          </div>
          <label class="flex items-center gap-3 cursor-pointer">
            <input v-model="form.isFeatured" type="checkbox" class="accent-gold">
            <span class="text-sm text-fg-mute">دورة مميزة</span>
          </label>
        </div>

        <button type="submit" :disabled="loading" class="btn-primary w-full justify-center py-4">
          {{ loading ? 'جارٍ الحفظ...' : (isEdit ? 'حفظ التعديلات' : 'إنشاء الدورة') }}
        </button>
      </div>
    </form>

    <div v-if="isEdit" class="mt-10 max-w-4xl">
      <div class="card p-5 flex flex-wrap items-center justify-between gap-4 border border-gold/20 bg-gold/5">
        <div>
          <h2 class="text-lg font-semibold text-fg">دروس الدورة</h2>
          <p class="text-fg-mute text-sm mt-1">إدارة الدروس والفيديوهات في صفحة مخصصة.</p>
        </div>
        <RouterLink :to="`/admin/courses/${route.params.id}/lessons`" class="btn-primary shrink-0">
          فتح صفحة الدروس
        </RouterLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { courseApi, categoryApi } from '@/services/api'
import { toast } from 'vue3-toastify'
import { resolveMediaUrl } from '@/utils/mediaUrl'
import { asciiFilenameForUpload } from '@/utils/safeUploadFile'
const route = useRoute()
const router = useRouter()
const isEdit = computed(() => !!route.params.id)
const categories = ref([])
const loading = ref(false)
const thumbFile = ref(null)
const previewUrl = ref(null)
const thumbInput = ref(null)

const form = reactive({ title: '', shortDescription: '', description: '', previewVideoUrl: '', categoryId: '', level: 'Beginner', price: 0, isFeatured: false })

onMounted(async () => {
  const res = await categoryApi.getAll()
  categories.value = res.data

  if (!isEdit.value) return

  try {
    const cr = await courseApi.getById(route.params.id)
    const c = cr.data
    Object.assign(form, {
      title: c.title ?? '',
      shortDescription: c.shortDescription ?? '',
      description: c.description ?? '',
      previewVideoUrl: c.previewVideoUrl ?? '',
      categoryId: Number(c.categoryId),
      level: c.level ?? 'Beginner',
      price: Number(c.price ?? 0),
      isFeatured: !!c.isFeatured
    })
    previewUrl.value = c.thumbnailUrl ? resolveMediaUrl(c.thumbnailUrl) : null
  } catch {
    toast.error('تعذر تحميل بيانات الدورة')
    router.push('/admin/courses')
  }
})

function onThumbChange(e) {
  const file = e.target.files?.[0]
  if (!file) return
  thumbFile.value = file
  previewUrl.value = URL.createObjectURL(file)
}

async function handleSubmit() {
  loading.value = true
  try {
    const fd = new FormData()
    Object.entries(form).forEach(([k, v]) => v !== null && fd.append(k, v))
    if (thumbFile.value) fd.append('thumbnail', thumbFile.value, asciiFilenameForUpload(thumbFile.value))

    if (isEdit.value) {
      await courseApi.update(route.params.id, fd)
      toast.success('تم تحديث الدورة')
    } else {
      const created = await courseApi.create(fd)
      toast.success('تم إنشاء الدورة — أضف الدروس من الصفحة التالية')
      const newId = created?.data?.id
      router.push(newId ? `/admin/courses/${newId}/lessons` : '/admin/courses')
    }
  } finally { loading.value = false }
}
</script>
