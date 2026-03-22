<template>
  <div class="pt-24 pb-20 px-4 max-w-4xl mx-auto" v-if="post">
    <RouterLink to="/blog" class="text-gray-400 hover:text-gold transition-colors text-sm mb-8 flex items-center gap-2 w-fit">
      ← العودة للمدونة
    </RouterLink>
    <div v-if="post.featuredImageUrl" class="aspect-video rounded-3xl overflow-hidden mb-10">
      <img :src="post.featuredImageUrl" :alt="post.title" class="w-full h-full object-cover">
    </div>
    <div class="text-gray-500 text-sm mb-4">{{ formatDate(post.publishedAt) }} · {{ post.viewCount }} مشاهدة</div>
    <h1 class="text-4xl font-bold text-white mb-6 leading-tight">{{ post.title }}</h1>
    <div class="prose prose-invert prose-gold max-w-none text-gray-300 leading-relaxed text-lg"
         v-html="post.content"></div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { blogApi } from '@/services/api'

const route = useRoute()
const post = ref(null)

onMounted(async () => {
  const res = await blogApi.getBySlug(route.params.slug)
  post.value = res.data
})

function formatDate(d) {
  if (!d) return ''
  return new Date(d).toLocaleDateString('ar-SA', { year: 'numeric', month: 'long', day: 'numeric' })
}
</script>
