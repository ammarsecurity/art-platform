<template>
  <nav
    class="fixed top-0 inset-x-0 z-50 transition-all duration-300"
    :class="scrolled ? 'bg-page/95 backdrop-blur-md shadow-lg shadow-black/20 border-b border-line' : 'bg-transparent'"
  >
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-18 py-4">
        <!-- Logo -->
        <RouterLink to="/" class="flex items-center gap-3 group">
          <SiteLogoMark box-class="w-10 h-10" text-class="text-lg" />
          <span class="text-xl font-bold text-fg group-hover:text-gold transition-colors">{{ site.siteName }}</span>
        </RouterLink>

        <!-- Desktop Links -->
        <div class="hidden md:flex items-center gap-8">
          <RouterLink v-for="link in navLinks" :key="link.to"
            :to="link.to"
            class="text-fg-soft hover:text-gold transition-colors duration-200 font-medium relative nav-link"
            active-class="text-gold"
          >{{ link.label }}</RouterLink>
        </div>

        <!-- Theme + Auth -->
        <div class="hidden md:flex items-center gap-2 lg:gap-3">
          <ThemeToggle compact />

          <template v-if="auth.isAuthenticated">
            <div
              class="flex min-h-[2.5rem] items-center gap-1 rounded-2xl border px-1.5 py-1 backdrop-blur-sm
                border-stone-300 bg-white shadow-md shadow-stone-900/10
                dark:border-line/70 dark:bg-input/50 dark:shadow-sm dark:shadow-black/20"
            >
              <RouterLink
                v-if="auth.isAdmin"
                to="/admin"
                class="inline-flex shrink-0 items-center rounded-lg px-2.5 py-1.5 text-xs font-semibold transition-colors focus:outline-none focus-visible:ring-2 focus-visible:ring-gold/50
                  text-gold-dark hover:bg-gold/15
                  dark:text-gold dark:hover:bg-gold/15 dark:focus-visible:ring-gold/35"
              >
                لوحة التحكم
              </RouterLink>

              <span
                v-if="auth.isAdmin"
                class="hidden h-5 w-px shrink-0 bg-stone-300 dark:bg-line/60 sm:block"
                aria-hidden="true"
              />

              <!-- User dropdown -->
              <div class="relative min-w-0" ref="dropdownRef">
                <button
                  type="button"
                  class="group flex max-w-[min(100%,14rem)] items-center gap-2 rounded-xl py-1 pe-1 ps-1.5 transition-colors sm:max-w-[18rem]
                    hover:bg-stone-100 dark:hover:bg-input/90"
                  :class="dropdownOpen ? 'bg-stone-100 dark:bg-input/90' : ''"
                  @click="dropdownOpen = !dropdownOpen"
                >
                  <div
                    class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full border text-xs font-bold
                      border-gold/50 bg-gold/25 text-gold-dark
                      dark:border-gold/35 dark:bg-gold/15 dark:text-gold"
                  >
                    {{ auth.user?.name?.charAt(0) }}
                  </div>
                  <span class="min-w-0 truncate text-sm font-semibold text-stone-800 dark:text-fg">
                    {{ auth.user?.name }}
                  </span>
                  <svg
                    class="h-4 w-4 shrink-0 text-stone-500 transition-transform duration-200 dark:text-fg-mute"
                    :class="dropdownOpen ? 'rotate-180' : ''"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    aria-hidden="true"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                  </svg>
                </button>
              <Transition name="dropdown">
                <div v-if="dropdownOpen" class="absolute end-0 top-[calc(100%+0.5rem)] z-50 w-52 overflow-hidden rounded-xl border border-line bg-surface shadow-xl shadow-black/25 dark:shadow-black/40">
                  <RouterLink to="/profile" @click="dropdownOpen = false"
                    class="flex items-center gap-3 px-4 py-3 text-sm text-fg-soft hover:bg-input hover:text-fg transition-colors">
                    <span>👤</span> الملف الشخصي
                  </RouterLink>
                  <RouterLink to="/my-courses" @click="dropdownOpen = false"
                    class="flex items-center gap-3 px-4 py-3 text-sm text-fg-soft hover:bg-input hover:text-fg transition-colors">
                    <span>🎓</span> دوراتي
                  </RouterLink>
                  <hr class="border-line">
                  <button @click="auth.logout()" class="w-full flex items-center gap-3 px-4 py-3 text-sm text-red-400 hover:bg-input transition-colors">
                    <span>🚪</span> تسجيل الخروج
                  </button>
                </div>
              </Transition>
              </div>
            </div>
          </template>
          <template v-else>
            <div class="flex items-center gap-2">
              <RouterLink
                to="/login"
                class="rounded-lg px-3 py-2 text-sm font-medium text-fg-soft transition-colors hover:bg-input/80 hover:text-fg"
              >
                دخول
              </RouterLink>
              <RouterLink to="/register" class="btn-primary text-sm px-5 py-2.5">إنشاء حساب</RouterLink>
            </div>
          </template>
        </div>

        <!-- Mobile Menu Button -->
        <button @click="mobileOpen = !mobileOpen" class="md:hidden p-2 text-fg-soft hover:text-fg">
          <div class="w-6 space-y-1.5">
            <span class="block h-0.5 bg-current transition-all" :class="mobileOpen ? 'rotate-45 translate-y-2' : ''"></span>
            <span class="block h-0.5 bg-current transition-all" :class="mobileOpen ? 'opacity-0' : ''"></span>
            <span class="block h-0.5 bg-current transition-all" :class="mobileOpen ? '-rotate-45 -translate-y-2' : ''"></span>
          </div>
        </button>
      </div>
    </div>

    <!-- Mobile Menu -->
    <Transition name="slide-down">
      <div v-if="mobileOpen" class="md:hidden bg-surface/95 backdrop-blur-md border-b border-line">
        <div class="px-4 py-6 space-y-4">
          <div class="flex justify-end pb-2 border-b border-line/60">
            <ThemeToggle />
          </div>
          <RouterLink v-for="link in navLinks" :key="link.to"
            :to="link.to" @click="mobileOpen = false"
            class="block py-2 text-fg-soft hover:text-gold transition-colors font-medium"
            active-class="text-gold">{{ link.label }}</RouterLink>
          <hr class="border-line my-4">
          <template v-if="!auth.isAuthenticated">
            <RouterLink to="/login" @click="mobileOpen = false" class="btn-outline w-full justify-center">دخول</RouterLink>
            <RouterLink to="/register" @click="mobileOpen = false" class="btn-primary w-full justify-center mt-2">إنشاء حساب</RouterLink>
          </template>
          <template v-else>
            <RouterLink to="/profile" @click="mobileOpen = false" class="block py-2 text-fg-soft hover:text-gold transition-colors">👤 الملف الشخصي</RouterLink>
            <RouterLink to="/my-courses" @click="mobileOpen = false" class="block py-2 text-fg-soft hover:text-gold transition-colors">🎓 دوراتي</RouterLink>
            <button @click="auth.logout()" class="text-red-400 font-medium mt-1">🚪 تسجيل الخروج</button>
          </template>
        </div>
      </div>
    </Transition>
  </nav>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import SiteLogoMark from './SiteLogoMark.vue'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'

const auth = useAuthStore()
const site = useSiteSettingsStore()
const scrolled = ref(false)
const mobileOpen = ref(false)
const dropdownOpen = ref(false)
const dropdownRef = ref(null)

const handleClickOutside = (e) => {
  if (dropdownRef.value && !dropdownRef.value.contains(e.target)) dropdownOpen.value = false
}

const navLinks = [
  { to: '/', label: 'الرئيسية' },
  { to: '/portfolio', label: 'المعرض' },
  { to: '/courses', label: 'الدورات' },
  { to: '/blog', label: 'المدونة' },
  { to: '/about', label: 'عن الفنان' },
  { to: '/contact', label: 'تواصل معنا' },
]

const handleScroll = () => { scrolled.value = window.scrollY > 50 }
onMounted(() => {
  window.addEventListener('scroll', handleScroll)
  document.addEventListener('click', handleClickOutside)
})
onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.nav-link::after {
  content: '';
  position: absolute;
  bottom: -4px;
  right: 0;
  width: 0;
  height: 2px;
  background: rgb(var(--color-gold));
  transition: width 0.3s ease;
}
.nav-link:hover::after,
.router-link-active::after {
  width: 100%;
}
.slide-down-enter-active, .slide-down-leave-active {
  transition: all 0.3s ease;
}
.slide-down-enter-from, .slide-down-leave-to {
  transform: translateY(-10px);
  opacity: 0;
}
.dropdown-enter-active, .dropdown-leave-active {
  transition: all 0.2s ease;
}
.dropdown-enter-from, .dropdown-leave-to {
  transform: translateY(-8px);
  opacity: 0;
}
</style>
