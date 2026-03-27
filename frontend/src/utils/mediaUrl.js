/**
 * Builds a browser-loadable URL for media returned by the API.
 * Prefer same-origin `/uploads/...` in dev (Vite proxies to the API) so images are not
 * loaded cross-origin from another port (avoids broken thumbnails on localhost).
 * Absolute URLs to external hosts are kept as-is.
 */
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
  const apiUrl = import.meta.env.VITE_API_URL || ''

  if (apiUrl.startsWith('http')) {
    try {
      const u = new URL(apiUrl.endsWith('/') ? apiUrl : `${apiUrl}/`)
      return `${u.origin}${rel}`
    } catch {
      return rel
    }
  }

  return rel
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
