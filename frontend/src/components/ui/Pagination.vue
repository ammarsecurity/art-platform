<template>
  <div class="flex items-center justify-center gap-2 mt-12">
    <button
      @click="emit('change', current - 1)"
      :disabled="current <= 1"
      class="px-4 py-2 rounded-lg border border-dark-300 text-gray-400 hover:border-gold hover:text-gold transition-all disabled:opacity-30 disabled:cursor-not-allowed"
    >← السابق</button>

    <div class="flex gap-2">
      <button v-for="page in visiblePages" :key="page"
        @click="page !== '...' && emit('change', page)"
        class="w-10 h-10 rounded-lg border transition-all font-medium text-sm"
        :class="page === current
          ? 'bg-gold border-gold text-dark font-bold'
          : page === '...'
            ? 'border-transparent text-gray-500 cursor-default'
            : 'border-dark-300 text-gray-400 hover:border-gold hover:text-gold'"
      >{{ page }}</button>
    </div>

    <button
      @click="emit('change', current + 1)"
      :disabled="current >= total"
      class="px-4 py-2 rounded-lg border border-dark-300 text-gray-400 hover:border-gold hover:text-gold transition-all disabled:opacity-30 disabled:cursor-not-allowed"
    >التالي →</button>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  current: { type: Number, required: true },
  total: { type: Number, required: true },
})
const emit = defineEmits(['change'])

const visiblePages = computed(() => {
  const pages = []
  if (props.total <= 7) {
    for (let i = 1; i <= props.total; i++) pages.push(i)
  } else {
    pages.push(1)
    if (props.current > 3) pages.push('...')
    for (let i = Math.max(2, props.current - 1); i <= Math.min(props.total - 1, props.current + 1); i++)
      pages.push(i)
    if (props.current < props.total - 2) pages.push('...')
    pages.push(props.total)
  }
  return pages
})
</script>
