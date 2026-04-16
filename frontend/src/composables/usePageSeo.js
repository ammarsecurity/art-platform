import { computed, unref } from 'vue'
import { useSeoMeta, useHead } from '@unhead/vue'
import { getSiteOrigin, DEFAULT_DESCRIPTION, DEFAULT_OG_IMAGE_PATH } from '@/config/site'

function absoluteUrl(maybeRelative) {
  const origin = getSiteOrigin()
  if (!maybeRelative) return origin ? `${origin}${DEFAULT_OG_IMAGE_PATH}` : ''
  if (/^https?:\/\//i.test(maybeRelative)) return maybeRelative
  const path = maybeRelative.startsWith('/') ? maybeRelative : `/${maybeRelative}`
  return origin ? `${origin}${path}` : path
}

/**
 * SEO لصفحة ديناميكية (عمل، مقال، دورة).
 * يمرّر مراجع computed أو قيم ثابتة.
 *
 * @param {object} input
 * @param {import('vue').MaybeRefOrGetter<string>} input.title
 * @param {import('vue').MaybeRefOrGetter<string>} [input.description]
 * @param {import('vue').MaybeRefOrGetter<string|undefined>} [input.imageUrl] — مطلق أو مسار
 * @param {'website'|'article'} [input.ogType]
 * @param {import('vue').MaybeRefOrGetter<object|object[]|null>} [input.jsonLd]
 */
export function usePageSeo(input) {
  const title = computed(() => {
    const t = unref(input.title)
    return (t && String(t).trim()) || 'صفحة'
  })

  const description = computed(() => {
    const d = input.description != null ? unref(input.description) : ''
    const s = (d && String(d).trim()) || DEFAULT_DESCRIPTION
    return s.length > 160 ? `${s.slice(0, 157)}…` : s
  })

  const image = computed(() => absoluteUrl(input.imageUrl != null ? unref(input.imageUrl) : ''))

  const ogType = computed(() => unref(input.ogType) || 'article')

  useSeoMeta({
    title,
    description,
    ogTitle: title,
    ogDescription: description,
    ogImage: image,
    ogType,
    ogLocale: 'ar_SA',
    twitterCard: 'summary_large_image',
    twitterTitle: title,
    twitterDescription: description,
    twitterImage: image,
    robots: 'index, follow',
  })

  useHead({
    script: computed(() => {
      const raw = input.jsonLd != null ? unref(input.jsonLd) : null
      if (!raw) return []
      const list = Array.isArray(raw) ? raw : [raw]
      return list
        .filter(Boolean)
        .map((node) => ({
          type: 'application/ld+json',
          innerHTML: JSON.stringify(node),
        }))
    }),
  })
}
