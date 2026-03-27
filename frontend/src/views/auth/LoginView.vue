<template>
  <div class="min-h-screen bg-page flex items-center justify-center p-4 relative">
    <div class="absolute top-4 end-4 z-10">
      <ThemeToggle />
    </div>
    <div class="w-full max-w-md">
      <!-- Logo -->
      <div class="text-center mb-10">
        <RouterLink to="/" class="inline-flex items-center gap-3 group mb-6">
          <SiteLogoMark box-class="w-12 h-12 rounded-xl" text-class="text-xl" />
          <span class="text-2xl font-bold text-fg">{{ site.siteName }}</span>
        </RouterLink>
        <h1 class="text-3xl font-bold text-fg">مرحباً بعودتك</h1>
        <p class="text-fg-mute mt-2">سجّل دخولك للمتابعة</p>
      </div>

      <div class="card p-8">
        <form @submit.prevent="handleLogin" class="space-y-5">
          <div>
            <label class="block text-sm text-fg-mute mb-2">رقم الهاتف</label>
            <input v-model="form.phone" type="tel" class="input-field" placeholder="05xxxxxxxx" required autofocus dir="ltr">
          </div>

          <div>
            <label class="block text-sm text-fg-mute mb-2">كلمة المرور</label>
            <div class="relative">
              <input v-model="form.password" :type="showPass ? 'text' : 'password'"
                class="input-field pl-12" placeholder="••••••••" required>
              <button type="button" @click="showPass = !showPass"
                class="absolute left-3 top-1/2 -translate-y-1/2 text-fg-dim hover:text-fg-soft">
                {{ showPass ? '🙈' : '👁' }}
              </button>
            </div>
          </div>

          <div class="flex justify-between items-center text-sm">
            <label class="flex items-center gap-2 text-fg-mute cursor-pointer">
              <input type="checkbox" class="rounded accent-gold"> تذكرني
            </label>
            <a href="#" class="text-gold hover:text-gold-light transition-colors">نسيت كلمة المرور؟</a>
          </div>

          <button type="submit" :disabled="auth.loading" class="btn-primary w-full justify-center py-4 text-lg mt-2">
            <span v-if="auth.loading" class="inline-block w-5 h-5 border-2 border-line/40 border-t-gold rounded-full animate-spin ml-2"></span>
            {{ auth.loading ? 'جارٍ الدخول...' : 'تسجيل الدخول' }}
          </button>
        </form>

        <p class="text-center text-fg-mute mt-6 text-sm">
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
import { useSiteSettingsStore } from '@/stores/siteSettings'
import SiteLogoMark from '@/components/layout/SiteLogoMark.vue'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'

const auth = useAuthStore()
const site = useSiteSettingsStore()
const showPass = ref(false)
const form = reactive({ phone: '', password: '' })

async function handleLogin() {
  await auth.login(form)
}
</script>
