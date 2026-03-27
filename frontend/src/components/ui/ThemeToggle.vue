<template>
  <button
    type="button"
    class="inline-flex h-9 min-w-[2.25rem] shrink-0 items-center justify-center rounded-lg border border-transparent text-fg-mute transition-colors hover:border-line/60 hover:bg-input/80 hover:text-gold focus:outline-none focus-visible:ring-2 focus-visible:ring-gold/40 focus-visible:ring-offset-2 focus-visible:ring-offset-page"
    :class="compact ? 'px-2' : 'gap-1.5 px-2.5'"
    :title="theme.mode === 'dark' ? 'الوضع الفاتح' : 'الوضع الداكن'"
    :aria-label="theme.mode === 'dark' ? 'تفعيل الوضع الفاتح' : 'تفعيل الوضع الداكن'"
    @click="theme.toggle()"
  >
    <span class="text-[1.15rem] leading-none" aria-hidden="true">{{ theme.mode === 'dark' ? '☀️' : '🌙' }}</span>
    <span v-if="!compact" class="hidden text-xs font-medium sm:inline">{{ theme.mode === 'dark' ? 'فاتح' : 'داكن' }}</span>
  </button>
</template>

<script setup>
import { useThemeStore } from '@/stores/theme'

defineProps({
  /** أيقونة فقط بدون نص «فاتح/داكن» — مناسب للشريط العلوي */
  compact: { type: Boolean, default: false },
})

const theme = useThemeStore()
</script>
