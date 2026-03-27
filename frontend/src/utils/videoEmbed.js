/**
 * رابط تضمين يوتيوب لـ iframe، أو null إن لم يكن الرابط يوتيوب.
 */
export function getYoutubeEmbedUrl(url) {
  if (!url || typeof url !== 'string') return null
  const u = url.trim()
  try {
    const parsed = new URL(u)
    const host = parsed.hostname.replace(/^www\./, '')
    let id = null
    if (host === 'youtu.be') {
      id = parsed.pathname.replace(/^\//, '').split('/')[0]
    } else if (host === 'youtube.com' || host === 'm.youtube.com') {
      if (parsed.pathname.startsWith('/embed/')) {
        id = parsed.pathname.split('/')[2]
      } else if (parsed.pathname.startsWith('/shorts/')) {
        id = parsed.pathname.split('/')[2]
      } else {
        id = parsed.searchParams.get('v')
      }
    }
    if (!id || !/^[a-zA-Z0-9_-]{6,}$/.test(id)) return null
    return `https://www.youtube.com/embed/${id}`
  } catch {
    return null
  }
}

/** مناسب لعنصر video (ملف مرفوع أو mp4 مباشر) */
export function isDirectVideoPlaybackUrl(url) {
  if (!url || typeof url !== 'string') return false
  const u = url.trim()
  if (u.startsWith('/uploads/')) return true
  return /\.(mp4|webm|ogg|m4v|mov)(\?|#|$)/i.test(u)
}
