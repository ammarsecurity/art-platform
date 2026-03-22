<template>
  <footer class="bg-dark-100 border-t border-dark-300 mt-20">
    <div class="max-w-7xl mx-auto px-4 py-16">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-10 mb-12">
        <!-- Brand -->
        <div class="md:col-span-2">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-gold to-gold-dark flex items-center justify-center">
              <span class="text-dark font-bold text-lg">ف</span>
            </div>
            <span class="text-xl font-bold text-white">منصة الفن</span>
          </div>
          <p class="text-gray-400 leading-relaxed max-w-sm">
            منصة عربية متخصصة للفنون البصرية والتعليم الإبداعي. استكشف أعمال الفنانين وتعلم مهاراتك من أفضل الأساتذة.
          </p>
          <div class="flex gap-4 mt-6">
            <a v-for="social in socials" :key="social.name" href="#"
              class="w-10 h-10 rounded-lg bg-dark-200 border border-dark-300 flex items-center justify-center text-gray-400 hover:text-gold hover:border-gold/40 transition-all duration-300">
              {{ social.icon }}
            </a>
          </div>
        </div>

        <!-- Links -->
        <div v-for="section in footerLinks" :key="section.title">
          <h4 class="text-white font-semibold mb-4">{{ section.title }}</h4>
          <ul class="space-y-3">
            <li v-for="link in section.links" :key="link.to">
              <RouterLink :to="link.to" class="text-gray-400 hover:text-gold transition-colors text-sm">
                {{ link.label }}
              </RouterLink>
            </li>
          </ul>
        </div>
      </div>

      <div class="border-t border-dark-300 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-sm text-gray-500">
        <p>© {{ new Date().getFullYear() }} منصة الفن. جميع الحقوق محفوظة.</p>
        <div class="flex gap-6">
          <RouterLink to="/privacy" class="hover:text-gray-300 transition-colors">سياسة الخصوصية</RouterLink>
          <RouterLink to="/terms" class="hover:text-gray-300 transition-colors">شروط الاستخدام</RouterLink>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
const socials = [
  { name: 'instagram', icon: '📷' },
  { name: 'twitter', icon: '𝕏' },
  { name: 'youtube', icon: '▶' },
  { name: 'behance', icon: 'Be' },
]

const footerLinks = [
  {
    title: 'الموقع',
    links: [
      { to: '/portfolio', label: 'المعرض الفني' },
      { to: '/courses', label: 'الدورات التعليمية' },
      { to: '/blog', label: 'المدونة' },
      { to: '/about', label: 'عن الفنان' },
    ]
  },
  {
    title: 'الدعم',
    links: [
      { to: '/contact', label: 'تواصل معنا' },
      { to: '/faq', label: 'الأسئلة الشائعة' },
      { to: '/login', label: 'تسجيل الدخول' },
      { to: '/register', label: 'إنشاء حساب' },
    ]
  },
]
</script>
