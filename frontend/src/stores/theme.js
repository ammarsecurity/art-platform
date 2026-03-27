import { defineStore } from 'pinia'
import { ref } from 'vue'

const STORAGE_KEY = 'art_theme'

function applyHtmlClass(mode) {
  const root = document.documentElement
  root.classList.remove('light', 'dark')
  root.classList.add(mode)
}

export const useThemeStore = defineStore('theme', () => {
  const mode = ref(
    typeof localStorage !== 'undefined' && localStorage.getItem(STORAGE_KEY) === 'light'
      ? 'light'
      : 'dark'
  )

  function setMode(m) {
    mode.value = m
    localStorage.setItem(STORAGE_KEY, m)
    applyHtmlClass(m)
  }

  function toggle() {
    setMode(mode.value === 'dark' ? 'light' : 'dark')
  }

  /** يُستدعى من main.js بعد createPinia — يُطبّق الفئة على html */
  function syncFromStorage() {
    const saved = localStorage.getItem(STORAGE_KEY)
    const m = saved === 'light' ? 'light' : 'dark'
    mode.value = m
    applyHtmlClass(m)
  }

  return { mode, setMode, toggle, syncFromStorage }
})
