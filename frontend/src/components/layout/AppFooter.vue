<template>
  <footer class="bg-surface border-t border-line mt-20">
    <div class="max-w-7xl mx-auto px-4 py-16">
      <div class="flex flex-col lg:flex-row flex-wrap gap-10 lg:gap-12 mb-12">
        <!-- Brand -->
        <div class="w-full lg:max-w-md lg:flex-shrink-0">
          <div class="flex items-center gap-3 mb-4">
            <SiteLogoMark box-class="w-10 h-10 rounded-xl" text-class="text-lg" :letter="ft.brandLetter" />
            <span class="text-xl font-bold text-fg">{{ ft.brandTitle }}</span>
          </div>
          <p class="text-fg-mute leading-relaxed max-w-sm">
            {{ ft.brandDescription }}
          </p>
          <div class="flex gap-4 mt-6 flex-wrap">
            <a
              v-for="(social, i) in ft.socialIcons"
              :key="i"
              :href="socialHref(social.url)"
              :target="isExternal(social.url) ? '_blank' : '_self'"
              :rel="isExternal(social.url) ? 'noopener noreferrer' : undefined"
              class="w-10 h-10 rounded-lg bg-input border border-line flex items-center justify-center text-fg-mute hover:text-gold hover:border-gold/40 transition-all duration-300"
              :title="social.name || ''"
            >
              {{ social.icon }}
            </a>
          </div>
        </div>

        <!-- Link columns -->
        <div
          v-for="(section, si) in ft.columns"
          :key="si"
          class="min-w-[10rem] flex-1"
        >
          <h4 class="text-fg font-semibold mb-4">{{ section.title }}</h4>
          <ul class="space-y-3">
            <li v-for="(link, li) in section.links" :key="li">
              <RouterLink
                v-if="!isExternal(link.href)"
                :to="link.href || '/'"
                class="text-fg-mute hover:text-gold transition-colors text-sm"
              >
                {{ link.label }}
              </RouterLink>
              <a
                v-else
                :href="link.href"
                target="_blank"
                rel="noopener noreferrer"
                class="text-fg-mute hover:text-gold transition-colors text-sm"
              >
                {{ link.label }}
              </a>
            </li>
          </ul>
        </div>
      </div>

      <div class="border-t border-line pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-sm text-fg-dim">
        <p>© {{ year }} {{ ft.copyrightLine }}</p>
        <div class="flex flex-wrap gap-6 justify-center">
          <template v-for="(leg, i) in ft.legalLinks" :key="i">
            <RouterLink
              v-if="!isExternal(leg.href)"
              :to="leg.href || '/'"
              class="hover:text-fg-soft transition-colors"
            >
              {{ leg.label }}
            </RouterLink>
            <a
              v-else
              :href="leg.href"
              target="_blank"
              rel="noopener noreferrer"
              class="hover:text-fg-soft transition-colors"
            >
              {{ leg.label }}
            </a>
          </template>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import SiteLogoMark from './SiteLogoMark.vue'

const site = useSiteSettingsStore()
const year = new Date().getFullYear()

function fallbackFooter() {
  return {
    brandLetter: 'ف',
    brandTitle: 'منصة الفن',
    brandDescription:
      'منصة عربية متخصصة للفنون البصرية والتعليم الإبداعي. استكشف أعمال الفنانين وتعلم مهاراتك من أفضل الأساتذة.',
    socialIcons: [
      { icon: '📷', url: 'https://instagram.com', name: 'Instagram' },
      { icon: '𝕏', url: 'https://twitter.com', name: 'Twitter' },
      { icon: '▶', url: 'https://youtube.com', name: 'YouTube' },
      { icon: 'Be', url: 'https://behance.net', name: 'Behance' }
    ],
    columns: [
      {
        title: 'الموقع',
        links: [
          { label: 'المعرض الفني', href: '/portfolio' },
          { label: 'الدورات التعليمية', href: '/courses' },
          { label: 'المدونة', href: '/blog' },
          { label: 'عن الفنان', href: '/about' }
        ]
      },
      {
        title: 'الدعم',
        links: [
          { label: 'تواصل معنا', href: '/contact' },
          { label: 'الأسئلة الشائعة', href: '/faq' },
          { label: 'تسجيل الدخول', href: '/login' },
          { label: 'إنشاء حساب', href: '/register' }
        ]
      }
    ],
    copyrightLine: 'منصة الفن. جميع الحقوق محفوظة.',
    legalLinks: [
      { label: 'سياسة الخصوصية', href: '/privacy' },
      { label: 'شروط الاستخدام', href: '/terms' }
    ]
  }
}

const ft = computed(() => {
  const f = site.data?.footer
  const d = fallbackFooter()
  if (!f) return d
  return {
    ...d,
    ...f,
    socialIcons: Array.isArray(f.socialIcons) && f.socialIcons.length ? f.socialIcons : d.socialIcons,
    columns: Array.isArray(f.columns) && f.columns.length ? f.columns : d.columns,
    legalLinks: Array.isArray(f.legalLinks) && f.legalLinks.length ? f.legalLinks : d.legalLinks
  }
})

function isExternal(href) {
  return /^https?:\/\//i.test(String(href || '').trim())
}

function socialHref(url) {
  const u = String(url || '').trim()
  return u || '#'
}

onMounted(() => site.load())
</script>
