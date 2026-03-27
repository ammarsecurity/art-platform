<template>
  <div
    class="flex items-center justify-center overflow-hidden flex-shrink-0 bg-transparent"
    :class="[boxClass, shadow ? 'shadow-lg shadow-gold/20' : '']"
  >
    <img
      v-if="src"
      :src="src"
      alt=""
      class="h-full w-full max-h-full object-contain object-center"
    >
    <span v-else class="font-bold text-gold leading-none" :class="textClass">{{ fallbackLetter }}</span>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import { resolveMediaUrl } from '@/utils/mediaUrl'

const props = defineProps({
  boxClass: { type: String, default: 'w-10 h-10 rounded-xl' },
  textClass: { type: String, default: 'text-lg' },
  /** يغلب حرف الفوتر الافتراضي من الإعدادات */
  letter: { type: String, default: '' },
  /** ظل اختياري حول الحاوية (افتراضياً بدون ظل مع إزالة الخلفية) */
  shadow: { type: Boolean, default: false },
})

const site = useSiteSettingsStore()

onMounted(() => {
  site.load()
})

const src = computed(() => {
  const u = site.siteLogoUrl
  return u ? resolveMediaUrl(u) : ''
})

const fallbackLetter = computed(() => {
  if (props.letter) return props.letter
  return site.data?.footer?.brandLetter || 'ف'
})
</script>
