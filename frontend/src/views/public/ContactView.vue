<template>
  <div class="pt-24 pb-20 px-4 max-w-6xl mx-auto">
    <div class="text-center mb-16">
      <div class="badge-gold mb-4">{{ cp.headerBadge }}</div>
      <h1 class="section-title">{{ cp.headerTitle }}</h1>
      <p class="section-subtitle">{{ cp.headerSubtitle }}</p>
    </div>

    <div class="grid lg:grid-cols-2 gap-16 items-start">
      <!-- Form -->
      <div class="card p-8">
        <h2 class="text-2xl font-bold text-fg mb-8">{{ cp.formCardTitle }}</h2>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm text-fg-mute mb-2">{{ cp.labelName }}</label>
              <input v-model="form.name" type="text" class="input-field" :placeholder="cp.placeholderName" required>
              <p v-if="errors.name" class="text-red-400 text-xs mt-1">{{ errors.name }}</p>
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">{{ cp.labelPhone }}</label>
              <input v-model="form.phone" type="tel" class="input-field" :placeholder="cp.placeholderPhone">
            </div>
          </div>

          <div>
            <label class="block text-sm text-fg-mute mb-2">{{ cp.labelEmail }}</label>
            <input v-model="form.email" type="email" class="input-field" :placeholder="cp.placeholderEmail" required>
          </div>

          <div>
            <label class="block text-sm text-fg-mute mb-2">{{ cp.labelSubject }}</label>
            <select v-model="form.subject" class="input-field" required>
              <option value="">{{ cp.subjectOptionPlaceholder }}</option>
              <option v-for="(opt, i) in cp.subjectOptions" :key="i" :value="opt">{{ opt }}</option>
            </select>
          </div>

          <div>
            <label class="block text-sm text-fg-mute mb-2">{{ cp.labelMessage }}</label>
            <textarea v-model="form.message" class="textarea-field" rows="5"
              :placeholder="cp.placeholderMessage" required minlength="10" maxlength="500"></textarea>
            <p class="text-fg-dim text-xs mt-1">{{ form.message.length }} / 500 حرف</p>
          </div>

          <button type="submit" :disabled="loading" class="btn-primary w-full justify-center py-4 text-lg">
            <span v-if="loading" class="inline-block w-5 h-5 border-2 border-line/40 border-t-gold rounded-full animate-spin ml-2"></span>
            {{ loading ? cp.submitSendingText : cp.submitButtonText }}
          </button>
        </form>
      </div>

      <!-- Contact info -->
      <div class="space-y-8">
        <div>
          <h2 class="text-2xl font-bold text-fg mb-6">{{ cp.infoSectionTitle }}</h2>
          <div class="space-y-6">
            <div v-for="(item, idx) in cp.infoRows" :key="idx" class="flex items-start gap-4">
              <div class="w-12 h-12 rounded-xl bg-gold/10 border border-gold/20 flex items-center justify-center text-xl flex-shrink-0">
                {{ item.icon }}
              </div>
              <div>
                <div class="text-fg-mute text-sm mb-1">{{ item.label }}</div>
                <div class="text-fg font-medium">{{ item.value }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Social -->
        <div>
          <h3 class="text-fg font-semibold mb-4">{{ cp.socialSectionTitle }}</h3>
          <div class="flex flex-wrap gap-3">
            <a
              v-for="(s, idx) in cp.socialLinks"
              :key="idx"
              :href="socialHref(s.url)"
              class="px-4 py-2 rounded-lg card hover:border-gold/40 transition-all text-sm text-fg-mute hover:text-gold"
              :target="isExternalUrl(s.url) ? '_blank' : '_self'"
              :rel="isExternalUrl(s.url) ? 'noopener noreferrer' : undefined"
            >
              {{ s.icon }} {{ s.name }}
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { contactApi } from '@/services/api'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import { toast } from 'vue3-toastify'

const site = useSiteSettingsStore()
const loading = ref(false)
const errors = ref({})
const form = reactive({ name: '', email: '', phone: '', subject: '', message: '' })

function fallbackContact() {
  return {
    headerBadge: '📬 تواصل معنا',
    headerTitle: 'نسعد بتواصلك',
    headerSubtitle: 'هل لديك استفسار أو مشروع تريد التعاون فيه؟ أرسل لنا رسالتك',
    formCardTitle: 'أرسل رسالة',
    labelName: 'الاسم الكامل *',
    labelPhone: 'رقم الجوال',
    labelEmail: 'البريد الإلكتروني *',
    labelSubject: 'الموضوع *',
    labelMessage: 'الرسالة *',
    placeholderName: 'محمد العمري',
    placeholderPhone: '+966 5x xxx xxxx',
    placeholderEmail: 'example@email.com',
    placeholderMessage: 'اكتب رسالتك هنا...',
    subjectOptionPlaceholder: 'اختر موضوع الرسالة',
    subjectOptions: ['استفسار عن الدورات', 'طلب تعاون', 'شراء عمل فني', 'أخرى'],
    submitButtonText: 'إرسال الرسالة 📨',
    submitSendingText: 'جارٍ الإرسال...',
    toastSuccessMessage: 'تم إرسال رسالتك بنجاح! سنتواصل معك قريباً 😊',
    infoSectionTitle: 'معلومات التواصل',
    infoRows: [
      { icon: '📧', label: 'البريد الإلكتروني', value: 'info@artplatform.com' },
      { icon: '📱', label: 'رقم الجوال', value: '+966 50 000 0000' },
      { icon: '🕐', label: 'ساعات العمل', value: 'السبت - الخميس، 9 ص - 6 م' },
      { icon: '📍', label: 'الموقع', value: 'الرياض، المملكة العربية السعودية' }
    ],
    socialSectionTitle: 'تابعني على منصات التواصل',
    socialLinks: [
      { name: 'Instagram', icon: '📷', url: 'https://instagram.com' },
      { name: 'Twitter', icon: '𝕏', url: 'https://twitter.com' },
      { name: 'YouTube', icon: '▶', url: 'https://youtube.com' }
    ]
  }
}

const cp = computed(() => {
  const c = site.data?.contactPage
  const f = fallbackContact()
  if (!c) return f
  return {
    ...f,
    ...c,
    subjectOptions: Array.isArray(c.subjectOptions) && c.subjectOptions.length ? c.subjectOptions : f.subjectOptions,
    infoRows: Array.isArray(c.infoRows) && c.infoRows.length ? c.infoRows : f.infoRows,
    socialLinks: Array.isArray(c.socialLinks) && c.socialLinks.length ? c.socialLinks : f.socialLinks
  }
})

function socialHref(url) {
  const u = String(url || '').trim()
  return u || '#'
}

function isExternalUrl(url) {
  return /^https?:\/\//i.test(String(url || '').trim())
}

onMounted(() => site.load())

async function handleSubmit() {
  errors.value = {}
  loading.value = true
  try {
    await contactApi.send(form)
    toast.success(cp.value.toastSuccessMessage)
    Object.assign(form, { name: '', email: '', phone: '', subject: '', message: '' })
  } catch (e) {
    if (e.response?.status === 422)
      errors.value = e.response.data.errors || {}
  } finally {
    loading.value = false
  }
}
</script>
