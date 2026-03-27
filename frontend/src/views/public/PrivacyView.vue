<template>
  <div class="min-h-screen py-24 px-4">
    <div class="max-w-3xl mx-auto">
      <div v-if="loading" class="space-y-4">
        <div v-for="i in 8" :key="i" class="h-4 bg-input rounded animate-pulse"></div>
      </div>
      <div v-else class="prose-content" v-html="rendered"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'

const loading = ref(true)
const rendered = ref('')

function renderMarkdown(text) {
  return text
    .replace(/^# (.+)$/gm, '<h1 class="text-3xl font-bold text-fg mb-6 mt-8">$1</h1>')
    .replace(/^## (.+)$/gm, '<h2 class="text-xl font-semibold text-gold mb-3 mt-6">$1</h2>')
    .replace(/^### (.+)$/gm, '<h3 class="text-lg font-semibold text-fg-soft mb-2 mt-4">$1</h3>')
    .replace(/\n\n/g, '</p><p class="text-fg-mute leading-relaxed mb-4">')
    .replace(/\*\*(.+?)\*\*/g, '<strong class="text-fg">$1</strong>')
}

onMounted(async () => {
  try {
    const res = await api.get('/pages/privacy_policy')
    rendered.value = renderMarkdown(res.data.content)
  } catch {
    rendered.value = '<p class="text-fg-mute">تعذّر تحميل الصفحة.</p>'
  } finally {
    loading.value = false
  }
})
</script>
