import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useSeoMeta, useHead } from '@unhead/vue'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import {
  getSiteOrigin,
  DEFAULT_DESCRIPTION,
  DEFAULT_SITE_NAME,
} from '@/config/site'

function pathNoQuery(path) {
  const q = path.indexOf('?')
  return q === -1 ? path : path.slice(0, q)
}

/** مناطق لا يجب فهرستها */
function shouldNoindex(path, name) {
  if (path.startsWith('/admin')) return true
  if (['/login', '/register', '/profile', '/my-courses'].includes(path)) return true
  if (/^\/courses\/[^/]+\/learn$/.test(path)) return true
  if (name === 'not-found') return true
  return false
}

/**
 * SEO للمسارات الثابتة + canonical و robots.
 * الصفحات ذات meta.seo.delegate تترك العنوان والوصف لعنصر الصفحة (عمل، مقال، دورة).
 */
export function useAppRouteSeo() {
  const route = useRoute()
  const site = useSiteSettingsStore()

  const siteName = computed(() => site.siteName || DEFAULT_SITE_NAME)

  const fullUrl = computed(() => {
    const origin = getSiteOrigin()
    if (!origin) return ''
    return `${origin}${pathNoQuery(route.path)}`
  })

  const delegated = computed(() => !!route.meta.seo?.delegate)

  const staticBlock = computed(() => {
    const seo = route.meta.seo
    if (!seo || seo.delegate) return null
    return {
      title: seo.title || siteName.value,
      description: seo.description || DEFAULT_DESCRIPTION,
    }
  })

  const robots = computed(() => {
    if (shouldNoindex(route.path, route.name)) return 'noindex, nofollow'
    if (route.meta.seo?.noindex) return 'noindex, nofollow'
    return 'index, follow'
  })

  useSeoMeta({
    title: computed(() => {
      if (delegated.value) return undefined
      return staticBlock.value?.title || siteName.value
    }),
    description: computed(() => {
      if (delegated.value) return undefined
      return staticBlock.value?.description || DEFAULT_DESCRIPTION
    }),
    ogTitle: computed(() => {
      if (delegated.value) return undefined
      return staticBlock.value?.title || siteName.value
    }),
    ogDescription: computed(() => {
      if (delegated.value) return undefined
      return staticBlock.value?.description || DEFAULT_DESCRIPTION
    }),
    ogType: 'website',
    ogLocale: 'ar_SA',
    ogUrl: fullUrl,
    twitterCard: 'summary_large_image',
    twitterTitle: computed(() => {
      if (delegated.value) return undefined
      return staticBlock.value?.title || siteName.value
    }),
    twitterDescription: computed(() => {
      if (delegated.value) return undefined
      return staticBlock.value?.description || DEFAULT_DESCRIPTION
    }),
    robots,
  })

  useHead({
    htmlAttrs: {
      lang: 'ar',
      dir: 'rtl',
    },
    link: computed(() => {
      const href = fullUrl.value
      if (!href) return []
      return [{ rel: 'canonical', href }]
    }),
  })
}
