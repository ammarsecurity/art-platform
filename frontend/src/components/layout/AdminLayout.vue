<template>
  <div class="min-h-screen bg-dark flex" dir="rtl">
    <!-- Sidebar -->
    <aside
      class="fixed right-0 top-0 h-full bg-dark-100 border-l border-dark-300 z-40 transition-all duration-300"
      :class="sidebarOpen ? 'w-64' : 'w-16'"
    >
      <!-- Logo -->
      <div class="flex items-center gap-3 p-4 border-b border-dark-300 h-16">
        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-gold to-gold-dark flex items-center justify-center flex-shrink-0">
          <span class="text-dark font-bold">ف</span>
        </div>
        <span v-if="sidebarOpen" class="text-white font-bold text-sm">لوحة التحكم</span>
      </div>

      <!-- Nav -->
      <nav class="p-3 space-y-1">
        <RouterLink v-for="item in navItems" :key="item.to"
          :to="item.to"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-gray-400 hover:text-white hover:bg-dark-200 transition-all duration-200 group"
          active-class="bg-gold/10 text-gold border border-gold/20"
          :class="!sidebarOpen ? 'justify-center' : ''"
        >
          <span class="text-lg flex-shrink-0">{{ item.icon }}</span>
          <span v-if="sidebarOpen" class="text-sm font-medium">{{ item.label }}</span>

          <!-- Tooltip when collapsed -->
          <div v-if="!sidebarOpen"
            class="fixed right-16 bg-dark-200 text-white text-xs py-1 px-2 rounded-md invisible group-hover:visible opacity-0 group-hover:opacity-100 transition-all whitespace-nowrap border border-dark-300 shadow-lg">
            {{ item.label }}
          </div>
        </RouterLink>
      </nav>

      <!-- Toggle button -->
      <button @click="sidebarOpen = !sidebarOpen"
        class="absolute -left-3 top-1/2 -translate-y-1/2 w-6 h-6 bg-dark-200 border border-dark-300 rounded-full flex items-center justify-center text-gray-400 hover:text-white transition-colors">
        {{ sidebarOpen ? '›' : '‹' }}
      </button>
    </aside>

    <!-- Main content -->
    <div class="flex-1 flex flex-col" :class="sidebarOpen ? 'mr-64' : 'mr-16'">
      <!-- Top bar -->
      <header class="sticky top-0 z-30 bg-dark-100/95 backdrop-blur-sm border-b border-dark-300 h-16 flex items-center justify-between px-6">
        <div class="flex items-center gap-4">
          <button @click="sidebarOpen = !sidebarOpen" class="text-gray-400 hover:text-white md:hidden">☰</button>
          <nav class="text-sm text-gray-500 hidden sm:flex items-center gap-2">
            <RouterLink to="/admin" class="hover:text-gray-300">الرئيسية</RouterLink>
            <span>›</span>
            <span class="text-gray-300">{{ currentPageName }}</span>
          </nav>
        </div>

        <div class="flex items-center gap-4">
          <RouterLink to="/" target="_blank" class="text-gray-400 hover:text-gold transition-colors text-sm">
            🌐 عرض الموقع
          </RouterLink>
          <div class="flex items-center gap-2 cursor-pointer" @click="auth.logout()">
            <div class="w-8 h-8 rounded-full bg-gold/20 flex items-center justify-center text-gold font-bold text-sm">
              {{ auth.user?.name?.charAt(0) }}
            </div>
            <span class="text-gray-300 text-sm hidden sm:block">{{ auth.user?.name }}</span>
          </div>
        </div>
      </header>

      <!-- Page content -->
      <main class="flex-1 p-6 overflow-auto">
        <RouterView />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const route = useRoute()
const sidebarOpen = ref(true)

const navItems = [
  { to: '/admin', icon: '📊', label: 'الإحصائيات' },
  { to: '/admin/artworks', icon: '🖼️', label: 'الأعمال الفنية' },
  { to: '/admin/courses', icon: '🎓', label: 'الدورات' },
  { to: '/admin/blog', icon: '✍️', label: 'المدونة' },
  { to: '/admin/categories', icon: '🏷️', label: 'التصنيفات' },
  { to: '/admin/users', icon: '👥', label: 'المستخدمون' },
  { to: '/admin/messages', icon: '📨', label: 'الرسائل' },
  { to: '/admin/pages', icon: '📄', label: 'الصفحات القانونية' },
]

const currentPageName = computed(() => {
  return navItems.find(i => route.path.startsWith(i.to) && i.to !== '/admin')?.label
    || (route.path === '/admin' ? 'الإحصائيات' : '')
})
</script>
