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
  getBySlug: (slug) => api.get(`/artworks/${slug}`),
  getById: (id) => api.get(`/artworks/${id}`),
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
  getBySlug: (slug) => api.get(`/courses/${slug}`),
  create: (formData) => api.post('/courses', formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  update: (id, formData) => api.put(`/courses/${id}`, formData, { headers: { 'Content-Type': 'multipart/form-data' } }),
  delete: (id) => api.delete(`/courses/${id}`),
  enroll: (courseId) => api.post(`/courses/${courseId}/enroll`),
  updateProgress: (data) => api.post('/courses/progress', data),
  getLessons: (courseId) => api.get(`/courses/${courseId}/lessons`),
  addLesson: (data) => api.post('/courses/lessons', data),
  updateLesson: (id, data) => api.put(`/courses/lessons/${id}`, data),
  deleteLesson: (id) => api.delete(`/courses/lessons/${id}`),
  reorderLessons: (courseId, ids) => api.put(`/courses/${courseId}/lessons/reorder`, { ids }),
}

// ── Blog ──────────────────────────────────────────────
export const blogApi = {
  getAll: (params) => api.get('/blog', { params }),
  getBySlug: (slug) => api.get(`/blog/${slug}`),
  getById: (id) => api.get(`/blog/${id}`),
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

// ── Testimonials ──────────────────────────────────────
export const testimonialApi = {
  getActive: () => api.get('/testimonials'),
  getAll: () => api.get('/testimonials/all'),
  create: (data) => api.post('/testimonials', data),
  update: (id, data) => api.put(`/testimonials/${id}`, data),
  delete: (id) => api.delete(`/testimonials/${id}`),
  toggle: (id) => api.put(`/testimonials/${id}/toggle`),
}

// ── Users (Admin) ─────────────────────────────────────
export const userApi = {
  getAll: (params) => api.get('/users', { params }),
  updateRole: (id, role) => api.put(`/users/${id}/role`, { role }),
  toggleStatus: (id) => api.put(`/users/${id}/status`),
}

// ── Admin ─────────────────────────────────────────────
export const adminApi = {
  getDashboard: () => api.get('/admin/dashboard'),
}

export default api
