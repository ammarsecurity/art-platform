<template>
  <div>
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-white">الأعمال الفنية</h1>
        <p class="text-gray-400 mt-1">إدارة معرض الأعمال الفنية</p>
      </div>
      <RouterLink to="/admin/artworks/new" class="btn-primary">+ إضافة عمل جديد</RouterLink>
    </div>

    <!-- Filters -->
    <div class="card p-4 mb-6 flex flex-wrap gap-4">
      <input v-model="search" @input="debouncedFetch" type="text" placeholder="بحث..."
        class="input-field py-2 text-sm w-64">
      <select v-model="statusFilter" @change="fetchArtworks()" class="input-field py-2 text-sm w-40">
        <option value="">الحالة: الكل</option>
        <option value="Published">منشور</option>
        <option value="Draft">مسودة</option>
        <option value="Archived">مؤرشف</option>
      </select>
      <select v-model="categoryFilter" @change="fetchArtworks()" class="input-field py-2 text-sm w-48">
        <option value="">التصنيف: الكل</option>
        <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
      </select>
    </div>

    <!-- Table -->
    <div class="card overflow-hidden">
      <div v-if="loading" class="p-8 text-center text-gray-500">
        <div class="w-8 h-8 border-2 border-gold/30 border-t-gold rounded-full animate-spin mx-auto mb-3"></div>
        جارٍ التحميل...
      </div>

      <table v-else class="w-full">
        <thead>
          <tr class="border-b border-dark-300 text-right">
            <th v-for="h in headers" :key="h" class="px-4 py-3 text-xs text-gray-400 font-medium">{{ h }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-dark-300">
          <tr v-for="artwork in artworks" :key="artwork.id"
            class="hover:bg-dark-200/50 transition-colors group">
            <td class="px-4 py-3">
              <div class="flex items-center gap-3">
                <img :src="artwork.thumbnailUrl || artwork.imageUrl" :alt="artwork.title"
                  class="w-12 h-12 rounded-xl object-cover">
                <div>
                  <p class="text-white font-medium text-sm">{{ artwork.title }}</p>
                  <p class="text-gray-500 text-xs">{{ artwork.medium }}</p>
                </div>
              </div>
            </td>
            <td class="px-4 py-3 text-gray-400 text-sm">{{ artwork.categoryName }}</td>
            <td class="px-4 py-3">
              <span class="badge text-xs" :class="statusClass(artwork.status)">{{ statusLabel(artwork.status) }}</span>
            </td>
            <td class="px-4 py-3">
              <span v-if="artwork.isFeatured" class="badge-gold text-xs">⭐ مميز</span>
            </td>
            <td class="px-4 py-3 text-gray-400 text-xs">{{ formatDate(artwork.createdAt) }}</td>
            <td class="px-4 py-3">
              <div class="flex items-center gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                <RouterLink :to="`/admin/artworks/${artwork.id}/edit`"
                  class="px-3 py-1 text-xs bg-dark-300 hover:bg-dark-200 text-white rounded-lg transition-colors">تعديل</RouterLink>
                <button @click="confirmDelete(artwork)"
                  class="px-3 py-1 text-xs bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-lg transition-colors">حذف</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="!loading && !artworks.length" class="text-center py-16 text-gray-500">
        <div class="text-5xl mb-3">🖼️</div>
        <p>لا توجد أعمال فنية</p>
      </div>
    </div>

    <!-- Pagination -->
    <Pagination v-if="totalPages > 1" :current="page" :total="totalPages" @change="changePage" />

    <!-- Delete Confirm Modal -->
    <Teleport to="body">
      <div v-if="deleteTarget" class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4" @click.self="deleteTarget = null">
        <div class="card p-8 max-w-md w-full">
          <h3 class="text-xl font-bold text-white mb-3">تأكيد الحذف</h3>
          <p class="text-gray-400 mb-6">هل أنت متأكد من حذف <span class="text-white font-medium">{{ deleteTarget.title }}</span>؟ لا يمكن التراجع عن هذا الإجراء.</p>
          <div class="flex gap-3">
            <button @click="deleteTarget = null" class="btn-ghost flex-1 justify-center border border-dark-300 rounded-lg py-3">إلغاء</button>
            <button @click="handleDelete" :disabled="deleting"
              class="flex-1 py-3 bg-red-500/90 hover:bg-red-500 text-white rounded-lg font-semibold transition-colors">
              {{ deleting ? 'جارٍ الحذف...' : 'حذف نهائياً' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { artworkApi, categoryApi } from '@/services/api'
import Pagination from '@/components/ui/Pagination.vue'
import { toast } from 'vue3-toastify'

const artworks = ref([])
const categories = ref([])
const loading = ref(false)
const page = ref(1)
const totalPages = ref(1)
const search = ref('')
const statusFilter = ref('')
const categoryFilter = ref('')
const deleteTarget = ref(null)
const deleting = ref(false)
let timer = null

const headers = ['العمل الفني', 'التصنيف', 'الحالة', 'مميز', 'التاريخ', 'الإجراءات']

onMounted(() => {
  fetchArtworks()
  categoryApi.getAll().then(r => categories.value = r.data)
})

async function fetchArtworks() {
  loading.value = true
  try {
    const res = await artworkApi.getAll({
      page: page.value, pageSize: 15,
      search: search.value,
      status: statusFilter.value || undefined,
      categoryId: categoryFilter.value || undefined,
    })
    artworks.value = res.data.items
    totalPages.value = res.data.totalPages
  } finally { loading.value = false }
}

function debouncedFetch() {
  clearTimeout(timer)
  timer = setTimeout(fetchArtworks, 400)
}

function changePage(p) { page.value = p; fetchArtworks() }

function confirmDelete(artwork) { deleteTarget.value = artwork }

async function handleDelete() {
  if (!deleteTarget.value) return
  deleting.value = true
  try {
    await artworkApi.delete(deleteTarget.value.id)
    toast.success('تم حذف العمل الفني')
    deleteTarget.value = null
    fetchArtworks()
  } finally { deleting.value = false }
}

const statusClass = s => ({
  Published: 'bg-green-500/20 text-green-400 border border-green-500/30',
  Draft: 'bg-yellow-500/20 text-yellow-400 border border-yellow-500/30',
  Archived: 'bg-gray-500/20 text-gray-400 border border-gray-500/30',
}[s] || '')

const statusLabel = s => ({ Published: 'منشور', Draft: 'مسودة', Archived: 'مؤرشف' }[s] || s)

function formatDate(d) {
  return new Date(d).toLocaleDateString('ar-SA', { year: 'numeric', month: 'short', day: 'numeric' })
}
</script>
