<template>
  <div class="min-h-screen bg-dark flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <!-- Logo -->
      <div class="text-center mb-10">
        <RouterLink to="/" class="inline-flex items-center gap-3 group mb-6">
          <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-gold to-gold-dark flex items-center justify-center shadow-lg shadow-gold/30">
            <span class="text-dark font-bold text-xl">ف</span>
          </div>
          <span class="text-2xl font-bold text-white">منصة مرتضى ثامر</span>
        </RouterLink>
        <h1 class="text-3xl font-bold text-white">مرحباً بعودتك</h1>
        <p class="text-gray-400 mt-2">سجّل دخولك للمتابعة</p>
      </div>

      <div class="card p-8">
        <form @submit.prevent="handleLogin" class="space-y-5">
          <div>
            <label class="block text-sm text-gray-400 mb-2">رقم الهاتف</label>
            <input v-model="form.phone" type="tel" class="input-field" placeholder="078xxxxxxxx" required autofocus dir="ltr">
          </div>

          <div>
            <label class="block text-sm text-gray-400 mb-2">كلمة المرور</label>
            <div class="relative">
              <input v-model="form.password" :type="showPass ? 'text' : 'password'"
                class="input-field pl-12" placeholder="••••••••" required>
              <button type="button" @click="showPass = !showPass"
                class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-300">
                {{ showPass ? '🙈' : '👁' }}
              </button>
            </div>
          </div>

          <div class="flex justify-between items-center text-sm">
            <label class="flex items-center gap-2 text-gray-400 cursor-pointer">
              <input type="checkbox" class="rounded accent-gold"> تذكرني
            </label>
            <a href="#" class="text-gold hover:text-gold-light transition-colors">نسيت كلمة المرور؟</a>
          </div>

          <button type="submit" :disabled="auth.loading" class="btn-primary w-full justify-center py-4 text-lg mt-2">
            <span v-if="auth.loading" class="inline-block w-5 h-5 border-2 border-dark/30 border-t-dark rounded-full animate-spin ml-2"></span>
            {{ auth.loading ? 'جارٍ الدخول...' : 'تسجيل الدخول' }}
          </button>
        </form>

        <p class="text-center text-gray-400 mt-6 text-sm">
          ليس لديك حساب؟
          <RouterLink to="/register" class="text-gold hover:text-gold-light transition-colors font-medium">إنشاء حساب جديد</RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const showPass = ref(false)
const form = reactive({ phone: '', password: '' })

async function handleLogin() {
  await auth.login(form)
}
</script>
