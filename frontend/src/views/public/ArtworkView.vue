<template>
  <div class="pt-24 pb-20 px-4 max-w-6xl mx-auto" v-if="artwork">
    <!-- Breadcrumb -->
    <nav class="text-sm text-gray-500 mb-8 flex items-center gap-2">
      <RouterLink to="/portfolio" class="hover:text-gray-300 transition-colors">المعرض</RouterLink>
      <span>›</span>
      <span class="text-gray-300">{{ artwork.title }}</span>
    </nav>

    <div class="grid lg:grid-cols-2 gap-16">
      <!-- Image -->
      <div class="relative">
        <div class="rounded-3xl overflow-hidden bg-dark-200 shadow-2xl">
          <img :src="artwork.imageUrl" :alt="artwork.title" class="w-full object-cover">
        </div>
      </div>

      <!-- Details -->
      <div>
        <div class="badge-gold mb-6">{{ artwork.categoryName }}</div>
        <h1 class="text-4xl font-bold text-white mb-4 leading-tight">{{ artwork.title }}</h1>
        <p v-if="artwork.description" class="text-gray-400 text-lg leading-relaxed mb-8">{{ artwork.description }}</p>

        <div class="card p-6 space-y-4 mb-8">
          <div v-if="artwork.medium" class="flex justify-between">
            <span class="text-gray-400">الخامة</span>
            <span class="text-white">{{ artwork.medium }}</span>
          </div>
          <div v-if="artwork.dimensions" class="flex justify-between">
            <span class="text-gray-400">الأبعاد</span>
            <span class="text-white">{{ artwork.dimensions }}</span>
          </div>
          <div v-if="artwork.year" class="flex justify-between">
            <span class="text-gray-400">السنة</span>
            <span class="text-white">{{ artwork.year }}</span>
          </div>
        </div>

        <div v-if="artwork.tags?.length" class="flex flex-wrap gap-2 mb-8">
          <span v-for="tag in artwork.tags" :key="tag" class="badge-gold text-sm">{{ tag }}</span>
        </div>

        <RouterLink to="/contact" class="btn-primary text-lg px-8 py-4">
          📧 استفسار عن العمل
        </RouterLink>
      </div>
    </div>
  </div>

  <div v-else-if="loading" class="pt-32 flex justify-center">
    <div class="w-12 h-12 border-4 border-gold/30 border-t-gold rounded-full animate-spin"></div>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useArtworkStore } from '@/stores/artworks'

const route = useRoute()
const store = useArtworkStore()
const artwork = computed(() => store.currentArtwork)
const loading = computed(() => store.loading)

onMounted(() => store.fetchBySlug(route.params.slug))
</script>
