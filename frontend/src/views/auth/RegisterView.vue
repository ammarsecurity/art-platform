<template>
  <div class="min-h-screen bg-dark flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <div class="text-center mb-10">
        <RouterLink to="/" class="inline-flex items-center gap-3 mb-6">
          <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-gold to-gold-dark flex items-center justify-center shadow-lg shadow-gold/30">
            <span class="text-dark font-bold text-xl">ف</span>
          </div>
          <span class="text-2xl font-bold text-white">منصة مرتضى ثامر</span>
        </RouterLink>
        <h1 class="text-3xl font-bold text-white">إنشاء حساب جديد</h1>
        <p class="text-gray-400 mt-2">انضم إلى مجتمع الفنانين</p>
      </div>

      <div class="card p-8">
        <form @submit.prevent="handleRegister" class="space-y-5">
          <div>
            <label class="block text-sm text-gray-400 mb-2">الاسم الكامل *</label>
            <input v-model="form.name" type="text" class="input-field" placeholder="اسمك الكامل" required minlength="2">
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">رقم الهاتف *</label>
            <input v-model="form.phone" type="tel" class="input-field" placeholder="078xxxxxxxx" required dir="ltr">
          </div>
          <div>
            <label class="block text-sm text-gray-400 mb-2">كلمة المرور *</label>
            <input v-model="form.password" type="password" class="input-field" placeholder="8 أحرف على الأقل" required minlength="8">
            <!-- Password strength -->
            <div class="flex gap-1 mt-2">
              <div v-for="i in 4" :key="i" class="h-1 flex-1 rounded-full transition-colors"
                :class="passwordStrength >= i ? strengthColor : 'bg-dark-300'"></div>
            </div>
            <p class="text-xs mt-1" :class="strengthText.class">{{ strengthText.label }}</p>
          </div>

          <label class="flex items-start gap-3 cursor-pointer">
            <input v-model="agreed" type="checkbox" class="mt-1 accent-gold" required>
            <span class="text-sm text-gray-400">
              أوافق على <a href="#" class="text-gold">شروط الاستخدام</a> و
              <a href="#" class="text-gold">سياسة الخصوصية</a>
            </span>
          </label>

          <button type="submit" :disabled="auth.loading || !agreed" class="btn-primary w-full justify-center py-4 text-lg">
            <span v-if="auth.loading" class="inline-block w-5 h-5 border-2 border-dark/30 border-t-dark rounded-full animate-spin ml-2"></span>
            {{ auth.loading ? 'جارٍ الإنشاء...' : 'إنشاء الحساب 🎨' }}
          </button>
        </form>

        <p class="text-center text-gray-400 mt-6 text-sm">
          لديك حساب بالفعل؟
          <RouterLink to="/login" class="text-gold hover:text-gold-light font-medium">تسجيل الدخول</RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const agreed = ref(false)
const form = reactive({ name: '', phone: '', password: '' })

const passwordStrength = computed(() => {
  const p = form.password
  let s = 0
  if (p.length >= 8) s++
  if (/[A-Z]/.test(p)) s++
  if (/[0-9]/.test(p)) s++
  if (/[^A-Za-z0-9]/.test(p)) s++
  return s
})

const strengthColor = computed(() => ['bg-red-500', 'bg-orange-500', 'bg-yellow-500', 'bg-green-500'][passwordStrength.value - 1] || 'bg-red-500')

const strengthText = computed(() => ({
  0: { label: '', class: '' },
  1: { label: 'ضعيفة جداً', class: 'text-red-400' },
  2: { label: 'ضعيفة', class: 'text-orange-400' },
  3: { label: 'متوسطة', class: 'text-yellow-400' },
  4: { label: 'قوية', class: 'text-green-400' },
}[passwordStrength.value]))

async function handleRegister() {
  await auth.register(form)
}
</script>
