import axios from 'axios'
import { toast } from 'vue3-toastify'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' }
})

// Request interceptor — attach token
api.interceptors.request.use(config => {
  const token = localStorage.getItem('auth_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  if (config.data instanceof FormData) {
    delete config.headers['Content-Type']
  }
  return config
})

// Response interceptor — handle errors
api.interceptors.response.use(
  response => response.data,
  error => {
    const status = error.response?.status
    const isNetworkError = !error.response
    // صامت لأخطاء الشبكة (الباك إيند غير متاح) وأخطاء 422
    if (status === 401) {
      localStorage.removeItem('auth_token')
      window.location.href = '/login'
    } else if (!isNetworkError && status !== 422 && status !== 500) {
      const message = error.response?.data?.message || 'حدث خطأ غير متوقع'
      toast.error(message)
    }
    return Promise.reject(error)
  }
)

// ── Auth ──────────────────────────────────────────────
export const authApi = {
  login: (data) => api.post('/auth/login', data),
  register: (data) => api.post('/auth/register', data),
  me: () => api.get('/auth/me'),
  updateProfile: (data) => api.put('/auth/profile', data),
  myCourses: () => api.get('/auth/my-courses'),
}

// ── Artworks ──────────────────────────────────────────
export const artworkApi = {
  getAll: (params) => api.get('/artworks', { params }),
  getFeatured: (count = 6) => api.get('/artworks/featured', { params: { count } }),
  getById: (id) => api.get(`/artworks/by-id/${id}`),
  getBySlug: (slug) => api.get(`/artworks/${encodeURIComponent(slug)}`),
  create: (formData) => api.post('/artworks', formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  update: (id, formData) => api.put(`/artworks/${id}`, formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  delete: (id) => api.delete(`/artworks/${id}`),
}

// ── Categories ────────────────────────────────────────
export const categoryApi = {
  getAll: () => api.get('/categories'),
  create: (data) => api.post('/categories', data),
  update: (id, data) => api.put(`/categories/${id}`, data),
  delete: (id) => api.delete(`/categories/${id}`),
}

// ── Courses ───────────────────────────────────────────
export const courseApi = {
  getAll: (params) => api.get('/courses', { params }),
  getFeatured: (count = 4) => api.get('/courses/featured', { params: { count } }),
  getById: (id) => api.get(`/courses/by-id/${id}`),
  getBySlug: (slug) => api.get(`/courses/${encodeURIComponent(slug)}`),
  create: (formData) => api.post('/courses', formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  update: (id, formData) => api.put(`/courses/${id}`, formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  setStatus: (id, status) => api.patch(`/courses/${id}/status`, { status }),
  delete: (id) => api.delete(`/courses/${id}`),
  enroll: (courseId) => api.post(`/courses/${courseId}/enroll`),
  updateProgress: (data) => api.post('/courses/progress', data),
  // رفع فيديو كبير قد يتجاوز 30 ثانية — مهلة أطول
  addLesson: (formData) =>
    api.post('/courses/lessons', formData, { timeout: 600_000 }),
  updateLesson: (id, formData) =>
    api.put(`/courses/lessons/${id}`, formData, { timeout: 600_000 }),
  deleteLesson: (id) => api.delete(`/courses/lessons/${id}`),
}

// ── Blog ──────────────────────────────────────────────
export const blogApi = {
  getAll: (params) => api.get('/blog', { params }),
  getById: (id) => api.get(`/blog/by-id/${id}`),
  getBySlug: (slug) => api.get(`/blog/${slug}`),
  create: (formData) => api.post('/blog', formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  update: (id, formData) => api.put(`/blog/${id}`, formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  delete: (id) => api.delete(`/blog/${id}`),
}

// ── Contact ───────────────────────────────────────────
export const contactApi = {
  send: (data) => api.post('/contact', data),
  getAll: (params) => api.get('/contact', { params }),
  markRead: (id) => api.patch(`/contact/${id}/read`),
  delete: (id) => api.delete(`/contact/${id}`),
}

// ── Site settings (عام / الصفحة الرئيسية / آراء الطلاب) ─────────────
export const settingsApi = {
  getPublic: () => api.get('/settings/public'),
  getAll: () => api.get('/settings'),
  updateKey: (key, value) => api.put(`/settings/${encodeURIComponent(key)}`, { value }),
  updateBatch: (payload) => api.put('/settings/batch', payload),
  /** رفع صورة قسم «عن الفنان» — FormData مع الحقل `file` */
  uploadAboutImage: (formData) => api.post('/settings/upload-image', formData),
}

// ── Admin ─────────────────────────────────────────────
export const adminApi = {
  getDashboard: () => api.get('/admin/dashboard'),
  getUsers: () => api.get('/admin/users'),
  updateUser: (id, data) => api.put(`/admin/users/${id}`, data),
  deleteUser: (id) => api.delete(`/admin/users/${id}`),
  resetUserPassword: (id, data) => api.put(`/admin/users/${id}/password`, data),
}

export default api
