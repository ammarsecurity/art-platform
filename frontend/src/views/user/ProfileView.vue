<template>
  <div class="min-h-screen py-24 px-4">
    <div class="max-w-2xl mx-auto space-y-6">

      <!-- Header -->
      <div class="flex items-center gap-4 mb-8">
        <div class="w-16 h-16 rounded-2xl bg-gradient-to-br from-gold to-gold-dark flex items-center justify-center text-dark text-2xl font-bold shadow-lg shadow-gold/20">
          {{ auth.user?.name?.charAt(0) }}
        </div>
        <div>
          <h1 class="text-2xl font-bold text-white">{{ auth.user?.name }}</h1>
          <p class="text-gray-400 text-sm">{{ auth.user?.phone }}</p>
        </div>
      </div>

      <!-- Profile form -->
      <div class="card p-6 space-y-5">
        <h2 class="text-lg font-semibold text-white border-b border-dark-300 pb-3">البيانات الشخصية</h2>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-5">
          <div>
            <label class="block text-sm text-gray-400 mb-2">الاسم الكامل *</label>
            <input v-model="form.name" type="text" class="input-field" placeholder="اسمك الكامل" required>
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">رقم الهاتف</label>
            <input v-model="form.phone" type="tel" class="input-field" placeholder="078xxxxxxxx" dir="ltr">
          </div>
        </div>

        <div>
          <label class="block text-sm text-gray-400 mb-2">نبذة عنك</label>
          <textarea v-model="form.bio" class="textarea-field" rows="3"
            placeholder="اكتب نبذة مختصرة عن نفسك..."></textarea>
        </div>

        <button @click="saveProfile" :disabled="saving" class="btn-primary">
          <span v-if="saving" class="inline-block w-4 h-4 border-2 border-dark/30 border-t-dark rounded-full animate-spin ml-2"></span>
          {{ saving ? 'جارٍ الحفظ...' : 'حفظ التغييرات' }}
        </button>
      </div>

      <!-- Change password -->
      <div class="card p-6 space-y-5">
        <h2 class="text-lg font-semibold text-white border-b border-dark-300 pb-3">تغيير كلمة المرور</h2>

        <div>
          <label class="block text-sm text-gray-400 mb-2">كلمة المرور الحالية</label>
          <input v-model="passwords.current" type="password" class="input-field" placeholder="••••••••">
        </div>
        <div>
          <label class="block text-sm text-gray-400 mb-2">كلمة المرور الجديدة</label>
          <input v-model="passwords.new" type="password" class="input-field" placeholder="8 أحرف على الأقل">
          <div class="flex gap-1 mt-2">
            <div v-for="i in 4" :key="i" class="h-1 flex-1 rounded-full transition-colors"
              :class="passwordStrength >= i ? strengthColor : 'bg-dark-300'"></div>
          </div>
        </div>
        <div>
          <label class="block text-sm text-gray-400 mb-2">تأكيد كلمة المرور الجديدة</label>
          <input v-model="passwords.confirm" type="password" class="input-field" placeholder="••••••••">
          <p v-if="passwords.new && passwords.confirm && passwords.new !== passwords.confirm"
            class="text-red-400 text-xs mt-1">كلمتا المرور غير متطابقتين</p>
        </div>

        <button @click="savePassword" :disabled="savingPass || !canSavePass" class="btn-outline">
          <span v-if="savingPass" class="inline-block w-4 h-4 border-2 border-gold/30 border-t-gold rounded-full animate-spin ml-2"></span>
          {{ savingPass ? 'جارٍ التغيير...' : 'تغيير كلمة المرور' }}
        </button>
      </div>

      <!-- Account info -->
      <div class="card p-6">
        <h2 class="text-lg font-semibold text-white border-b border-dark-300 pb-3 mb-4">معلومات الحساب</h2>
        <div class="space-y-3 text-sm">
          <div class="flex justify-between">
            <span class="text-gray-500">نوع الحساب</span>
            <span class="badge-gold">{{ auth.user?.role === 'Admin' ? 'مدير' : 'طالب' }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-gray-500">تاريخ الانضمام</span>
            <span class="text-gray-300">{{ joinDate }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-gray-500">دوراتي</span>
            <RouterLink to="/my-courses" class="text-gold hover:text-gold-light text-sm">
              عرض دوراتي ←
            </RouterLink>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { authApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const auth = useAuthStore()
const saving = ref(false)
const savingPass = ref(false)

const form = reactive({ name: '', phone: '', bio: '' })
const passwords = reactive({ current: '', new: '', confirm: '' })

onMounted(() => {
  form.name = auth.user?.name || ''
  form.phone = auth.user?.phone || ''
  form.bio = auth.user?.bio || ''
})

const joinDate = computed(() => {
  if (!auth.user?.createdAt) return '—'
  return new Date(auth.user.createdAt).toLocaleDateString('ar-SA', { year: 'numeric', month: 'long', day: 'numeric' })
})

const passwordStrength = computed(() => {
  const p = passwords.new
  let s = 0
  if (p.length >= 8) s++
  if (/[A-Z]/.test(p)) s++
  if (/[0-9]/.test(p)) s++
  if (/[^A-Za-z0-9]/.test(p)) s++
  return s
})

const strengthColor = computed(() =>
  ['bg-red-500', 'bg-orange-500', 'bg-yellow-500', 'bg-green-500'][passwordStrength.value - 1] || 'bg-red-500'
)

const canSavePass = computed(() =>
  passwords.current && passwords.new && passwords.confirm &&
  passwords.new === passwords.confirm && passwords.new.length >= 8
)

async function saveProfile() {
  saving.value = true
  try {
    const res = await authApi.updateProfile({ name: form.name, phone: form.phone, bio: form.bio })
    auth.user = res.data
    toast.success('تم تحديث الملف الشخصي بنجاح')
  } catch (e) {
    toast.error(e.response?.data?.message || 'فشل تحديث البيانات')
  } finally {
    saving.value = false
  }
}

async function savePassword() {
  savingPass.value = true
  try {
    await authApi.updateProfile({
      name: form.name,
      phone: form.phone,
      bio: form.bio,
      currentPassword: passwords.current,
      newPassword: passwords.new
    })
    passwords.current = ''
    passwords.new = ''
    passwords.confirm = ''
    toast.success('تم تغيير كلمة المرور بنجاح')
  } catch (e) {
    toast.error(e.response?.data?.message || 'فشل تغيير كلمة المرور')
  } finally {
    savingPass.value = false
  }
}
</script>
