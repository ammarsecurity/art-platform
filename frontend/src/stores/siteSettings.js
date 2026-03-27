import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { settingsApi } from '@/services/api'

export const useSiteSettingsStore = defineStore('siteSettings', () => {
  const data = ref(null)
  const loading = ref(false)

  async function load() {
    if (data.value) return
    loading.value = true
    try {
      const res = await settingsApi.getPublic()
      data.value = res.data
    } catch {
      data.value = null
    } finally {
      loading.value = false
    }
  }

  /** إعادة تحميل بعد التعديل من لوحة التحكم */
  async function refresh() {
    data.value = null
    await load()
  }

  const siteName = computed(() => data.value?.siteName || 'منصة الفن')
  /** رابط صورة الشعار بعد الحفظ من إعدادات الموقع */
  const siteLogoUrl = computed(() => data.value?.siteLogoUrl || '')

  return { data, loading, load, refresh, siteName, siteLogoUrl }
})
