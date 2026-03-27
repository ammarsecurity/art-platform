<template>
  <div class="pt-24 pb-20 px-4 max-w-7xl mx-auto">
    <!-- Header -->
    <div class="text-center mb-16">
      <div class="badge-gold mb-4">🖼️ المعرض الفني</div>
      <h1 class="section-title">أعمالي الفنية</h1>
      <p class="section-subtitle">مجموعة من أجمل اللوحات والأعمال الإبداعية</p>
    </div>

    <!-- Filters -->
    <div class="flex flex-wrap items-center gap-3 mb-10">
      <button
        @click="setCategory(null)"
        class="px-4 py-2 rounded-full border transition-all text-sm font-medium"
        :class="!activeCategory ? 'bg-gold border-gold text-dark' : 'border-line text-fg-mute hover:border-gold hover:text-gold'"
      >الكل</button>

      <button
        v-for="cat in artworkStore.categories"
        :key="cat.id"
        @click="setCategory(cat.id)"
        class="px-4 py-2 rounded-full border transition-all text-sm font-medium"
        :class="activeCategory === cat.id ? 'bg-gold border-gold text-dark' : 'border-line text-fg-mute hover:border-gold hover:text-gold'"
      >{{ cat.name }}</button>

      <!-- Search -->
      <div class="mr-auto relative">
        <input
          v-model="search"
          @input="debouncedSearch"
          type="text"
          placeholder="بحث..."
          class="input-field pr-10 w-64 text-sm py-2"
        >
        <span class="absolute right-3 top-1/2 -translate-y-1/2 text-fg-dim">🔍</span>
      </div>
    </div>

    <!-- Grid -->
    <div v-if="artworkStore.loading" class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
      <div v-for="i in 12" :key="i" class="aspect-square rounded-2xl bg-input animate-pulse"></div>
    </div>

    <div v-else-if="artworkStore.artworks.length === 0" class="text-center py-24">
      <div class="text-6xl mb-4">🎨</div>
      <p class="text-fg-mute text-xl">لا توجد أعمال فنية لعرضها</p>
    </div>

    <div v-else class="columns-2 md:columns-3 lg:columns-4 gap-4 space-y-4">
      <div v-for="artwork in artworkStore.artworks" :key="artwork.id" class="break-inside-avoid">
        <ArtworkCard :artwork="artwork" />
      </div>
    </div>

    <!-- Pagination -->
    <Pagination
      v-if="artworkStore.pagination.totalPages > 1"
      :current="artworkStore.pagination.page"
      :total="artworkStore.pagination.totalPages"
      @change="artworkStore.setPage($event)"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useArtworkStore } from '@/stores/artworks'
import ArtworkCard from '@/components/ui/ArtworkCard.vue'
import Pagination from '@/components/ui/Pagination.vue'

const artworkStore = useArtworkStore()
const activeCategory = ref(null)
const search = ref('')
let searchTimer = null

onMounted(() => {
  artworkStore.fetchArtworks()
  artworkStore.fetchCategories()
})

function setCategory(id) {
  activeCategory.value = id
  artworkStore.setFilter('categoryId', id)
}

function debouncedSearch() {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => artworkStore.setFilter('search', search.value), 400)
}
</script>
