<template>
  <section
    class="relative min-h-[88vh] flex flex-col justify-end overflow-hidden bg-page"
    @mouseenter="pause"
    @mouseleave="resume"
  >
    <!-- خلفيات الشرائح -->
    <div class="absolute inset-0">
      <div
        v-for="(s, i) in slides"
        :key="i"
        class="absolute inset-0 transition-opacity duration-[900ms] ease-out"
        :class="i === current ? 'opacity-100 z-[1]' : 'opacity-0 z-0'"
      >
        <img
          :src="resolveMediaUrl(s.imageUrl)"
          alt=""
          class="w-full h-full object-cover"
          loading="eager"
          fetchpriority="high"
        >
      </div>
      <div class="absolute inset-0 z-[2] bg-gradient-to-t from-page via-page/75 to-page/30 pointer-events-none" />
      <div class="absolute inset-0 z-[2] bg-gradient-to-b from-black/40 via-transparent to-transparent pointer-events-none" />
      <div class="absolute inset-0 z-[2] pointer-events-none home-hero-pattern opacity-[0.38]" aria-hidden="true" />
    </div>

    <!-- زخرفة -->
    <div class="absolute top-24 left-8 w-56 h-56 rounded-2xl bg-gold/5 border border-gold/10 animate-float hidden lg:block z-[3] pointer-events-none"
         style="animation-delay: 0s">
      <div class="absolute inset-4 rounded-xl bg-gradient-to-br from-gold/10 to-transparent" />
    </div>
    <div class="absolute bottom-48 right-8 w-40 h-40 rounded-2xl bg-gold/5 border border-gold/10 animate-float hidden lg:block z-[3] pointer-events-none"
         style="animation-delay: 1.2s" />

    <!-- محتوى -->
    <div class="relative z-[4] w-full max-w-5xl mx-auto px-4 pb-28 pt-32 text-center">
      <div class="badge-gold mb-6 inline-flex">{{ heroBadge }}</div>

      <template v-if="useClassicHeadline">
        <h1 class="text-4xl sm:text-5xl md:text-7xl font-bold text-fg mb-6 leading-tight">
          {{ heroTitleLine1 }}
          <span class="text-gradient block mt-2">{{ heroTitleLine2 }}</span>
        </h1>
        <p class="text-lg sm:text-xl text-fg-soft mb-10 max-w-2xl mx-auto leading-relaxed">
          {{ heroParagraph }}
        </p>
      </template>
      <template v-else>
        <h1 class="text-4xl sm:text-5xl md:text-6xl font-bold text-fg mb-4 leading-tight">
          {{ active.title }}
        </h1>
        <p
          v-if="slideSubtitle"
          class="text-lg sm:text-xl text-fg-soft mb-10 max-w-2xl mx-auto leading-relaxed"
        >
          {{ slideSubtitle }}
        </p>
      </template>

      <div class="flex flex-col sm:flex-row gap-4 justify-center items-center mb-16">
        <template v-if="active.linkUrl && linkLabel">
          <RouterLink
            v-if="isInternalLink(active.linkUrl)"
            :to="active.linkUrl"
            class="btn-primary text-lg px-8 py-4"
          >
            {{ linkLabel }}
          </RouterLink>
          <a
            v-else
            :href="active.linkUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="btn-primary text-lg px-8 py-4"
          >
            {{ linkLabel }}
          </a>
        </template>
        <RouterLink to="/portfolio" class="btn-outline text-lg px-8 py-4">
          🎨 استكشف المعرض
        </RouterLink>
        <RouterLink to="/courses" class="btn-outline text-lg px-8 py-4">
          🎓 ابدأ التعلم
        </RouterLink>
      </div>

      <div class="flex flex-wrap justify-center gap-10 sm:gap-14 pb-6">
        <div v-for="(stat, idx) in stats" :key="idx" class="text-center min-w-[5rem]">
          <div class="text-3xl sm:text-4xl font-bold text-gold">{{ stat.value }}</div>
          <div class="text-fg-mute text-sm mt-1">{{ stat.label }}</div>
        </div>
      </div>
    </div>

    <!-- أسهم -->
    <button
      type="button"
      class="absolute z-[5] top-1/2 -translate-y-1/2 right-3 md:right-6 w-11 h-11 rounded-full bg-black/45 hover:bg-black/65 border border-white/15 text-white flex items-center justify-center transition-colors backdrop-blur-sm"
      aria-label="الشريحة التالية"
      @click="next"
    >
      <span class="text-xl leading-none">›</span>
    </button>
    <button
      type="button"
      class="absolute z-[5] top-1/2 -translate-y-1/2 left-3 md:left-6 w-11 h-11 rounded-full bg-black/45 hover:bg-black/65 border border-white/15 text-white flex items-center justify-center transition-colors backdrop-blur-sm"
      aria-label="الشريحة السابقة"
      @click="prev"
    >
      <span class="text-xl leading-none">‹</span>
    </button>

    <!-- نقاط -->
    <div class="absolute z-[5] bottom-8 left-1/2 -translate-x-1/2 flex gap-2">
      <button
        v-for="(_, i) in slides"
        :key="i"
        type="button"
        class="h-2 rounded-full transition-all duration-300"
        :class="i === current ? 'w-8 bg-gold' : 'w-2 bg-white/35 hover:bg-white/55'"
        :aria-label="`شريحة ${i + 1}`"
        @click="go(i)"
      />
    </div>

    <div class="absolute bottom-4 left-1/2 -translate-x-1/2 z-[5] text-gold/80 animate-bounce hidden sm:block pointer-events-none">
      <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
      </svg>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { resolveMediaUrl } from '@/utils/mediaUrl'

const props = defineProps({
  slides: { type: Array, required: true },
  intervalSec: { type: Number, default: 8 },
  heroBadge: { type: String, default: '' },
  heroTitleLine1: { type: String, default: '' },
  heroTitleLine2: { type: String, default: '' },
  heroParagraph: { type: String, default: '' },
  stats: { type: Array, default: () => [] },
})

const current = ref(0)
let timer = null
let paused = false

const active = computed(() => props.slides[current.value] || {})

const useClassicHeadline = computed(() => !String(active.value?.title || '').trim())

const slideSubtitle = computed(() => {
  if (useClassicHeadline.value) return ''
  const sub = String(active.value?.subtitle || '').trim()
  return sub || props.heroParagraph
})

const linkLabel = computed(() => String(active.value?.linkLabel || '').trim() || 'اكتشف المزيد')

function isInternalLink(url) {
  const u = String(url || '').trim()
  return u.startsWith('/') && !u.startsWith('//')
}

function intervalMs() {
  const s = Number(props.intervalSec)
  return Math.min(120_000, Math.max(3_000, (Number.isFinite(s) ? s : 8) * 1000))
}

function tick() {
  if (paused || !props.slides.length) return
  current.value = (current.value + 1) % props.slides.length
}

function startTimer() {
  clearInterval(timer)
  timer = null
  if (!props.slides.length) return
  timer = setInterval(tick, intervalMs())
}

function pause() {
  paused = true
  clearInterval(timer)
  timer = null
}

function resume() {
  paused = false
  startTimer()
}

function go(i) {
  if (i < 0 || i >= props.slides.length) return
  current.value = i
  startTimer()
}

function prev() {
  go((current.value - 1 + props.slides.length) % props.slides.length)
}

function next() {
  go((current.value + 1) % props.slides.length)
}

watch(
  () => props.slides.length,
  (n) => {
    if (current.value >= n) current.value = 0
    startTimer()
  }
)

watch(() => props.intervalSec, startTimer)

onMounted(() => startTimer())
onUnmounted(() => clearInterval(timer))
</script>
