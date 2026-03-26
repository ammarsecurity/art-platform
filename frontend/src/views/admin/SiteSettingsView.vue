<template>
  <div>
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-white">إعدادات الموقع</h1>
      <p class="text-gray-400 mt-1">تحكم في جميع محتويات الموقع من مكان واحد</p>
    </div>

    <div v-if="loading" class="space-y-4">
      <div v-for="i in 8" :key="i" class="card h-16 animate-pulse"></div>
    </div>

    <div v-else>
      <!-- Tabs -->
      <div class="flex gap-2 mb-8 border-b border-dark-300">
        <button v-for="tab in tabs" :key="tab.id"
          @click="activeTab = tab.id"
          class="px-5 py-3 text-sm font-medium transition-all border-b-2 -mb-px"
          :class="activeTab === tab.id
            ? 'border-gold text-gold'
            : 'border-transparent text-gray-400 hover:text-white'">
          {{ tab.icon }} {{ tab.label }}
        </button>
      </div>

      <!-- Tab: General -->
      <form v-if="activeTab === 'general'" @submit.prevent="save('general')" class="space-y-6">
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">🌐</span> معلومات الموقع
          </h2>
          <div class="grid md:grid-cols-2 gap-5">
            <div class="md:col-span-2">
              <label class="block text-sm text-gray-400 mb-2">اسم الموقع</label>
              <input v-model="fields.site_name" type="text" class="input-field">
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm text-gray-400 mb-2">وصف الموقع</label>
              <input v-model="fields.site_description" type="text" class="input-field">
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm text-gray-400 mb-2">وصف الفوتر</label>
              <textarea v-model="fields.footer_description" class="textarea-field" rows="3"></textarea>
            </div>
          </div>
        </div>
        <SaveButton :saving="saving" />
      </form>

      <!-- Tab: Home Page -->
      <form v-if="activeTab === 'home'" @submit.prevent="save('home')" class="space-y-6">
        <!-- Hero -->
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">🎯</span> قسم البانر الرئيسي
          </h2>
          <div class="space-y-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">نص الشارة (Badge)</label>
              <input v-model="fields.hero_badge" type="text" class="input-field" placeholder="✨ منصة مرتضى ثامر العربية الأولى">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">عنوان البانر</label>
              <input v-model="fields.hero_title" type="text" class="input-field">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">وصف البانر</label>
              <input v-model="fields.hero_subtitle" type="text" class="input-field">
            </div>
          </div>
        </div>

        <!-- Stats -->
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">📊</span> الإحصائيات
          </h2>
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">عدد الأعمال</label>
              <input v-model="fields.stat_artworks" type="text" class="input-field" dir="ltr" placeholder="+200">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">عدد الدورات</label>
              <input v-model="fields.stat_courses" type="text" class="input-field" dir="ltr" placeholder="+50">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">عدد الطلاب</label>
              <input v-model="fields.stat_students" type="text" class="input-field" dir="ltr" placeholder="+5000">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">سنوات الخبرة</label>
              <input v-model="fields.stat_experience" type="text" class="input-field" dir="ltr" placeholder="15+">
            </div>
          </div>
        </div>

        <!-- About snippet on home -->
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">👨‍🎨</span> قسم عن الفنان (الرئيسية)
          </h2>
          <div class="space-y-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">الفقرة الأولى</label>
              <textarea v-model="fields.home_about_bio_1" class="textarea-field" rows="3"></textarea>
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">الفقرة الثانية</label>
              <textarea v-model="fields.home_about_bio_2" class="textarea-field" rows="2"></textarea>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm text-gray-400 mb-2">قيمة سنوات الخبرة</label>
                <input v-model="fields.home_about_experience" type="text" class="input-field" dir="ltr" placeholder="+15">
              </div>
              <div>
                <label class="block text-sm text-gray-400 mb-2">رابط صورة الفنان</label>
                <input v-model="fields.artist_image_url" type="url" class="input-field" dir="ltr">
              </div>
            </div>
          </div>
        </div>

        <!-- CTA -->
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">📣</span> قسم CTA (دعوة للتسجيل)
          </h2>
          <div class="space-y-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">العنوان</label>
              <input v-model="fields.cta_title" type="text" class="input-field">
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">الوصف</label>
              <input v-model="fields.cta_subtitle" type="text" class="input-field">
            </div>
          </div>
        </div>

        <SaveButton :saving="saving" />
      </form>

      <!-- Tab: About Page -->
      <form v-if="activeTab === 'about'" @submit.prevent="save('about')" class="space-y-6">
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">📝</span> نبذة عن الفنان
          </h2>
          <div class="space-y-4">
            <div>
              <label class="block text-sm text-gray-400 mb-2">الفقرة الأولى</label>
              <textarea v-model="fields.about_bio_1" class="textarea-field" rows="3"></textarea>
            </div>
            <div>
              <label class="block text-sm text-gray-400 mb-2">الفقرة الثانية</label>
              <textarea v-model="fields.about_bio_2" class="textarea-field" rows="3"></textarea>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm text-gray-400 mb-2">سنوات الخبرة</label>
                <input v-model="fields.about_experience" type="text" class="input-field" dir="ltr" placeholder="15+">
              </div>
            </div>
          </div>
        </div>

        <!-- Skills -->
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">⚡</span> المهارات
          </h2>
          <div class="space-y-3">
            <div v-for="(skill, idx) in skillsArr" :key="idx" class="flex items-center gap-3">
              <input v-model="skill.icon" type="text" class="input-field w-16 text-center" placeholder="🖌️">
              <input v-model="skill.name" type="text" class="input-field flex-1" placeholder="اسم المهارة">
              <input v-model.number="skill.level" type="number" min="0" max="100" class="input-field w-24" dir="ltr" placeholder="95">
              <span class="text-gray-400 text-sm w-8">%</span>
              <button type="button" @click="removeSkill(idx)"
                class="text-red-400 hover:text-red-300 text-sm px-2 py-1 rounded">✕</button>
            </div>
            <button type="button" @click="addSkill"
              class="text-gold hover:text-gold-light text-sm mt-2">+ إضافة مهارة</button>
          </div>
        </div>

        <!-- Awards -->
        <div class="card p-6">
          <h2 class="text-white font-bold text-lg mb-5 flex items-center gap-2">
            <span class="text-gold">🏆</span> الجوائز والإنجازات
          </h2>
          <div class="space-y-3">
            <div v-for="(award, idx) in awardsArr" :key="idx" class="flex items-center gap-3">
              <input v-model="award.title" type="text" class="input-field flex-1" placeholder="عنوان الجائزة">
              <input v-model="award.org" type="text" class="input-field flex-1" placeholder="الجهة">
              <input v-model="award.year" type="text" class="input-field w-24" dir="ltr" placeholder="2024">
              <button type="button" @click="removeAward(idx)"
                class="text-red-400 hover:text-red-300 text-sm px-2 py-1 rounded">✕</button>
            </div>
            <button type="button" @click="addAward"
              class="text-gold hover:text-gold-light text-sm mt-2">+ إضافة جائزة</button>
          </div>
        </div>

        <SaveButton :saving="saving" />
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { toast } from 'vue3-toastify'
import api from '@/services/api'
import { invalidateSettingsCache } from '@/composables/useSiteSettings'

// Inline SaveButton to keep it simple
const SaveButton = {
  props: ['saving'],
  template: `
    <div class="flex justify-end">
      <button type="submit" :disabled="saving" class="btn-primary px-8 py-3">
        <span v-if="saving" class="inline-block w-4 h-4 border-2 border-dark/30 border-t-dark rounded-full animate-spin ml-2"></span>
        {{ saving ? 'جارٍ الحفظ...' : 'حفظ التغييرات' }}
      </button>
    </div>
  `
}

const loading = ref(true)
const saving = ref(false)
const activeTab = ref('general')

const tabs = [
  { id: 'general', icon: '🌐', label: 'عام' },
  { id: 'home', icon: '🏠', label: 'الرئيسية' },
  { id: 'about', icon: '👨‍🎨', label: 'عن الفنان' },
]

const fields = reactive({
  site_name: '', site_description: '', footer_description: '',
  hero_badge: '', hero_title: '', hero_subtitle: '',
  stat_artworks: '', stat_courses: '', stat_students: '', stat_experience: '',
  cta_title: '', cta_subtitle: '',
  home_about_bio_1: '', home_about_bio_2: '', home_about_experience: '', artist_image_url: '',
  about_bio_1: '', about_bio_2: '', about_experience: '',
  about_skills: '', about_awards: '',
})

const skillsArr = ref([])
const awardsArr = ref([])

const tabKeys = {
  general: ['site_name', 'site_description', 'footer_description'],
  home: ['hero_badge', 'hero_title', 'hero_subtitle', 'stat_artworks', 'stat_courses', 'stat_students', 'stat_experience', 'cta_title', 'cta_subtitle', 'home_about_bio_1', 'home_about_bio_2', 'home_about_experience', 'artist_image_url'],
  about: ['about_bio_1', 'about_bio_2', 'about_experience', 'about_skills', 'about_awards'],
}

onMounted(async () => {
  try {
    const res = await api.get('/settings')
    const data = res.data || {}
    Object.keys(fields).forEach(k => { if (data[k] !== undefined) fields[k] = data[k] })
    skillsArr.value = parseJson(fields.about_skills, [
      { icon: '🖌️', name: 'رسم زيتي', level: 95 },
      { icon: '💻', name: 'فن رقمي', level: 88 },
      { icon: '📷', name: 'تصوير', level: 75 },
      { icon: '✏️', name: 'رسم يدوي', level: 92 },
    ])
    awardsArr.value = parseJson(fields.about_awards, [
      { title: 'أفضل فنان عربي', org: 'مهرجان الفنون العربية', year: '2024' },
      { title: 'جائزة الإبداع', org: 'وزارة الثقافة', year: '2023' },
      { title: 'المعلم المميز', org: 'منصة تعليمية دولية', year: '2022' },
    ])
  } catch { }
  finally { loading.value = false }
})

function parseJson(str, fallback) {
  try { return str ? JSON.parse(str) : fallback } catch { return fallback }
}

function addSkill() { skillsArr.value.push({ icon: '🎨', name: '', level: 80 }) }
function removeSkill(i) { skillsArr.value.splice(i, 1) }
function addAward() { awardsArr.value.push({ title: '', org: '', year: '' }) }
function removeAward(i) { awardsArr.value.splice(i, 1) }

async function save(tab) {
  // Sync JSON arrays back to fields before saving
  if (tab === 'about') {
    fields.about_skills = JSON.stringify(skillsArr.value)
    fields.about_awards = JSON.stringify(awardsArr.value)
  }

  const keys = tabKeys[tab]
  const payload = {}
  keys.forEach(k => { payload[k] = fields[k] })

  saving.value = true
  try {
    await api.put('/settings/bulk', payload)
    invalidateSettingsCache()
    toast.success('تم حفظ الإعدادات بنجاح')
  } catch {
    toast.error('فشل الحفظ، تأكد من تشغيل الباك إيند')
  } finally { saving.value = false }
}
</script>
