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
        {
          path: '',
          name: 'home',
          component: () => import('@/views/public/HomeView.vue'),
          meta: {
            seo: {
              title: 'الرئيسية',
              description:
                'منصة عربية للفن والتعليم: معرض أعمال، دورات فيديو، ومدونة — تعلّم الفن واستلهم من محتوى احترافي.',
            },
          },
        },
        {
          path: 'portfolio',
          name: 'portfolio',
          component: () => import('@/views/public/PortfolioView.vue'),
          meta: {
            seo: {
              title: 'المعرض الفني',
              description: 'تصفّح مجموعة من الأعمال الفنية بخامات وأسلوب متنوّع، مع تفاصيل كل عمل ووسيلة للتواصل.',
            },
          },
        },
        {
          path: 'portfolio/:slug',
          name: 'artwork',
          component: () => import('@/views/public/ArtworkView.vue'),
          meta: { seo: { delegate: true } },
        },
        {
          path: 'courses',
          name: 'courses',
          component: () => import('@/views/public/CoursesView.vue'),
          meta: {
            seo: {
              title: 'الدورات التعليمية',
              description: 'دورات فيديو في الفنون والتصميم لمستويات مختلفة — تعلّم بالوتيرة التي تناسبك.',
            },
          },
        },
        {
          path: 'courses/:slug',
          name: 'course',
          component: () => import('@/views/public/CourseView.vue'),
          meta: { seo: { delegate: true } },
        },
        {
          path: 'blog',
          name: 'blog',
          component: () => import('@/views/public/BlogView.vue'),
          meta: {
            seo: {
              title: 'المدونة',
              description: 'مقالات ونصائح حول الفن، الإبداع، والتعلّم الذاتي.',
            },
          },
        },
        {
          path: 'blog/:slug',
          name: 'post',
          component: () => import('@/views/public/BlogPostView.vue'),
          meta: { seo: { delegate: true } },
        },
        {
          path: 'about',
          name: 'about',
          component: () => import('@/views/public/AboutView.vue'),
          meta: {
            seo: {
              title: 'من نحن',
              description: 'تعرّف على رؤية المنصة وقصة الفنان وراء المحتوى والدورات.',
            },
          },
        },
        {
          path: 'contact',
          name: 'contact',
          component: () => import('@/views/public/ContactView.vue'),
          meta: {
            seo: {
              title: 'اتصل بنا',
              description: 'أرسل استفسارك أو طلبك بخصوص الأعمال أو الدورات — نرد في أقرب وقت.',
            },
          },
        },
        {
          path: 'terms',
          name: 'terms',
          component: () => import('@/views/public/TermsView.vue'),
          meta: {
            seo: {
              title: 'الشروط والأحكام',
              description: 'شروط استخدام المنصة والخدمات التعليمية والمحتوى المنشور.',
            },
          },
        },
        {
          path: 'privacy',
          name: 'privacy',
          component: () => import('@/views/public/PrivacyView.vue'),
          meta: {
            seo: {
              title: 'سياسة الخصوصية',
              description: 'كيف نتعامل مع بياناتك عند التسجيل والتصفح واستخدام الدورات.',
            },
          },
        },
        {
          path: 'profile',
          name: 'profile',
          component: () => import('@/views/user/ProfileView.vue'),
          meta: {
            requiresAuth: true,
            seo: {
              title: 'الملف الشخصي',
              description: 'إدارة بيانات حسابك على المنصة.',
              noindex: true,
            },
          },
        },
        {
          path: 'my-courses',
          name: 'my-courses',
          component: () => import('@/views/user/MyCoursesView.vue'),
          meta: {
            requiresAuth: true,
            seo: {
              title: 'دوراتي',
              description: 'الدورات المسجّل بها وتقدّمك في التعلم.',
              noindex: true,
            },
          },
        },
      ]
    },

    // Learn route — standalone (no public navbar)
    {
      path: '/courses/:slug/learn',
      name: 'learn',
      component: () => import('@/views/courses/LearnView.vue'),
      meta: {
        requiresAuth: true,
        seo: {
          title: 'مشاهدة الدرس',
          description: 'واجهة التعلم للدورة.',
          noindex: true,
        },
      },
    },

    // Auth routes
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/auth/LoginView.vue'),
      meta: {
        guestOnly: true,
        seo: {
          title: 'تسجيل الدخول',
          description: 'ادخل إلى حسابك للوصول إلى الدورات والملف الشخصي.',
          noindex: true,
        },
      },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/auth/RegisterView.vue'),
      meta: {
        guestOnly: true,
        seo: {
          title: 'إنشاء حساب',
          description: 'سجّل حساباً جديداً للانضمام إلى الدورات والمجتمع.',
          noindex: true,
        },
      },
    },

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
        { path: 'courses/:id/lessons', name: 'admin-course-lessons', component: () => import('@/views/admin/CourseLessonsView.vue') },
        { path: 'courses/:id/edit', name: 'admin-course-edit', component: () => import('@/views/admin/CourseFormView.vue') },
        { path: 'blog', name: 'admin-blog', component: () => import('@/views/admin/BlogView.vue') },
        { path: 'categories', name: 'admin-categories', component: () => import('@/views/admin/CategoriesView.vue') },
        { path: 'users', name: 'admin-users', component: () => import('@/views/admin/UsersView.vue') },
        { path: 'messages', name: 'admin-messages', component: () => import('@/views/admin/MessagesView.vue') },
        { path: 'settings', name: 'admin-settings', component: () => import('@/views/admin/SiteSettingsView.vue') },
        { path: 'contact-page', name: 'admin-contact-page', component: () => import('@/views/admin/ContactPageSettingsView.vue') },
        { path: 'footer', name: 'admin-footer', component: () => import('@/views/admin/FooterSettingsView.vue') },
        { path: 'pages', name: 'admin-pages', component: () => import('@/views/admin/PagesView.vue') },
      ]
    },

    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('@/views/NotFound.vue'),
      meta: {
        seo: {
          title: 'الصفحة غير موجودة',
          description: 'لم يتم العثور على الصفحة المطلوبة.',
        },
      },
    }
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
