/**
 * يبني رابطاً يعمل في المتصفح للوسائط القادمة من الـ API.
 * مسارات نسبية مثل /uploads/artworks/... تُحوَّل إلى أصل الباك إند
 * (مثال: https://api.murtada-thamer.com/uploads/...).
 *
 * في التطوير على localhost يُفضَّل أحياناً نفس المنفذ مع proxy في Vite — انظر useRelativeUploadsInDev.
 */

/** مطابق لـ api.js — يُستخرج منه الـ origin فقط (بدون /api) */
const DEFAULT_API_BASE = 'https://api.murtada-thamer.com/api'

function getApiOrigin() {
  const base = import.meta.env.VITE_API_URL || DEFAULT_API_BASE
  if (!base.startsWith('http')) return ''
  try {
    return new URL(base.endsWith('/') ? base : `${base}/`).origin
  } catch {
    return ''
  }
}

export function resolveMediaUrl(path) {
  if (path == null || path === '') return ''
  const s = String(path).trim()
  if (s.startsWith('data:') || s.startsWith('blob:')) return s

  if (/^https?:\/\//i.test(s)) {
    try {
      const u = new URL(s)
      if (shouldRewriteUploadsToSameOrigin(u) && u.pathname.startsWith('/uploads')) {
        return u.pathname + u.search
      }
    } catch {
      /* ignore */
    }
    return s
  }

  const rel = s.startsWith('/') ? s : `/${s}`
  const origin = getApiOrigin()

  if (useRelativeUploadsInDev() && rel.startsWith('/uploads')) {
    return rel
  }

  if (origin) {
    return `${origin}${rel}`
  }

  return rel
}

function useRelativeUploadsInDev() {
  if (!import.meta.env.DEV) return false
  if (import.meta.env.VITE_USE_RELATIVE_MEDIA === 'true') return true
  if (typeof window !== 'undefined') {
    const h = window.location.hostname
    if (h === 'localhost' || h === '127.0.0.1') return true
  }
  return false
}

function shouldRewriteUploadsToSameOrigin(url) {
  if (import.meta.env.DEV) return true
  if (import.meta.env.VITE_USE_RELATIVE_MEDIA === 'true') return true
  if (typeof window !== 'undefined') {
    const h = window.location.hostname
    if (h === 'localhost' || h === '127.0.0.1') return true
  }
  return false
}
