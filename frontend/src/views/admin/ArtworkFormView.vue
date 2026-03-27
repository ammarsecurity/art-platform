<template>
  <div>
    <div class="flex items-center gap-4 mb-8">
      <RouterLink to="/admin/artworks" class="text-fg-mute hover:text-fg transition-colors">← رجوع</RouterLink>
      <h1 class="text-3xl font-bold text-fg">{{ isEdit ? 'تعديل العمل الفني' : 'إضافة عمل فني جديد' }}</h1>
    </div>

    <form @submit.prevent="handleSubmit" class="grid lg:grid-cols-3 gap-8">
      <!-- Main fields -->
      <div class="lg:col-span-2 space-y-6">
        <div class="card p-6 space-y-5">
          <div>
            <label class="block text-sm text-fg-mute mb-2">عنوان العمل *</label>
            <input v-model="form.title" type="text" class="input-field" placeholder="اسم اللوحة أو العمل الفني" required>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">الوصف</label>
            <textarea v-model="form.description" class="textarea-field" rows="5"
              placeholder="وصف العمل الفني، الإلهام، والمواد المستخدمة..."></textarea>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm text-fg-mute mb-2">الخامة / الأسلوب</label>
              <input v-model="form.medium" type="text" class="input-field" placeholder="زيت على قماش">
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">الأبعاد</label>
              <input v-model="form.dimensions" type="text" class="input-field" placeholder="60 × 80 سم">
            </div>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">التاجات (افصل بمسافة)</label>
            <input v-model="tagsInput" type="text" class="input-field" placeholder="لوحة زيتية، تجريدي، طبيعة">
          </div>
        </div>
      </div>

      <!-- Sidebar fields -->
      <div class="space-y-6">
        <!-- Image upload -->
        <div class="card p-6">
          <label class="block text-sm text-fg-mute mb-3">صورة العمل {{ isEdit ? '(اتركها فارغة للإبقاء على الصورة الحالية)' : '*' }}</label>
          <div
            class="border-2 border-dashed border-line rounded-xl p-8 text-center cursor-pointer hover:border-gold/40 transition-colors"
            :class="previewUrl ? 'p-0 border-0' : ''"
            @click="$refs.imageInput.click()"
          >
            <img v-if="previewUrl" :src="previewUrl" class="w-full rounded-xl aspect-square object-cover" alt="معاينة">
            <div v-else>
              <div class="text-4xl mb-3">🖼️</div>
              <p class="text-fg-mute text-sm">اضغط لرفع الصورة</p>
              <p class="text-fg-dim text-xs mt-1">JPG, PNG, WebP — بحد أقصى 10 ميجابايت</p>
            </div>
          </div>
          <input ref="imageInput" type="file" class="hidden" accept="image/*" @change="handleImageChange">
          <button v-if="previewUrl" type="button" @click="clearImage"
            class="mt-2 text-red-400 text-xs hover:text-red-300 transition-colors">✕ حذف الصورة</button>
        </div>

        <div class="card p-6 space-y-5">
          <div>
            <label class="block text-sm text-fg-mute mb-2">التصنيف *</label>
            <select v-model="form.categoryId" class="input-field" required>
              <option value="">اختر التصنيف</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">السنة</label>
            <input v-model.number="form.year" type="number" class="input-field" :placeholder="new Date().getFullYear()" min="1900" :max="new Date().getFullYear()">
          </div>
          <div v-if="isEdit">
            <label class="block text-sm text-fg-mute mb-2">الحالة</label>
            <select v-model="form.status" class="input-field">
              <option value="Published">منشور</option>
              <option value="Draft">مسودة</option>
              <option value="Archived">مؤرشف</option>
            </select>
          </div>
          <label class="flex items-center gap-3 cursor-pointer">
            <div class="relative">
              <input v-model="form.isFeatured" type="checkbox" class="sr-only peer">
              <div class="w-10 h-6 bg-line rounded-full peer-checked:bg-gold transition-colors"></div>
              <div class="absolute top-1 right-1 w-4 h-4 bg-white rounded-full transition-transform peer-checked:translate-x-[-16px]"></div>
            </div>
            <span class="text-sm text-fg-mute">تمييز هذا العمل ⭐</span>
          </label>
        </div>

        <button type="submit" :disabled="loading" class="btn-primary w-full justify-center py-4 text-base">
          <span v-if="loading" class="inline-block w-5 h-5 border-2 border-line/40 border-t-gold rounded-full animate-spin ml-2"></span>
          {{ loading ? 'جارٍ الحفظ...' : (isEdit ? 'حفظ التعديلات' : 'نشر العمل الفني') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { artworkApi, categoryApi } from '@/services/api'
import { asciiFilenameForUpload } from '@/utils/safeUploadFile'
import { toast } from 'vue3-toastify'
import { resolveMediaUrl } from '@/utils/mediaUrl'

const route = useRoute()
const router = useRouter()
const isEdit = computed(() => !!route.params.id)
const categories = ref([])
const loading = ref(false)
const imageFile = ref(null)
const previewUrl = ref(null)
const imageInput = ref(null)
const tagsInput = ref('')

const form = reactive({
  title: '', description: '', medium: '', dimensions: '',
  year: null, categoryId: '', isFeatured: false, status: 'Published'
})

onMounted(async () => {
  const res = await categoryApi.getAll()
  categories.value = res.data

  if (isEdit.value) {
    const artRes = await artworkApi.getById(route.params.id)
    const a = artRes.data
    Object.assign(form, { title: a.title, description: a.description, medium: a.medium, dimensions: a.dimensions, year: a.year, categoryId: a.categoryId, isFeatured: a.isFeatured, status: a.status })
    tagsInput.value = a.tags?.join(' ') || ''
    previewUrl.value = resolveMediaUrl(a.imageUrl)
  }
})

function handleImageChange(e) {
  const file = e.target.files[0]
  if (!file) return
  imageFile.value = file
  previewUrl.value = URL.createObjectURL(file)
}

function clearImage() { imageFile.value = null; previewUrl.value = null }

async function handleSubmit() {
  if (!isEdit.value && !imageFile.value) return toast.error('يرجى رفع صورة للعمل الفني')

  loading.value = true
  try {
    const fd = new FormData()
    Object.entries(form).forEach(([k, v]) => v !== null && fd.append(k, v))
    tagsInput.value.split(/[\s,]+/).filter(Boolean).forEach(t => fd.append('tags', t))
    if (imageFile.value) fd.append('image', imageFile.value, asciiFilenameForUpload(imageFile.value))

    if (isEdit.value) {
      await artworkApi.update(route.params.id, fd)
      toast.success('تم تحديث العمل الفني')
    } else {
      await artworkApi.create(fd)
      toast.success('تم نشر العمل الفني')
    }
    router.push('/admin/artworks')
  } finally { loading.value = false }
}
</script>
