<template>
  <nav
    class="fixed top-0 inset-x-0 z-50 transition-all duration-300"
    :class="scrolled ? 'bg-dark/95 backdrop-blur-md shadow-lg shadow-black/20 border-b border-dark-300' : 'bg-transparent'"
  >
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-18 py-4">
        <!-- Logo -->
        <RouterLink to="/" class="flex items-center gap-3 group">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-gold to-gold-dark flex items-center justify-center shadow-lg shadow-gold/30">
            <span class="text-dark font-bold text-lg">ف</span>
          </div>
          <span class="text-xl font-bold text-white group-hover:text-gold transition-colors">منصة الفن</span>
        </RouterLink>

        <!-- Desktop Links -->
        <div class="hidden md:flex items-center gap-8">
          <RouterLink v-for="link in navLinks" :key="link.to"
            :to="link.to"
            class="text-gray-300 hover:text-gold transition-colors duration-200 font-medium relative nav-link"
            active-class="text-gold"
          >{{ link.label }}</RouterLink>
        </div>

        <!-- Auth Actions -->
        <div class="hidden md:flex items-center gap-4">
          <template v-if="auth.isAuthenticated">
            <RouterLink v-if="auth.isAdmin" to="/admin"
              class="badge-gold cursor-pointer hover:bg-gold/30 transition-colors">
              لوحة التحكم
            </RouterLink>
            <!-- User dropdown -->
            <div class="relative" ref="dropdownRef">
              <button @click="dropdownOpen = !dropdownOpen"
                class="flex items-center gap-2.5 cursor-pointer group">
                <div class="w-9 h-9 rounded-full bg-gold/20 border border-gold/40 flex items-center justify-center text-gold text-sm font-bold">
                  {{ auth.user?.name?.charAt(0) }}
                </div>
                <span class="text-gray-300 text-sm group-hover:text-white transition-colors">{{ auth.user?.name }}</span>
                <svg class="w-3.5 h-3.5 text-gray-400 transition-transform" :class="dropdownOpen ? 'rotate-180' : ''" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/></svg>
              </button>
              <Transition name="dropdown">
                <div v-if="dropdownOpen" class="absolute left-0 top-12 w-48 bg-dark-100 border border-dark-300 rounded-xl shadow-xl shadow-black/30 overflow-hidden z-50">
                  <RouterLink to="/profile" @click="dropdownOpen = false"
                    class="flex items-center gap-3 px-4 py-3 text-sm text-gray-300 hover:bg-dark-200 hover:text-white transition-colors">
                    <span>👤</span> الملف الشخصي
                  </RouterLink>
                  <RouterLink to="/my-courses" @click="dropdownOpen = false"
                    class="flex items-center gap-3 px-4 py-3 text-sm text-gray-300 hover:bg-dark-200 hover:text-white transition-colors">
                    <span>🎓</span> دوراتي
                  </RouterLink>
                  <hr class="border-dark-300">
                  <button @click="auth.logout()" class="w-full flex items-center gap-3 px-4 py-3 text-sm text-red-400 hover:bg-dark-200 transition-colors">
                    <span>🚪</span> تسجيل الخروج
                  </button>
                </div>
              </Transition>
            </div>
          </template>
          <template v-else>
            <RouterLink to="/login" class="text-gray-300 hover:text-white transition-colors font-medium">دخول</RouterLink>
            <RouterLink to="/register" class="btn-primary text-sm px-5 py-2.5">إنشاء حساب</RouterLink>
          </template>
        </div>

        <!-- Mobile Menu Button -->
        <button @click="mobileOpen = !mobileOpen" class="md:hidden p-2 text-gray-300 hover:text-white">
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
      <div v-if="mobileOpen" class="md:hidden bg-dark-100/95 backdrop-blur-md border-b border-dark-300">
        <div class="px-4 py-6 space-y-4">
          <RouterLink v-for="link in navLinks" :key="link.to"
            :to="link.to" @click="mobileOpen = false"
            class="block py-2 text-gray-300 hover:text-gold transition-colors font-medium"
            active-class="text-gold">{{ link.label }}</RouterLink>
          <hr class="border-dark-300 my-4">
          <template v-if="!auth.isAuthenticated">
            <RouterLink to="/login" @click="mobileOpen = false" class="btn-outline w-full justify-center">دخول</RouterLink>
            <RouterLink to="/register" @click="mobileOpen = false" class="btn-primary w-full justify-center mt-2">إنشاء حساب</RouterLink>
          </template>
          <template v-else>
            <RouterLink to="/profile" @click="mobileOpen = false" class="block py-2 text-gray-300 hover:text-gold transition-colors">👤 الملف الشخصي</RouterLink>
            <RouterLink to="/my-courses" @click="mobileOpen = false" class="block py-2 text-gray-300 hover:text-gold transition-colors">🎓 دوراتي</RouterLink>
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

const auth = useAuthStore()
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
  background: #C9A96E;
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
