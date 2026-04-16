/** عنوان الموقع العام (لـ canonical و Open Graph) — يُفضّل ضبط VITE_SITE_URL في الإنتاج */
export function getSiteOrigin() {
  const env = import.meta.env.VITE_SITE_URL
  if (env && typeof env === 'string') return env.replace(/\/$/, '')
  if (typeof window !== 'undefined' && window.location?.origin) return window.location.origin
  return ''
}

export const DEFAULT_SITE_NAME = 'مرتضى ثامر'

export const DEFAULT_DESCRIPTION =
  'مرتضى ثامر — استكشف معرض الأعمال الفنية، الدورات التعليمية، والمدونة. فن عربي احترافي وتعليم للموهوبين.'

/** صورة افتراضية لـ og:image عند عدم توفر صورة للصفحة */
export const DEFAULT_OG_IMAGE_PATH = '/favicon.ico'
