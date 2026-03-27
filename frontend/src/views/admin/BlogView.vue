<template>
  <div>
    <div class="flex flex-wrap items-center justify-between gap-4 mb-6">
      <h1 class="text-3xl font-bold text-fg">إدارة المدونة</h1>
      <button type="button" class="btn-primary" @click="openForm()">+ مقال جديد</button>
    </div>

    <!-- فلاتر الحالة + فرز القائمة -->
    <div class="flex flex-wrap items-center gap-3 mb-6">
      <div class="flex flex-wrap gap-2">
        <button
          v-for="opt in statusOptions"
          :key="opt.value"
          type="button"
          class="px-4 py-2 rounded-lg text-sm transition-colors border"
          :class="statusFilter === opt.value
            ? 'bg-gold/20 text-gold border-gold/40'
            : 'bg-input text-fg-mute border-line hover:border-gold/30'"
          @click="statusFilter = opt.value; fetchPosts()"
        >
          {{ opt.label }}
        </button>
      </div>
      <div class="flex items-center gap-2 mr-auto">
        <label class="text-sm text-fg-dim whitespace-nowrap">ترتيب القائمة</label>
        <select v-model="listSortBy" class="input-field py-2 text-sm min-w-[10rem]" @change="fetchPosts">
          <option value="sortorder">ترتيب العرض</option>
          <option value="publishedat">تاريخ النشر</option>
          <option value="createdat">تاريخ الإنشاء</option>
          <option value="title">العنوان</option>
        </select>
      </div>
    </div>

    <div class="card overflow-hidden">
      <div v-if="listLoading" class="p-12 text-center text-fg-dim">جارٍ التحميل...</div>
      <table v-else class="w-full">
        <thead>
          <tr class="border-b border-line text-right">
            <th class="px-4 py-3 text-xs text-fg-mute w-20">الترتيب</th>
            <th class="px-4 py-3 text-xs text-fg-mute min-w-[12rem]">العنوان</th>
            <th class="px-4 py-3 text-xs text-fg-mute">الحالة</th>
            <th class="px-4 py-3 text-xs text-fg-mute">المشاهدات</th>
            <th class="px-4 py-3 text-xs text-fg-mute">التاريخ</th>
            <th class="px-4 py-3 text-xs text-fg-mute w-40"></th>
          </tr>
        </thead>
        <tbody class="divide-y divide-line">
          <tr v-for="post in posts" :key="post.id" class="hover:bg-input/40 transition-colors group">
            <td class="px-4 py-3 text-fg-mute text-sm tabular-nums align-top">{{ post.sortOrder ?? 0 }}</td>
            <td class="px-4 py-3">
              <div class="flex items-center gap-3 min-w-0">
                <img
                  v-if="post.featuredImageUrl"
                  :src="resolveMediaUrl(post.featuredImageUrl)"
                  :alt="post.title"
                  class="w-12 h-12 rounded-lg object-cover flex-shrink-0 bg-line"
                >
                <span class="text-fg font-medium text-sm line-clamp-2">{{ post.title }}</span>
              </div>
            </td>
            <td class="px-4 py-3">
              <span
                class="badge text-xs"
                :class="post.status === 'Published'
                  ? 'bg-green-500/20 text-green-400 border border-green-500/30'
                  : post.status === 'Archived'
                    ? 'bg-gray-600/30 text-fg-soft border border-gray-500/30'
                    : 'bg-yellow-500/20 text-yellow-400 border border-yellow-500/30'"
              >
                {{ post.status === 'Published' ? 'منشور' : post.status === 'Archived' ? 'مؤرشف' : 'مسودة' }}
              </span>
            </td>
            <td class="px-4 py-3 text-fg-mute text-sm">{{ post.viewCount }}</td>
            <td class="px-4 py-3 text-fg-dim text-xs whitespace-nowrap">
              {{ new Date(post.publishedAt || post.createdAt).toLocaleDateString('ar-SA') }}
            </td>
            <td class="px-4 py-3">
              <div class="flex flex-wrap gap-2 justify-end opacity-100 sm:opacity-0 sm:group-hover:opacity-100 transition-opacity">
                <button type="button" class="px-3 py-1 text-xs bg-line hover:bg-input text-fg rounded-lg" @click="openForm(post)">تعديل</button>
                <button type="button" class="px-3 py-1 text-xs bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-lg" @click="confirmDelete(post)">حذف</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-if="!listLoading && !posts.length" class="p-12 text-center text-fg-dim">لا توجد مقالات ضمن هذا الفلتر.</div>
    </div>

    <!-- نموذج المقال -->
    <Teleport to="body">
      <div
        v-if="showForm"
        class="fixed inset-0 bg-black/70 backdrop-blur-sm z-50 flex items-start justify-center p-4 overflow-y-auto"
        @click.self="closeForm"
      >
        <div class="card p-8 max-w-2xl w-full my-8 border border-line">
          <h3 class="text-xl font-bold text-fg mb-6">{{ editPost ? 'تعديل المقال' : 'مقال جديد' }}</h3>

          <div v-if="loadingEdit" class="py-16 text-center text-fg-dim">جارٍ تحميل المقال...</div>

          <form v-else @submit.prevent="handleSave" class="space-y-5">
            <div>
              <label class="block text-sm text-fg-mute mb-2">العنوان *</label>
              <input v-model="form.title" type="text" class="input-field" required>
            </div>

            <div>
              <label class="block text-sm text-fg-mute mb-2">مقتطف</label>
              <textarea v-model="form.excerpt" class="textarea-field" rows="2" placeholder="يظهر في بطاقات المدونة"></textarea>
            </div>

            <!-- صورة الغلاف أولاً — ثم المحتوى -->
            <div>
              <label class="block text-sm text-fg-mute mb-2">صورة الغلاف</label>
              <div
                class="border-2 border-dashed border-line rounded-xl p-6 text-center cursor-pointer hover:border-gold/35 transition-colors"
                @click="imageInput?.click()"
              >
                <input ref="imageInput" type="file" class="hidden" accept="image/*" @change="onImageChange">
                <img
                  v-if="formImagePreview"
                  :src="formImagePreview"
                  alt=""
                  class="max-h-48 mx-auto rounded-lg object-contain"
                >
                <div v-else class="text-fg-dim text-sm">انقر لاختيار صورة (JPG, PNG, WebP)</div>
                <p v-if="imageFile" class="text-gold text-xs mt-2 truncate">{{ imageFile.name }}</p>
              </div>
              <p class="text-fg-dim text-xs mt-2">يُفضّل نسبة عرض مناسبة للبطاقات (مثلاً 16:9). يُستخدم اسم ملف لاتيني عند الرفع.</p>
            </div>

            <div>
              <label class="block text-sm text-fg-mute mb-2">المحتوى *</label>
              <textarea v-model="form.content" class="textarea-field font-mono text-sm" rows="12" required placeholder="يدعم Markdown أو نصاً عادياً"></textarea>
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm text-fg-mute mb-2">ترتيب العرض</label>
                <input v-model.number="form.sortOrder" type="number" class="input-field" min="0" step="1">
                <p class="text-fg-dim text-xs mt-1">رقم أعلى = يظهر أولاً عند الفرز الافتراضي</p>
              </div>
              <div>
                <label class="block text-sm text-fg-mute mb-2">حالة النشر</label>
                <select v-model="form.status" class="input-field">
                  <option value="Draft">مسودة</option>
                  <option value="Published">منشور</option>
                  <option value="Archived">مؤرشف</option>
                </select>
              </div>
            </div>

            <label class="flex items-center gap-3 cursor-pointer">
              <input v-model="form.isFeatured" type="checkbox" class="accent-gold">
              <span class="text-sm text-fg-mute">تمييز هذا المقال في الصفحة الرئيسية للمدونة</span>
            </label>

            <details class="text-sm">
              <summary class="text-fg-mute cursor-pointer hover:text-gold">SEO — اختياري</summary>
              <div class="mt-3 space-y-3 pt-2 border-t border-line">
                <div>
                  <label class="block text-xs text-fg-dim mb-1">Meta Title</label>
                  <input v-model="form.metaTitle" type="text" class="input-field">
                </div>
                <div>
                  <label class="block text-xs text-fg-dim mb-1">Meta Description</label>
                  <textarea v-model="form.metaDescription" class="textarea-field" rows="2"></textarea>
                </div>
              </div>
            </details>

            <div class="flex gap-3 pt-2">
              <button type="button" class="btn-ghost flex-1 justify-center border border-line rounded-lg py-3" @click="closeForm">إلغاء</button>
              <button type="submit" :disabled="saving" class="btn-primary flex-1 justify-center">{{ saving ? 'جارٍ الحفظ...' : 'حفظ' }}</button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>

    <ConfirmDialog
      v-model="showDeleteDialog"
      title="تأكيد الحذف"
      :loading="deleting"
      loading-label="جارٍ الحذف..."
      @confirm="handleDelete"
    >
      هل أنت متأكد من حذف المقال
      <span class="text-fg font-medium">{{ deleteTarget?.title }}</span>
      ؟ لا يمكن التراجع عن هذا الإجراء.
    </ConfirmDialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { blogApi } from '@/services/api'
import { asciiFilenameForUpload } from '@/utils/safeUploadFile'
import { resolveMediaUrl } from '@/utils/mediaUrl'
import { toast } from 'vue3-toastify'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'

const posts = ref([])
const listLoading = ref(false)
const statusFilter = ref('all')
const listSortBy = ref('sortorder')
const statusOptions = [
  { value: 'all', label: 'الكل' },
  { value: 'Published', label: 'منشور' },
  { value: 'Draft', label: 'مسودة' },
  { value: 'Archived', label: 'مؤرشف' }
]

const showForm = ref(false)
const showDeleteDialog = ref(false)
const deleteTarget = ref(null)
const deleting = ref(false)
const editPost = ref(null)
const saving = ref(false)
const loadingEdit = ref(false)
const imageFile = ref(null)
const imageInput = ref(null)
const formImagePreview = ref('')

const form = reactive({
  title: '',
  excerpt: '',
  content: '',
  status: 'Draft',
  isFeatured: false,
  sortOrder: 0,
  metaTitle: '',
  metaDescription: ''
})

onMounted(fetchPosts)

async function fetchPosts() {
  listLoading.value = true
  try {
    const res = await blogApi.getAll({
      page: 1,
      pageSize: 100,
      status: statusFilter.value,
      sortBy: listSortBy.value,
      sortOrder: 'desc'
    })
    posts.value = res.data.items
  } finally {
    listLoading.value = false
  }
}

function nextSortOrder() {
  const nums = posts.value.map(p => Number(p.sortOrder) || 0)
  return (nums.length ? Math.max(...nums) : 0) + 1
}

function revokePreview() {
  if (formImagePreview.value?.startsWith('blob:')) {
    URL.revokeObjectURL(formImagePreview.value)
  }
  formImagePreview.value = ''
}

function onImageChange(e) {
  const f = e.target.files?.[0]
  imageFile.value = f || null
  revokePreview()
  if (f) formImagePreview.value = URL.createObjectURL(f)
}

function closeForm() {
  revokePreview()
  imageFile.value = null
  if (imageInput.value) imageInput.value.value = ''
  showForm.value = false
  editPost.value = null
  loadingEdit.value = false
}

async function openForm(post = null) {
  editPost.value = post
  imageFile.value = null
  if (imageInput.value) imageInput.value.value = ''
  revokePreview()
  formImagePreview.value = ''

  if (post) {
    loadingEdit.value = true
    showForm.value = true
    try {
      const res = await blogApi.getById(post.id)
      const d = res.data
      Object.assign(form, {
        title: d.title ?? '',
        excerpt: d.excerpt ?? '',
        content: d.content ?? '',
        status: d.status ?? 'Draft',
        isFeatured: !!d.isFeatured,
        sortOrder: Number(d.sortOrder) || 0,
        metaTitle: d.metaTitle ?? '',
        metaDescription: d.metaDescription ?? ''
      })
      if (d.featuredImageUrl) formImagePreview.value = resolveMediaUrl(d.featuredImageUrl)
    } catch {
      toast.error('تعذر تحميل المقال')
      showForm.value = false
    } finally {
      loadingEdit.value = false
    }
  } else {
    Object.assign(form, {
      title: '',
      excerpt: '',
      content: '',
      status: 'Draft',
      isFeatured: false,
      sortOrder: nextSortOrder(),
      metaTitle: '',
      metaDescription: ''
    })
    showForm.value = true
  }
}

async function handleSave() {
  saving.value = true
  try {
    const fd = new FormData()
    fd.append('title', form.title.trim())
    fd.append('content', form.content)
    if (form.excerpt != null) fd.append('excerpt', form.excerpt)
    fd.append('status', form.status)
    fd.append('isFeatured', form.isFeatured ? 'true' : 'false')
    fd.append('sortOrder', String(Number(form.sortOrder) || 0))
    if (form.metaTitle?.trim()) fd.append('metaTitle', form.metaTitle.trim())
    if (form.metaDescription?.trim()) fd.append('metaDescription', form.metaDescription.trim())
    if (imageFile.value) fd.append('image', imageFile.value, asciiFilenameForUpload(imageFile.value))

    if (editPost.value) {
      await blogApi.update(editPost.value.id, fd)
      toast.success('تم تحديث المقال')
    } else {
      await blogApi.create(fd)
      toast.success('تم حفظ المقال')
    }
    closeForm()
    fetchPosts()
  } finally {
    saving.value = false
  }
}

function confirmDelete(post) {
  deleteTarget.value = post
  showDeleteDialog.value = true
}

async function handleDelete() {
  if (!deleteTarget.value) return
  deleting.value = true
  try {
    await blogApi.delete(deleteTarget.value.id)
    toast.success('تم الحذف')
    showDeleteDialog.value = false
    deleteTarget.value = null
    fetchPosts()
  } finally {
    deleting.value = false
  }
}
</script>
