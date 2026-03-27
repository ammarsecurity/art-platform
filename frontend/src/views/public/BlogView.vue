<template>
  <div class="pt-24 pb-20 px-4 max-w-6xl mx-auto">
    <div class="text-center mb-16">
      <div class="badge-gold mb-4">✍️ المدونة</div>
      <h1 class="section-title">مقالات وأفكار</h1>
      <p class="section-subtitle">رؤى وإلهامات من عالم الفن</p>
    </div>

    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="i in 6" :key="i" class="rounded-2xl bg-input animate-pulse h-80"></div>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <RouterLink v-for="post in posts" :key="post.id" :to="`/blog/${post.slug}`" class="card group">
        <div class="aspect-video overflow-hidden">
          <img :src="postImageSrc(post.featuredImageUrl)"
            :alt="post.title" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500">
        </div>
        <div class="p-6">
          <div class="text-fg-dim text-xs mb-3">{{ formatDate(post.publishedAt) }}</div>
          <h3 class="text-fg font-bold text-lg mb-3 group-hover:text-gold transition-colors line-clamp-2">{{ post.title }}</h3>
          <p class="text-fg-mute text-sm line-clamp-3">{{ post.excerpt }}</p>
          <div class="mt-4 text-gold text-sm font-medium">اقرأ المزيد ←</div>
        </div>
      </RouterLink>
    </div>

    <Pagination v-if="totalPages > 1" :current="page" :total="totalPages" @change="changePage" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { blogApi } from '@/services/api'
import Pagination from '@/components/ui/Pagination.vue'
import { resolveMediaUrl } from '@/utils/mediaUrl'

function postImageSrc(url) {
  return resolveMediaUrl(url) || 'https://picsum.photos/400/225?grayscale'
}

const posts = ref([])
const loading = ref(false)
const page = ref(1)
const totalPages = ref(1)

onMounted(fetchPosts)

async function fetchPosts() {
  loading.value = true
  try {
    const res = await blogApi.getAll({
      page: page.value,
      pageSize: 9,
      sortBy: 'sortorder',
      sortOrder: 'desc'
    })
    posts.value = res.data.items
    totalPages.value = res.data.totalPages
  } finally { loading.value = false }
}

function changePage(p) { page.value = p; fetchPosts() }
function formatDate(d) {
  if (!d) return ''
  return new Date(d).toLocaleDateString('ar-SA', { year: 'numeric', month: 'long', day: 'numeric' })
}
</script>
