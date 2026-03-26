import { ref } from 'vue'
import api from '@/services/api'

// Module-level cache so settings are fetched once per page load
let cache = null
let fetchPromise = null

export function useSiteSettings() {
  const settings = ref(cache || {})
  const loading = ref(!cache)

  async function fetchSettings() {
    if (cache) {
      settings.value = cache
      loading.value = false
      return
    }
    if (!fetchPromise) {
      fetchPromise = api.get('/settings')
        .then(res => {
          cache = res.data || {}
          return cache
        })
        .catch(() => {
          cache = {}
          return cache
        })
    }
    settings.value = await fetchPromise
    loading.value = false
  }

  function get(key, fallback = '') {
    return settings.value[key] ?? fallback
  }

  function getJson(key, fallback = []) {
    try {
      const val = settings.value[key]
      return val ? JSON.parse(val) : fallback
    } catch {
      return fallback
    }
  }

  return { settings, loading, fetchSettings, get, getJson }
}

export function invalidateSettingsCache() {
  cache = null
  fetchPromise = null
}
