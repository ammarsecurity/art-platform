/** يزيل وسوم HTML لمقتطف نصي مناسب لوصف meta */
export function stripHtml(html) {
  if (html == null) return ''
  return String(html)
    .replace(/<[^>]+>/g, ' ')
    .replace(/\s+/g, ' ')
    .trim()
}
