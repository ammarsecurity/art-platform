<template>
  <RouterLink :to="`/portfolio/${artwork.slug}`" class="card group block w-full overflow-hidden">
    <div class="relative overflow-hidden aspect-square bg-input">
      <img
        :src="imageSrc"
        :alt="artwork.title"
        class="relative z-0 w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
      >
      <!-- Overlay -->
      <div class="pointer-events-none absolute inset-0 z-10 bg-gradient-to-t from-black/80 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>

      <!-- Featured badge -->
      <div v-if="artwork.isFeatured" class="absolute top-3 right-3 z-20">
        <span class="badge-gold text-xs">⭐ مميز</span>
      </div>

      <!-- Info on hover -->
      <div class="absolute bottom-0 left-0 right-0 z-20 p-4 translate-y-full group-hover:translate-y-0 transition-transform duration-300">
        <h3 class="text-fg font-bold text-sm mb-1 line-clamp-1">{{ artwork.title }}</h3>
        <div class="flex items-center justify-between">
          <span class="text-gold/80 text-xs">{{ artwork.categoryName }}</span>
          <span v-if="artwork.year" class="text-fg-mute text-xs">{{ artwork.year }}</span>
        </div>
      </div>
    </div>

    <!-- Card bottom (visible always on list view) -->
    <div v-if="showInfo" class="p-4">
      <h3 class="text-fg font-semibold mb-1 line-clamp-1">{{ artwork.title }}</h3>
      <div class="flex items-center justify-between text-sm">
        <span class="text-gold">{{ artwork.categoryName }}</span>
        <span class="text-fg-dim">{{ artwork.medium }}</span>
      </div>
    </div>
  </RouterLink>
</template>

<script setup>
import { computed } from 'vue'
import { resolveMediaUrl } from '@/utils/mediaUrl'

const props = defineProps({
  artwork: { type: Object, required: true },
  showInfo: { type: Boolean, default: false }
})

const imageSrc = computed(() =>
  resolveMediaUrl(props.artwork.thumbnailUrl || props.artwork.imageUrl)
)
</script>
