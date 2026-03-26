<template>
  <div>
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-white">آراء الطلاب</h1>
        <p class="text-gray-400 mt-1">إدارة التقييمات الظاهرة في الصفحة الرئيسية</p>
      </div>
    </div>

    <div class="grid lg:grid-cols-5 gap-8">
      <!-- List -->
      <div class="lg:col-span-3 space-y-3">
        <div v-if="loading" class="space-y-3">
          <div v-for="i in 3" :key="i" class="card h-28 animate-pulse"></div>
        </div>

        <div v-else-if="items.length === 0" class="card p-10 text-center text-gray-500">
          <div class="text-4xl mb-3">💬</div>
          <p>لا توجد تقييمات بعد.</p>
        </div>

        <div v-for="item in items" :key="item.id"
          class="card p-5 group"
          :class="!item.isActive ? 'opacity-50' : ''">
          <div class="flex items-start gap-4">
            <div class="w-10 h-10 rounded-full bg-gold/20 text-gold flex items-center justify-center font-bold shrink-0 text-sm">
              {{ item.name.charAt(0) }}
            </div>
            <div class="flex-1 min-w-0">
              <div class="flex items-center gap-2 mb-1">
                <span class="text-white font-semibold text-sm">{{ item.name }}</span>
                <span class="text-gray-500 text-xs">— {{ item.title }}</span>
                <span class="text-gold text-xs mr-auto">{{ '★'.repeat(item.rating) }}</span>
              </div>
              <p class="text-gray-400 text-sm leading-relaxed line-clamp-2">"{{ item.text }}"</p>
            </div>
          </div>
          <div class="flex items-center gap-2 mt-4 opacity-0 group-hover:opacity-100 transition-opacity">
            <button @click="editItem(item)"
              class="px-3 py-1.5 text-xs bg-dark-300 hover:bg-dark-200 text-white rounded-lg transition-colors">تعديل</button>
            <button @click="handleToggle(item)"
              class="px-3 py-1.5 text-xs rounded-lg transition-colors"
              :class="item.isActive ? 'bg-yellow-500/20 text-yellow-400 hover:bg-yellow-500/30' : 'bg-green-500/20 text-green-400 hover:bg-green-500/30'">
              {{ item.isActive ? 'إخفاء' : 'إظهار' }}
            </button>
            <button @click="handleDelete(item.id)"
              class="px-3 py-1.5 text-xs bg-red-500/20 hover:bg-red-500/30 text-red-400 rounded-lg transition-colors">حذف</button>
          </div>
        </div>
      </div>

      <!-- Form -->
      <div class="lg:col-span-2">
        <div class="card p-6 sticky top-6">
          <h2 class="text-white font-bold text-lg mb-5">
            {{ editing ? 'تعديل التقييم' : 'إضافة تقييم جديد' }}
          </h2>
          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">اسم الطالب *</label>
              <input v-model="form.name" type="text" class="input-field" required placeholder="مثال: أحمد الكندي">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">المسمى / التخصص *</label>
              <input v-model="form.title" type="text" class="input-field" required placeholder="مثال: طالب فنون بصرية">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">نص التقييم *</label>
              <textarea v-model="form.text" class="textarea-field" rows="4" required
                placeholder="ما قاله الطالب عن المنصة..."></textarea>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm text-gray-400 mb-2">التقييم (نجوم)</label>
                <select v-model.number="form.rating" class="input-field">
                  <option :value="5">★★★★★ (5)</option>
                  <option :value="4">★★★★ (4)</option>
                  <option :value="3">★★★ (3)</option>
                  <option :value="2">★★ (2)</option>
                  <option :value="1">★ (1)</option>
                </select>
              </div>
              <div>
                <label class="block text-sm text-gray-400 mb-2">الترتيب</label>
                <input v-model.number="form.sortOrder" type="number" class="input-field" min="0" placeholder="0">
              </div>
            </div>
            <label class="flex items-center gap-3 cursor-pointer">
              <input v-model="form.isActive" type="checkbox" class="accent-gold">
              <span class="text-sm text-gray-400">ظاهر في الصفحة الرئيسية</span>
            </label>
            <div class="flex gap-3 pt-2">
              <button type="submit" :disabled="saving" class="btn-primary flex-1 justify-center py-3">
                {{ saving ? 'جارٍ الحفظ...' : (editing ? 'حفظ التعديلات' : 'إضافة التقييم') }}
              </button>
              <button v-if="editing" type="button" @click="cancelEdit"
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
import { testimonialApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const items = ref([])
const loading = ref(false)
const saving = ref(false)
const editing = ref(null)

const defaultForm = { name: '', title: '', text: '', rating: 5, sortOrder: 0, isActive: true }
const form = reactive({ ...defaultForm })

onMounted(fetchAll)

async function fetchAll() {
  loading.value = true
  try {
    const res = await testimonialApi.getAll()
    items.value = res.data || []
  } catch { } finally { loading.value = false }
}

async function handleSubmit() {
  saving.value = true
  try {
    if (editing.value) {
      await testimonialApi.update(editing.value.id, { ...form })
      toast.success('تم تحديث التقييم')
    } else {
      await testimonialApi.create({ ...form })
      toast.success('تمت إضافة التقييم')
    }
    cancelEdit()
    fetchAll()
  } finally { saving.value = false }
}

function editItem(item) {
  editing.value = item
  Object.assign(form, {
    name: item.name, title: item.title, text: item.text,
    rating: item.rating, sortOrder: item.sortOrder, isActive: item.isActive,
  })
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function cancelEdit() {
  editing.value = null
  Object.assign(form, { ...defaultForm })
}

async function handleToggle(item) {
  const res = await testimonialApi.toggle(item.id)
  item.isActive = res.data?.isActive ?? !item.isActive
  toast.success(res.message || 'تم التحديث')
}

async function handleDelete(id) {
  if (!confirm('حذف هذا التقييم؟')) return
  await testimonialApi.delete(id)
  toast.success('تم الحذف')
  fetchAll()
}
</script>
