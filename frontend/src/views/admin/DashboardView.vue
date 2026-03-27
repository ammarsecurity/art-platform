<template>
  <div class="animate-fade-in">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-fg">لوحة التحكم</h1>
      <p class="text-fg-mute mt-1">مرحباً {{ auth.user?.name }}، إليك ملخص اليوم</p>
    </div>

    <!-- Stats Grid -->
    <div v-if="loading" class="grid grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4 mb-10">
      <div v-for="i in 6" :key="i" class="card p-6 animate-pulse h-28"></div>
    </div>

    <div v-else class="grid grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4 mb-10">
      <div v-for="stat in statCards" :key="stat.label" class="card p-6 group hover:border-gold/40 transition-all cursor-default">
        <div class="flex items-center justify-between mb-4">
          <span class="text-2xl">{{ stat.icon }}</span>
          <span class="text-xs badge-gold">{{ stat.trend }}</span>
        </div>
        <div class="text-3xl font-bold text-fg mb-1">{{ stat.value }}</div>
        <div class="text-fg-mute text-sm">{{ stat.label }}</div>
      </div>
    </div>

    <!-- Content Grid -->
    <div class="grid lg:grid-cols-3 gap-6">
      <!-- Recent Activity -->
      <div class="lg:col-span-2 card p-6">
        <h2 class="text-xl font-bold text-fg mb-6">النشاط الأخير</h2>
        <div v-if="!stats?.recentActivity?.length" class="text-fg-dim text-center py-8">لا يوجد نشاط حديث</div>
        <div class="space-y-4">
          <div v-for="activity in stats?.recentActivity" :key="activity.title + activity.date"
            class="flex items-center gap-4 p-3 rounded-xl hover:bg-input transition-colors">
            <div class="w-10 h-10 rounded-xl flex items-center justify-center text-lg"
              :class="activity.type === 'artwork' ? 'bg-gold/10 text-gold' : 'bg-blue-500/10 text-blue-400'">
              {{ activity.type === 'artwork' ? '🖼️' : '🎓' }}
            </div>
            <div class="flex-1">
              <p class="text-fg text-sm font-medium">{{ activity.title }}</p>
              <p class="text-fg-dim text-xs">{{ formatDate(activity.date) }}</p>
            </div>
            <span class="badge text-xs" :class="activity.type === 'artwork' ? 'badge-gold' : 'bg-blue-500/20 text-blue-400 border border-blue-500/30'">
              {{ activity.type === 'artwork' ? 'عمل فني' : 'دورة' }}
            </span>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="card p-6">
        <h2 class="text-xl font-bold text-fg mb-6">إجراءات سريعة</h2>
        <div class="space-y-3">
          <RouterLink v-for="action in quickActions" :key="action.to"
            :to="action.to"
            class="flex items-center gap-3 p-4 rounded-xl bg-input hover:bg-line transition-colors group">
            <span class="text-2xl">{{ action.icon }}</span>
            <div>
              <p class="text-fg text-sm font-medium group-hover:text-gold transition-colors">{{ action.label }}</p>
              <p class="text-fg-dim text-xs">{{ action.desc }}</p>
            </div>
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { adminApi } from '@/services/api'

const auth = useAuthStore()
const loading = ref(true)
const stats = ref(null)

onMounted(async () => {
  try {
    const res = await adminApi.getDashboard()
    stats.value = res.data
  } finally {
    loading.value = false
  }
})

const statCards = computed(() => stats.value ? [
  { icon: '🖼️', label: 'الأعمال الفنية', value: stats.value.totalArtworks, trend: '+5%' },
  { icon: '🎓', label: 'الدورات', value: stats.value.totalCourses, trend: '+12%' },
  { icon: '👥', label: 'المستخدمون', value: stats.value.totalUsers, trend: '+8%' },
  { icon: '📚', label: 'التسجيلات', value: stats.value.totalEnrollments, trend: '+20%' },
  { icon: '✍️', label: 'المقالات', value: stats.value.totalBlogPosts, trend: '+3%' },
  { icon: '📨', label: 'رسائل جديدة', value: stats.value.unreadMessages, trend: 'جديد' },
] : [])

const quickActions = [
  { icon: '⚙️', label: 'إعدادات الموقع', desc: 'الصفحة الرئيسية وآراء الطلاب', to: '/admin/settings' },
  { icon: '🖼️', label: 'إضافة عمل فني', desc: 'رفع لوحة جديدة', to: '/admin/artworks/new' },
  { icon: '🎓', label: 'إضافة دورة', desc: 'إنشاء دورة تعليمية', to: '/admin/courses/new' },
  { icon: '✍️', label: 'كتابة مقال', desc: 'نشر في المدونة', to: '/admin/blog' },
  { icon: '📨', label: 'استعراض الرسائل', desc: 'رسائل التواصل', to: '/admin/messages' },
]

function formatDate(d) {
  return new Date(d).toLocaleDateString('ar-SA', { year: 'numeric', month: 'long', day: 'numeric' })
}
</script>
