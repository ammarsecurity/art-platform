<template>
  <div class="w-full max-w-none space-y-8">
    <div>
      <h1 class="text-3xl font-bold text-fg">الفوتر</h1>
      <p class="text-fg-mute mt-1">العلامة، الوصف، روابط السوشيال، أعمدة الروابط، حقوق النشر والروابط القانونية — تظهر في أسفل كل الصفحات العامة بعد الحفظ.</p>
    </div>

    <div v-if="loading" class="card p-12 text-center text-fg-dim">جارٍ التحميل...</div>

    <template v-else>
      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">العلامة والوصف</h2>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">حرف الشعار (بدون صورة مرفوعة)</label>
            <input v-model="footer.brandLetter" type="text" maxlength="4" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">اسم الموقع بجانب الشعار</label>
            <input v-model="footer.brandTitle" type="text" class="input-field">
          </div>
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">الوصف</label>
          <textarea v-model="footer.brandDescription" class="textarea-field" rows="3"></textarea>
        </div>
      </div>

      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">أيقونات السوشيال</h2>
        <div class="flex justify-end">
          <button type="button" class="btn-outline text-sm py-1" @click="addSocial">+ رابط</button>
        </div>
        <div v-for="(s, i) in footer.socialIcons" :key="i" class="card p-4 bg-input/40 border border-line flex flex-wrap gap-2 items-end">
          <div class="flex-1 min-w-[4rem]">
            <label class="block text-xs text-fg-dim mb-1">أيقونة</label>
            <input v-model="s.icon" type="text" class="input-field py-2 text-sm">
          </div>
          <div class="flex-[2] min-w-[8rem]">
            <label class="block text-xs text-fg-dim mb-1">الرابط</label>
            <input v-model="s.url" type="text" class="input-field py-2 text-sm" dir="ltr" placeholder="https://">
          </div>
          <div class="flex-1 min-w-[6rem]">
            <label class="block text-xs text-fg-dim mb-1">اسم (اختياري)</label>
            <input v-model="s.name" type="text" class="input-field py-2 text-sm">
          </div>
          <button type="button" class="text-red-400 text-sm mb-2" @click="removeSocial(i)">حذف</button>
        </div>
      </div>

      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">أعمدة الروابط</h2>
        <div class="flex justify-end">
          <button type="button" class="btn-outline text-sm py-1" @click="addColumn">+ عمود</button>
        </div>
        <div v-for="(col, ci) in footer.columns" :key="ci" class="card p-4 bg-input/40 border border-line space-y-3">
          <div class="flex justify-between items-center">
            <input v-model="col.title" type="text" class="input-field py-2 text-sm font-semibold max-w-xs" placeholder="عنوان العمود">
            <button type="button" class="text-red-400 text-sm" @click="removeColumn(ci)">حذف العمود</button>
          </div>
          <div class="flex justify-end">
            <button type="button" class="btn-outline text-xs py-1" @click="addLink(ci)">+ رابط</button>
          </div>
          <div v-for="(link, li) in col.links" :key="li" class="flex flex-wrap gap-2 items-end">
            <input v-model="link.label" type="text" class="input-field py-2 text-sm flex-1 min-w-[6rem]" placeholder="النص">
            <input v-model="link.href" type="text" class="input-field py-2 text-sm flex-1 min-w-[8rem]" dir="ltr" placeholder="/contact أو https://">
            <button type="button" class="text-red-400 text-sm" @click="removeLink(ci, li)">حذف</button>
          </div>
        </div>
      </div>

      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">الشريط السفلي</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">حقوق النشر (يُسبق تلقائياً بـ © والسنة)</label>
          <input v-model="footer.copyrightLine" type="text" class="input-field" placeholder="منصة الفن. جميع الحقوق محفوظة.">
        </div>
        <div>
          <div class="flex justify-between items-center mb-2">
            <span class="text-sm text-fg-mute">روابط قانونية (بجانب حقوق النشر)</span>
            <button type="button" class="btn-outline text-sm py-1" @click="addLegal">+ رابط</button>
          </div>
          <div v-for="(l, i) in footer.legalLinks" :key="i" class="flex gap-2 mb-2">
            <input v-model="l.label" type="text" class="input-field py-2 text-sm flex-1" placeholder="النص">
            <input v-model="l.href" type="text" class="input-field py-2 text-sm flex-1" dir="ltr" placeholder="/privacy">
            <button type="button" class="text-red-400 text-sm px-2" @click="removeLegal(i)">حذف</button>
          </div>
        </div>
      </div>

      <div class="flex items-center gap-4">
        <button type="button" class="btn-primary" :disabled="saving" @click="handleSave">
          {{ saving ? 'جارٍ الحفظ...' : 'حفظ الفوتر' }}
        </button>
        <a href="/" target="_blank" class="text-gold text-sm hover:underline">معاينة الموقع ←</a>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { settingsApi } from '@/services/api'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import { toast } from 'vue3-toastify'

const loading = ref(true)
const saving = ref(false)
const siteStore = useSiteSettingsStore()

function emptyFooter() {
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

const footer = reactive(emptyFooter())

onMounted(async () => {
  const def = emptyFooter()
  try {
    const res = await settingsApi.getPublic()
    const f = res.data?.footer
    if (f) {
      Object.assign(footer, def, f)
      footer.socialIcons = Array.isArray(f.socialIcons) && f.socialIcons.length
        ? f.socialIcons.map(s => ({
            icon: String(s.icon ?? ''),
            url: String(s.url ?? ''),
            name: String(s.name ?? '')
          }))
        : def.socialIcons.map(s => ({ ...s }))
      footer.columns = Array.isArray(f.columns) && f.columns.length
        ? f.columns.map(c => ({
            title: String(c.title ?? ''),
            links: Array.isArray(c.links)
              ? c.links.map(l => ({ label: String(l.label ?? ''), href: String(l.href ?? '') }))
              : []
          }))
        : def.columns.map(c => ({ title: c.title, links: c.links.map(l => ({ ...l })) }))
      footer.legalLinks = Array.isArray(f.legalLinks) && f.legalLinks.length
        ? f.legalLinks.map(l => ({ label: String(l.label ?? ''), href: String(l.href ?? '') }))
        : def.legalLinks.map(l => ({ ...l }))
    }
  } catch {
    toast.error('تعذر تحميل إعدادات الفوتر')
  } finally {
    loading.value = false
  }
})

function addSocial() {
  footer.socialIcons.push({ icon: '🔗', url: '', name: '' })
}

function removeSocial(i) {
  if (footer.socialIcons.length > 1) footer.socialIcons.splice(i, 1)
}

function addColumn() {
  footer.columns.push({ title: 'عنوان', links: [{ label: '', href: '/' }] })
}

function removeColumn(i) {
  if (footer.columns.length > 1) footer.columns.splice(i, 1)
}

function addLink(ci) {
  footer.columns[ci].links.push({ label: '', href: '/' })
}

function removeLink(ci, li) {
  const links = footer.columns[ci].links
  if (links.length > 1) links.splice(li, 1)
}

function addLegal() {
  footer.legalLinks.push({ label: '', href: '/' })
}

function removeLegal(i) {
  if (footer.legalLinks.length > 1) footer.legalLinks.splice(i, 1)
}

async function handleSave() {
  saving.value = true
  try {
    await settingsApi.updateBatch({ footer_config: JSON.stringify(footer) })
    toast.success('تم حفظ الفوتر')
    await siteStore.refresh()
  } catch {
    toast.error('تعذر الحفظ')
  } finally {
    saving.value = false
  }
}
</script>
