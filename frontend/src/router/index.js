import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(),
  scrollBehavior: () => ({ top: 0 }),
  routes: [
    // Public routes
    {
      path: '/',
      component: () => import('@/components/layout/PublicLayout.vue'),
      children: [
        { path: '', name: 'home', component: () => import('@/views/public/HomeView.vue') },
        { path: 'portfolio', name: 'portfolio', component: () => import('@/views/public/PortfolioView.vue') },
        { path: 'portfolio/:slug', name: 'artwork', component: () => import('@/views/public/ArtworkView.vue') },
        { path: 'courses', name: 'courses', component: () => import('@/views/public/CoursesView.vue') },
        { path: 'courses/:slug', name: 'course', component: () => import('@/views/public/CourseView.vue') },
        { path: 'blog', name: 'blog', component: () => import('@/views/public/BlogView.vue') },
        { path: 'blog/:slug', name: 'post', component: () => import('@/views/public/BlogPostView.vue') },
        { path: 'about', name: 'about', component: () => import('@/views/public/AboutView.vue') },
        { path: 'contact', name: 'contact', component: () => import('@/views/public/ContactView.vue') },
        { path: 'terms', name: 'terms', component: () => import('@/views/public/TermsView.vue') },
        { path: 'privacy', name: 'privacy', component: () => import('@/views/public/PrivacyView.vue') },
        { path: 'profile', name: 'profile', component: () => import('@/views/user/ProfileView.vue'), meta: { requiresAuth: true } },
        { path: 'my-courses', name: 'my-courses', component: () => import('@/views/user/MyCoursesView.vue'), meta: { requiresAuth: true } },
      ]
    },

    // Learn route — standalone (no public navbar)
    { path: '/courses/:slug/learn', name: 'learn', component: () => import('@/views/courses/LearnView.vue'), meta: { requiresAuth: true } },

    // Auth routes
    { path: '/login', name: 'login', component: () => import('@/views/auth/LoginView.vue'), meta: { guestOnly: true } },
    { path: '/register', name: 'register', component: () => import('@/views/auth/RegisterView.vue'), meta: { guestOnly: true } },

    // Admin routes
    {
      path: '/admin',
      component: () => import('@/components/layout/AdminLayout.vue'),
      meta: { requiresAuth: true, requiresAdmin: true },
      children: [
        { path: '', name: 'admin-dashboard', component: () => import('@/views/admin/DashboardView.vue') },
        { path: 'artworks', name: 'admin-artworks', component: () => import('@/views/admin/ArtworksView.vue') },
        { path: 'artworks/new', name: 'admin-artwork-new', component: () => import('@/views/admin/ArtworkFormView.vue') },
        { path: 'artworks/:id/edit', name: 'admin-artwork-edit', component: () => import('@/views/admin/ArtworkFormView.vue') },
        { path: 'courses', name: 'admin-courses', component: () => import('@/views/admin/CoursesView.vue') },
        { path: 'courses/new', name: 'admin-course-new', component: () => import('@/views/admin/CourseFormView.vue') },
        { path: 'courses/:id/edit', name: 'admin-course-edit', component: () => import('@/views/admin/CourseFormView.vue') },
        { path: 'blog', name: 'admin-blog', component: () => import('@/views/admin/BlogView.vue') },
        { path: 'categories', name: 'admin-categories', component: () => import('@/views/admin/CategoriesView.vue') },
        { path: 'users', name: 'admin-users', component: () => import('@/views/admin/UsersView.vue') },
        { path: 'messages', name: 'admin-messages', component: () => import('@/views/admin/MessagesView.vue') },
        { path: 'pages', name: 'admin-pages', component: () => import('@/views/admin/PagesView.vue') },
      ]
    },

    { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/NotFound.vue') }
  ]
})

router.beforeEach(async (to, _from, next) => {
  const auth = useAuthStore()

  if (!auth.user && auth.token) await auth.init()

  if (to.meta.requiresAuth && !auth.isAuthenticated) return next('/login')
  if (to.meta.requiresAdmin && !auth.isAdmin) return next('/')
  if (to.meta.guestOnly && auth.isAuthenticated) return next('/')

  next()
})

export default router
