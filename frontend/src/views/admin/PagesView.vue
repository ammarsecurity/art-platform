<template>
  <div class="space-y-8">
    <h1 class="text-2xl font-bold text-fg">إدارة الصفحات القانونية</h1>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Terms of Use -->
      <div class="card p-6 space-y-4">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-fg">شروط الاستخدام</h2>
          <a href="/terms" target="_blank" class="text-gold text-sm hover:text-gold-light">معاينة ←</a>
        </div>
        <textarea
          v-model="terms"
          class="input-field resize-none font-mono text-sm"
          rows="20"
          placeholder="اكتب شروط الاستخدام بصيغة Markdown..."
          dir="rtl"
        ></textarea>
        <button @click="save('terms_of_use', terms)" :disabled="saving.terms" class="btn-primary w-full justify-center">
          <span v-if="saving.terms" class="inline-block w-4 h-4 border-2 border-line/40 border-t-gold rounded-full animate-spin ml-2"></span>
          {{ saving.terms ? 'جارٍ الحفظ...' : 'حفظ شروط الاستخدام' }}
        </button>
      </div>

      <!-- Privacy Policy -->
      <div class="card p-6 space-y-4">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-fg">سياسة الخصوصية</h2>
          <a href="/privacy" target="_blank" class="text-gold text-sm hover:text-gold-light">معاينة ←</a>
        </div>
        <textarea
          v-model="privacy"
          class="input-field resize-none font-mono text-sm"
          rows="20"
          placeholder="اكتب سياسة الخصوصية بصيغة Markdown..."
          dir="rtl"
        ></textarea>
        <button @click="save('privacy_policy', privacy)" :disabled="saving.privacy" class="btn-primary w-full justify-center">
          <span v-if="saving.privacy" class="inline-block w-4 h-4 border-2 border-line/40 border-t-gold rounded-full animate-spin ml-2"></span>
          {{ saving.privacy ? 'جارٍ الحفظ...' : 'حفظ سياسة الخصوصية' }}
        </button>
      </div>
    </div>

    <!-- Markdown Guide -->
    <div class="card p-5 bg-surface/50">
      <h3 class="text-sm font-semibold text-gold mb-3">دليل Markdown</h3>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-3 text-xs text-fg-mute font-mono">
        <span><span class="text-fg"># عنوان</span> → عنوان رئيسي</span>
        <span><span class="text-fg">## عنوان</span> → عنوان ثانوي</span>
        <span><span class="text-fg">**نص**</span> → نص عريض</span>
        <span><span class="text-fg">سطر فارغ</span> → فقرة جديدة</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { toast } from 'vue3-toastify'
import api from '@/services/api'

const terms = ref('')
const privacy = ref('')
const saving = ref({ terms: false, privacy: false })

onMounted(async () => {
  try {
    const [t, p] = await Promise.all([
      api.get('/pages/terms_of_use'),
      api.get('/pages/privacy_policy'),
    ])
    terms.value = t.data.content
    privacy.value = p.data.content
  } catch {
    toast.error('تعذّر تحميل الصفحات')
  }
})

async function save(key, content) {
  const field = key === 'terms_of_use' ? 'terms' : 'privacy'
  saving.value[field] = true
  try {
    await api.put(`/pages/${key}`, { content })
    toast.success('تم الحفظ بنجاح')
  } catch {
    toast.error('فشل الحفظ')
  } finally {
    saving.value[field] = false
  }
}
</script>
