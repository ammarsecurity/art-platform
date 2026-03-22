<template>
  <div class="pt-24 pb-20 px-4 max-w-6xl mx-auto">
    <div class="text-center mb-16">
      <div class="badge-gold mb-4">📬 تواصل معنا</div>
      <h1 class="section-title">نسعد بتواصلك</h1>
      <p class="section-subtitle">هل لديك استفسار أو مشروع تريد التعاون فيه؟ أرسل لنا رسالتك</p>
    </div>

    <div class="grid lg:grid-cols-2 gap-16 items-start">
      <!-- Form -->
      <div class="card p-8">
        <h2 class="text-2xl font-bold text-white mb-8">أرسل رسالة</h2>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">الاسم الكامل *</label>
              <input v-model="form.name" type="text" class="input-field" placeholder="محمد العمري" required>
              <p v-if="errors.name" class="text-red-400 text-xs mt-1">{{ errors.name }}</p>
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">رقم الجوال</label>
              <input v-model="form.phone" type="tel" class="input-field" placeholder="+966 5x xxx xxxx">
            </div>
          </div>

          <div>
            <label class="block text-sm text-gray-400 mb-2">البريد الإلكتروني *</label>
            <input v-model="form.email" type="email" class="input-field" placeholder="example@email.com" required>
          </div>

          <div>
            <label class="block text-sm text-gray-400 mb-2">الموضوع *</label>
            <select v-model="form.subject" class="input-field" required>
              <option value="">اختر موضوع الرسالة</option>
              <option value="استفسار عن الدورات">استفسار عن الدورات</option>
              <option value="طلب تعاون">طلب تعاون</option>
              <option value="شراء عمل فني">شراء عمل فني</option>
              <option value="أخرى">أخرى</option>
            </select>
          </div>

          <div>
            <label class="block text-sm text-gray-400 mb-2">الرسالة *</label>
            <textarea v-model="form.message" class="textarea-field" rows="5"
              placeholder="اكتب رسالتك هنا..." required minlength="10"></textarea>
            <p class="text-gray-500 text-xs mt-1">{{ form.message.length }} / 500 حرف</p>
          </div>

          <button type="submit" :disabled="loading" class="btn-primary w-full justify-center py-4 text-lg">
            <span v-if="loading" class="inline-block w-5 h-5 border-2 border-dark/30 border-t-dark rounded-full animate-spin ml-2"></span>
            {{ loading ? 'جارٍ الإرسال...' : 'إرسال الرسالة 📨' }}
          </button>
        </form>
      </div>

      <!-- Contact info -->
      <div class="space-y-8">
        <div>
          <h2 class="text-2xl font-bold text-white mb-6">معلومات التواصل</h2>
          <div class="space-y-6">
            <div v-for="item in contactItems" :key="item.label" class="flex items-start gap-4">
              <div class="w-12 h-12 rounded-xl bg-gold/10 border border-gold/20 flex items-center justify-center text-xl flex-shrink-0">
                {{ item.icon }}
              </div>
              <div>
                <div class="text-gray-400 text-sm mb-1">{{ item.label }}</div>
                <div class="text-white font-medium">{{ item.value }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Map placeholder -->
        <div class="rounded-2xl overflow-hidden bg-dark-200 border border-dark-300 h-64 flex items-center justify-center">
          <div class="text-center text-gray-500">
            <div class="text-4xl mb-2">🗺️</div>
            <p>الرياض، المملكة العربية السعودية</p>
          </div>
        </div>

        <!-- Social -->
        <div>
          <h3 class="text-white font-semibold mb-4">تابعني على منصات التواصل</h3>
          <div class="flex gap-3">
            <a v-for="s in socials" :key="s.name" href="#"
              class="px-4 py-2 rounded-lg card hover:border-gold/40 transition-all text-sm text-gray-400 hover:text-gold">
              {{ s.icon }} {{ s.name }}
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { contactApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const loading = ref(false)
const errors = ref({})
const form = reactive({ name: '', email: '', phone: '', subject: '', message: '' })

const contactItems = [
  { icon: '📧', label: 'البريد الإلكتروني', value: 'info@artplatform.com' },
  { icon: '📱', label: 'رقم الجوال', value: '+966 50 000 0000' },
  { icon: '🕐', label: 'ساعات العمل', value: 'السبت - الخميس، 9 ص - 6 م' },
  { icon: '📍', label: 'الموقع', value: 'الرياض، المملكة العربية السعودية' },
]

const socials = [
  { name: 'Instagram', icon: '📷' },
  { name: 'Twitter', icon: '𝕏' },
  { name: 'YouTube', icon: '▶' },
]

async function handleSubmit() {
  errors.value = {}
  loading.value = true
  try {
    await contactApi.send(form)
    toast.success('تم إرسال رسالتك بنجاح! سنتواصل معك قريباً 😊')
    Object.assign(form, { name: '', email: '', phone: '', subject: '', message: '' })
  } catch (e) {
    if (e.response?.status === 422)
      errors.value = e.response.data.errors || {}
  } finally {
    loading.value = false
  }
}
</script>
