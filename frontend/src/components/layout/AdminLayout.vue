<template>
  <div class="min-h-screen bg-page flex" dir="rtl">
    <!-- Sidebar -->
    <aside
      class="fixed right-0 top-0 h-full bg-surface border-l border-line z-40 transition-all duration-300"
      :class="sidebarOpen ? 'w-64' : 'w-16'"
    >
      <!-- Logo -->
      <div class="flex items-center gap-3 p-4 border-b border-line h-16">
        <SiteLogoMark box-class="w-8 h-8 rounded-lg" text-class="text-sm" :shadow="false" />
        <span v-if="sidebarOpen" class="text-fg font-bold text-sm">لوحة التحكم</span>
      </div>

      <!-- Nav -->
      <nav class="p-3 space-y-1">
        <RouterLink v-for="item in navItems" :key="item.to"
          :to="item.to"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-fg-mute hover:text-fg hover:bg-input transition-all duration-200 group"
          active-class="bg-gold/10 text-gold border border-gold/20"
          :class="!sidebarOpen ? 'justify-center' : ''"
        >
          <span class="text-lg flex-shrink-0">{{ item.icon }}</span>
          <span v-if="sidebarOpen" class="text-sm font-medium">{{ item.label }}</span>

          <!-- Tooltip when collapsed -->
          <div v-if="!sidebarOpen"
            class="fixed right-16 bg-input text-fg text-xs py-1 px-2 rounded-md invisible group-hover:visible opacity-0 group-hover:opacity-100 transition-all whitespace-nowrap border border-line shadow-lg">
            {{ item.label }}
          </div>
        </RouterLink>
      </nav>

      <!-- Toggle button -->
      <button @click="sidebarOpen = !sidebarOpen"
        class="absolute -left-3 top-1/2 -translate-y-1/2 w-6 h-6 bg-input border border-line rounded-full flex items-center justify-center text-fg-mute hover:text-fg transition-colors">
        {{ sidebarOpen ? '›' : '‹' }}
      </button>
    </aside>

    <!-- Main content -->
    <div class="flex-1 flex flex-col" :class="sidebarOpen ? 'mr-64' : 'mr-16'">
      <!-- Top bar -->
      <header class="sticky top-0 z-30 bg-surface/95 backdrop-blur-sm border-b border-line h-16 flex items-center justify-between px-6">
        <div class="flex items-center gap-4">
          <button @click="sidebarOpen = !sidebarOpen" class="text-fg-mute hover:text-fg md:hidden">☰</button>
          <nav class="text-sm text-fg-dim hidden sm:flex items-center gap-2">
            <RouterLink to="/admin" class="hover:text-fg-soft">الرئيسية</RouterLink>
            <span>›</span>
            <span class="text-fg-soft">{{ currentPageName }}</span>
          </nav>
        </div>

        <div class="flex items-center gap-4">
          <ThemeToggle />
          <RouterLink to="/" target="_blank" class="text-fg-mute hover:text-gold transition-colors text-sm">
            🌐 عرض الموقع
          </RouterLink>
          <div class="flex items-center gap-2 cursor-pointer" @click="auth.logout()">
            <div class="w-8 h-8 rounded-full bg-gold/20 flex items-center justify-center text-gold font-bold text-sm">
              {{ auth.user?.name?.charAt(0) }}
            </div>
            <span class="text-fg-soft text-sm hidden sm:block">{{ auth.user?.name }}</span>
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
import SiteLogoMark from './SiteLogoMark.vue'
import ThemeToggle from '@/components/ui/ThemeToggle.vue'

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
  { to: '/admin/contact-page', icon: '📬', label: 'صفحة التواصل' },
  { to: '/admin/footer', icon: '⬇️', label: 'الفوتر' },
  { to: '/admin/settings', icon: '⚙️', label: 'إعدادات الموقع' },
  { to: '/admin/pages', icon: '📄', label: 'الصفحات القانونية' },
]

const currentPageName = computed(() => {
  if (route.path === '/admin' || route.path === '/admin/') return 'الإحصائيات'
  const item = navItems.find(i => i.to !== '/admin' && route.path.startsWith(i.to))
  return item?.label ?? ''
})
</script>
