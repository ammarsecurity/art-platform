<template>
  <div>
    <div class="flex items-center justify-between mb-8">
      <h1 class="text-3xl font-bold text-white">إدارة المدونة</h1>
      <button @click="openForm()" class="btn-primary">+ مقال جديد</button>
    </div>

    <div class="card overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-dark-300 text-right">
            <th class="px-4 py-3 text-xs text-gray-400">العنوان</th>
            <th class="px-4 py-3 text-xs text-gray-400">الحالة</th>
            <th class="px-4 py-3 text-xs text-gray-400">المشاهدات</th>
            <th class="px-4 py-3 text-xs text-gray-400">التاريخ</th>
            <th class="px-4 py-3 text-xs text-gray-400"></th>
          </tr>
        </thead>
        <tbody class="divide-y divide-dark-300">
          <tr v-for="post in posts" :key="post.id" class="hover:bg-dark-200/40 transition-colors group">
            <td class="px-4 py-3">
              <div class="flex items-center gap-3">
                <img v-if="post.featuredImageUrl" :src="post.featuredImageUrl" :alt="post.title"
                  class="w-10 h-10 rounded-lg object-cover">
                <span class="text-white font-medium text-sm line-clamp-1">{{ post.title }}</span>
              </div>
            </td>
            <td class="px-4 py-3">
              <span class="badge text-xs" :class="post.status === 'Published' ? 'bg-green-500/20 text-green-400 border border-green-500/30' : 'bg-yellow-500/20 text-yellow-400 border border-yellow-500/30'">
                {{ post.status === 'Published' ? 'منشور' : 'مسودة' }}
              </span>
            </td>
            <td class="px-4 py-3 text-gray-400 text-sm">{{ post.viewCount }}</td>
            <td class="px-4 py-3 text-gray-500 text-xs">{{ new Date(post.createdAt).toLocaleDateString('ar-SA') }}</td>
            <td class="px-4 py-3">
              <div class="flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                <button @click="openForm(post)" class="px-3 py-1 text-xs bg-dark-300 hover:bg-dark-200 text-white rounded-lg">تعديل</button>
                <button @click="handleDelete(post.id)" class="px-3 py-1 text-xs bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-lg">حذف</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Form Modal -->
    <Teleport to="body">
      <div v-if="showForm" class="fixed inset-0 bg-black/70 backdrop-blur-sm z-50 flex items-start justify-center p-4 overflow-y-auto" @click.self="showForm = false">
        <div class="card p-8 max-w-2xl w-full my-8">
          <h3 class="text-xl font-bold text-white mb-6">{{ editPost ? 'تعديل المقال' : 'مقال جديد' }}</h3>
          <form @submit.prevent="handleSave" class="space-y-5">
            <div>
              <label class="block text-sm text-gray-400 mb-2">العنوان *</label>
              <input v-model="form.title" type="text" class="input-field" required>
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">مقتطف</label>
              <textarea v-model="form.excerpt" class="textarea-field" rows="2"></textarea>
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">المحتوى *</label>
              <textarea v-model="form.content" class="textarea-field" rows="10" required></textarea>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm text-gray-400 mb-2">الحالة</label>
                <select v-model="form.status" class="input-field">
                  <option value="Draft">مسودة</option>
                  <option value="Published">منشور</option>
                </select>
              </div>
              <div>
                <label class="block text-sm text-gray-400 mb-2">صورة المقال</label>
                <input type="file" ref="imageInput" accept="image/*" class="input-field text-sm py-2" @change="e => imageFile = e.target.files[0]">
              </div>
            </div>
            <label class="flex items-center gap-3 cursor-pointer">
              <input v-model="form.isFeatured" type="checkbox" class="accent-gold">
              <span class="text-sm text-gray-400">تمييز هذا المقال</span>
            </label>
            <div class="flex gap-3 pt-2">
              <button type="button" @click="showForm = false" class="btn-ghost flex-1 justify-center border border-dark-300 rounded-lg py-3">إلغاء</button>
              <button type="submit" :disabled="saving" class="btn-primary flex-1 justify-center">{{ saving ? 'جارٍ الحفظ...' : 'حفظ' }}</button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { blogApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const posts = ref([])
const showForm = ref(false)
const editPost = ref(null)
const saving = ref(false)
const imageFile = ref(null)
const form = reactive({ title: '', excerpt: '', content: '', status: 'Draft', isFeatured: false })

onMounted(fetchPosts)

async function fetchPosts() {
  const res = await blogApi.getAll({ page: 1, pageSize: 50, status: 'all' })
  posts.value = res.data.items
}

function openForm(post = null) {
  editPost.value = post
  imageFile.value = null
  Object.assign(form, post
    ? { title: post.title, excerpt: post.excerpt || '', content: '', status: post.status, isFeatured: post.isFeatured }
    : { title: '', excerpt: '', content: '', status: 'Draft', isFeatured: false })
  showForm.value = true
}

async function handleSave() {
  saving.value = true
  try {
    const fd = new FormData()
    Object.entries(form).forEach(([k, v]) => fd.append(k, v))
    if (imageFile.value) fd.append('image', imageFile.value)

    if (editPost.value) {
      await blogApi.update(editPost.value.id, fd)
      toast.success('تم تحديث المقال')
    } else {
      await blogApi.create(fd)
      toast.success('تم نشر المقال')
    }
    showForm.value = false
    fetchPosts()
  } finally { saving.value = false }
}

async function handleDelete(id) {
  if (!confirm('حذف هذا المقال؟')) return
  await blogApi.delete(id)
  toast.success('تم الحذف')
  fetchPosts()
}
</script>
