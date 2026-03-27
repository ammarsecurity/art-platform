import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Vue3Toastify from 'vue3-toastify'
import 'vue3-toastify/dist/index.css'
import router from './router'
import App from './App.vue'
import './assets/main.css'
import { useThemeStore } from './stores/theme'

const pinia = createPinia()
const app = createApp(App)

app.use(pinia)

// تطبيق الثيم قبل أول رسم (يتزامن مع index.html)
const themeStore = useThemeStore(pinia)
themeStore.syncFromStorage()
app.use(router)
app.use(Vue3Toastify, {
  autoClose: 3000,
  position: 'top-right',
  rtl: true,
  theme: themeStore.mode === 'dark' ? 'dark' : 'light',
})

app.mount('#app')
