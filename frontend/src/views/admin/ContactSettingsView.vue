<template>
  <div>
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-white">إعدادات صفحة التواصل</h1>
      <p class="text-gray-400 mt-1">تحكم في المعلومات الظاهرة في صفحة تواصل معنا</p>
    </div>

    <div v-if="loading" class="space-y-4">
      <div v-for="i in 6" :key="i" class="card h-20 animate-pulse"></div>
    </div>

    <form v-else @submit.prevent="saveAll" class="space-y-6">
      <!-- معلومات التواصل الأساسية -->
      <div class="card p-6">
        <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
          <span class="text-gold">📋</span> معلومات التواصل
        </h2>
        <div class="grid md:grid-cols-2 gap-5">
          <div>
            <label class="block text-sm text-gray-400 mb-2">البريد الإلكتروني</label>
            <input v-model="fields.contact_email" type="email" class="input-field" placeholder="info@artplatform.com">
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">رقم الجوال</label>
            <input v-model="fields.contact_phone" type="text" class="input-field" placeholder="+966 50 000 0000" dir="ltr">
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">ساعات العمل</label>
            <input v-model="fields.contact_hours" type="text" class="input-field" placeholder="السبت - الخميس، 9 ص - 6 م">
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">الموقع الجغرافي</label>
            <input v-model="fields.contact_location" type="text" class="input-field" placeholder="الرياض، المملكة العربية السعودية">
          </div>
        </div>
      </div>

      <!-- روابط التواصل الاجتماعي -->
      <div class="card p-6">
        <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
          <span class="text-gold">🔗</span> روابط التواصل الاجتماعي
        </h2>
        <div class="space-y-4">
          <div>
            <label class="block text-sm text-gray-400 mb-2">📷 Instagram</label>
            <input v-model="fields.social_instagram" type="url" class="input-field" placeholder="https://instagram.com/..." dir="ltr">
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">𝕏 Twitter / X</label>
            <input v-model="fields.social_twitter" type="url" class="input-field" placeholder="https://x.com/..." dir="ltr">
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">▶ YouTube</label>
            <input v-model="fields.social_youtube" type="url" class="input-field" placeholder="https://youtube.com/..." dir="ltr">
          </div>
        </div>
      </div>

      <!-- Preview -->
      <div class="card p-6 bg-dark-100/50">
        <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
          <span class="text-gold">👁️</span> معاينة
        </h2>
        <div class="space-y-4">
          <div v-for="item in previewItems" :key="item.label" class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gold/10 border border-gold/20 flex items-center justify-center text-lg flex-shrink-0">
              {{ item.icon }}
            </div>
            <div>
              <div class="text-gray-500 text-xs">{{ item.label }}</div>
              <div class="text-white text-sm font-medium">{{ item.value || '—' }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="flex justify-end">
        <button type="submit" :disabled="saving" class="btn-primary px-8 py-3">
          <span v-if="saving" class="inline-block w-4 h-4 border-2 border-dark/30 border-t-dark rounded-full animate-spin ml-2"></span>
          {{ saving ? 'جارٍ الحفظ...' : 'حفظ جميع الإعدادات' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { toast } from 'vue3-toastify'
import api from '@/services/api'
import { invalidateSettingsCache } from '@/composables/useSiteSettings'

const loading = ref(false)
const saving = ref(false)

const fields = reactive({
  contact_email: '',
  contact_phone: '',
  contact_hours: '',
  contact_location: '',
  social_instagram: '',
  social_twitter: '',
  social_youtube: '',
})

const previewItems = computed(() => [
  { icon: '📧', label: 'البريد الإلكتروني', value: fields.contact_email },
  { icon: '📱', label: 'رقم الجوال', value: fields.contact_phone },
  { icon: '🕐', label: 'ساعات العمل', value: fields.contact_hours },
  { icon: '📍', label: 'الموقع', value: fields.contact_location },
  { icon: '📷', label: 'Instagram', value: fields.social_instagram },
  { icon: '𝕏', label: 'Twitter / X', value: fields.social_twitter },
  { icon: '▶', label: 'YouTube', value: fields.social_youtube },
])

onMounted(async () => {
  loading.value = true
  try {
    const res = await api.get('/settings')
    const data = res.data ?? {}
    Object.keys(fields).forEach(k => { if (data[k] !== undefined) fields[k] = data[k] })
  } catch { } finally { loading.value = false }
})

async function saveAll() {
  saving.value = true
  try {
    await api.put('/settings/bulk', { ...fields })
    invalidateSettingsCache()
    toast.success('تم حفظ جميع الإعدادات بنجاح')
  } catch {
    toast.error('فشل الحفظ، تأكد من تشغيل الباك إيند')
  } finally { saving.value = false }
}
</script>
