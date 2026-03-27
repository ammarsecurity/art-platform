/**
 * اسم ملف ASCII فقط لـ FormData — يمنع خطأ Kestrel:
 * "Invalid non-ASCII or control character in header" عند وجود أحرف عربية في اسم الملف الأصلي.
 */
export function asciiFilenameForUpload(file) {
  if (!file?.name) return 'upload.bin'
  const match = /\.([a-zA-Z0-9]{1,10})$/.exec(file.name.trim())
  const ext = match ? `.${match[1].toLowerCase()}` : ''
  return `upload${ext}`
}
