<template>
  <div>
    <div class="flex items-center justify-between mb-8">
      <h1 class="text-3xl font-bold text-white">التصنيفات</h1>
      <button @click="openModal()" class="btn-primary">+ تصنيف جديد</button>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div v-for="cat in categories" :key="cat.id"
        class="card p-6 flex items-center justify-between group">
        <div class="flex items-center gap-4">
          <div class="w-12 h-12 rounded-xl bg-gold/10 border border-gold/20 flex items-center justify-center text-2xl">
            🏷️
          </div>
          <div>
            <h3 class="text-white font-semibold">{{ cat.name }}</h3>
            <div class="flex gap-3 text-xs text-gray-500 mt-1">
              <span>{{ cat.artworkCount }} عمل</span>
              <span>{{ cat.courseCount }} دورة</span>
            </div>
          </div>
        </div>
        <div class="flex items-center gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
          <button @click="openModal(cat)" class="p-2 text-gray-400 hover:text-white transition-colors">✏️</button>
          <button @click="handleDelete(cat.id)" class="p-2 text-gray-400 hover:text-red-400 transition-colors">🗑️</button>
        </div>
      </div>
    </div>

    <!-- Modal -->
    <Teleport to="body">
      <div v-if="showModal" class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4" @click.self="showModal = false">
        <div class="card p-8 max-w-md w-full">
          <h3 class="text-xl font-bold text-white mb-6">{{ editTarget ? 'تعديل التصنيف' : 'تصنيف جديد' }}</h3>
          <form @submit.prevent="handleSave" class="space-y-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">اسم التصنيف *</label>
              <input v-model="form.name" type="text" class="input-field" placeholder="مثال: لوحات زيتية" required>
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">الوصف</label>
              <textarea v-model="form.description" class="textarea-field" rows="3"></textarea>
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">ترتيب العرض</label>
              <input v-model.number="form.sortOrder" type="number" class="input-field" min="0">
            </div>
            <div class="flex gap-3 pt-2">
              <button type="button" @click="showModal = false" class="btn-ghost flex-1 justify-center border border-dark-300 rounded-lg py-3">إلغاء</button>
              <button type="submit" :disabled="saving" class="btn-primary flex-1 justify-center">
                {{ saving ? 'جارٍ الحفظ...' : 'حفظ' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { categoryApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const categories = ref([])
const showModal = ref(false)
const editTarget = ref(null)
const saving = ref(false)
const form = reactive({ name: '', description: '', sortOrder: 0 })

onMounted(fetchCategories)

async function fetchCategories() {
  const res = await categoryApi.getAll()
  categories.value = res.data
}

function openModal(cat = null) {
  editTarget.value = cat
  Object.assign(form, cat ? { name: cat.name, description: cat.description || '', sortOrder: cat.sortOrder } : { name: '', description: '', sortOrder: 0 })
  showModal.value = true
}

async function handleSave() {
  saving.value = true
  try {
    if (editTarget.value) {
      await categoryApi.update(editTarget.value.id, form)
      toast.success('تم تحديث التصنيف')
    } else {
      await categoryApi.create(form)
      toast.success('تم إنشاء التصنيف')
    }
    showModal.value = false
    fetchCategories()
  } finally { saving.value = false }
}

async function handleDelete(id) {
  if (!confirm('هل أنت متأكد من حذف هذا التصنيف؟')) return
  await categoryApi.delete(id)
  toast.success('تم حذف التصنيف')
  fetchCategories()
}
</script>
