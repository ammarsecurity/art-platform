import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/services/api'
import { toast } from 'vue3-toastify'
import router from '@/router'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null)
  const token = ref(localStorage.getItem('auth_token'))
  const loading = ref(false)

  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')

  async function init() {
    if (token.value) {
      try {
        const res = await authApi.me()
        user.value = res.data
      } catch {
        logout()
      }
    }
  }

  async function login(credentials) {
    loading.value = true
    try {
      const res = await authApi.login(credentials)
      token.value = res.data.token
      user.value = res.data.user
      localStorage.setItem('auth_token', token.value)
      toast.success(`مرحباً ${user.value.name}!`)
      router.push(isAdmin.value ? '/admin' : '/')
    } finally {
      loading.value = false
    }
  }

  async function register(data) {
    loading.value = true
    try {
      const res = await authApi.register(data)
      token.value = res.data.token
      user.value = res.data.user
      localStorage.setItem('auth_token', token.value)
      toast.success('تم إنشاء حسابك بنجاح!')
      router.push('/')
    } finally {
      loading.value = false
    }
  }

  function logout() {
    user.value = null
    token.value = null
    localStorage.removeItem('auth_token')
    router.push('/login')
  }

  return { user, token, loading, isAuthenticated, isAdmin, init, login, register, logout }
})
