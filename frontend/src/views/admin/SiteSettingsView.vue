<template>
  <div class="w-full max-w-none space-y-8">
    <div>
      <h1 class="text-3xl font-bold text-fg">إعدادات الموقع</h1>
      <p class="text-fg-mute mt-1">الاسم، سلايدر الصور، البانر، آراء الطلاب — تنعكس على الموقع العام بعد الحفظ.</p>
    </div>

    <div v-if="loading" class="card p-12 text-center text-fg-dim">جارٍ التحميل...</div>

    <template v-else>
      <!-- عام -->
      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">عام</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">اسم الموقع</label>
          <input v-model="form.siteName" type="text" class="input-field">
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">شعار الموقع</label>
          <p class="text-xs text-fg-dim mb-3 leading-relaxed">
            يظهر في الشريط العلوي، لوحة التحكم، تسجيل الدخول والتسجيل، صفحة مشاهدة الدروس، والفوتر. إن لم ترفع صورة يُعرض حرف الشعار من إعدادات الفوتر بلون ذهبي بدون خلفية.
          </p>
          <div class="flex flex-wrap items-start gap-4">
            <div
              class="relative w-20 h-20 rounded-xl overflow-hidden border border-dashed border-line bg-transparent flex items-center justify-center flex-shrink-0"
            >
              <img
                v-if="siteLogoDisplay"
                :src="siteLogoDisplay"
                alt=""
                class="w-full h-full object-contain p-1"
              >
              <span v-else class="text-gold text-2xl font-bold">ف</span>
              <div
                v-if="siteLogoUploading"
                class="absolute inset-0 bg-page/70 flex items-center justify-center text-xs text-fg-soft"
              >
                جارٍ الرفع...
              </div>
            </div>
            <div class="flex flex-col gap-2 min-w-0">
              <div class="flex flex-wrap gap-2">
                <button type="button" class="btn-outline text-sm py-2" :disabled="siteLogoUploading" @click="openSiteLogoUpload">
                  رفع صورة
                </button>
                <button
                  v-if="form.siteLogoUrl"
                  type="button"
                  class="text-sm text-red-400 hover:text-red-300 py-2"
                  :disabled="siteLogoUploading"
                  @click="clearSiteLogo"
                >
                  إزالة الشعار
                </button>
              </div>
              <p class="text-xs text-fg-dim">PNG أو JPG أو WebP — يُفضّل مربع أو شفاف الخلفية.</p>
            </div>
          </div>
          <input
            ref="siteLogoInput"
            type="file"
            class="hidden"
            accept="image/jpeg,image/png,image/webp,image/gif"
            @change="handleSiteLogoChange"
          >
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">وصف الموقع (وصف مختصر)</label>
          <input v-model="form.siteDescription" type="text" class="input-field">
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">بريد التواصل</label>
          <input v-model="form.contactEmail" type="email" class="input-field" dir="ltr">
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">عنوان SEO (hero_title)</label>
            <input v-model="form.heroTitle" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">وصف SEO (hero_subtitle)</label>
            <input v-model="form.heroSubtitle" type="text" class="input-field">
          </div>
        </div>
      </div>

      <!-- سلايدر الصفحة الرئيسية -->
      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">سلايدر الصفحة الرئيسية</h2>
        <p class="text-sm text-fg-dim leading-relaxed">
          صور عريضة في أعلى الصفحة مع انتقال تلقائي وأسهم ونقاط. اترك «عنوان الشريحة» فارغاً لعرض نص البانر (العنوانان الافتراضيان) فوق الصور.
        </p>
        <label class="flex items-center gap-3 cursor-pointer">
          <input v-model="home.slider.enabled" type="checkbox" class="rounded border-line text-gold focus:ring-gold">
          <span class="text-fg-soft">تفعيل السلايدر</span>
        </label>
        <div>
          <label class="block text-sm text-fg-mute mb-2">مدة كل شريحة (ثوانٍ، بين 3 و 120)</label>
          <input
            v-model.number="home.slider.intervalSeconds"
            type="number"
            min="3"
            max="120"
            class="input-field max-w-xs"
          >
        </div>
        <div class="flex flex-wrap justify-between items-center gap-2">
          <span class="text-fg-mute text-sm">الشرائح</span>
          <button type="button" class="btn-outline text-sm py-1" @click="addSliderSlide">+ شريحة</button>
        </div>
        <div v-for="(slide, i) in home.slider.items" :key="i" class="card p-4 bg-input/40 border border-line space-y-3">
          <div class="flex flex-wrap justify-between items-center gap-2">
            <span class="text-fg-dim text-sm">شريحة {{ i + 1 }}</span>
            <div class="flex flex-wrap gap-2">
              <button type="button" class="text-fg-mute text-sm disabled:opacity-0" :disabled="i === 0" @click="moveSliderSlide(i, -1)">↑</button>
              <button type="button" class="text-fg-mute text-sm disabled:opacity-0" :disabled="i === home.slider.items.length - 1" @click="moveSliderSlide(i, 1)">↓</button>
              <button type="button" class="text-red-400 text-sm" @click="removeSliderSlide(i)">حذف</button>
            </div>
          </div>
          <div class="flex flex-col sm:flex-row gap-4">
            <div
              class="relative shrink-0 w-full sm:w-40 h-28 rounded-lg overflow-hidden border border-line bg-input cursor-pointer group"
              @click="!sliderImageUploading && openSliderUpload(i)"
            >
              <img
                v-if="slidePreview(slide)"
                :src="slidePreview(slide)"
                alt=""
                class="w-full h-full object-cover"
              >
              <div v-else class="flex items-center justify-center h-full text-fg-dim text-xs px-2 text-center">
                رفع صورة
              </div>
              <div
                v-if="sliderImageUploading && sliderUploadIndex === i"
                class="absolute inset-0 bg-page/70 flex items-center justify-center text-xs text-fg-soft"
              >
                جارٍ الرفع...
              </div>
            </div>
            <div class="flex-1 space-y-2 min-w-0">
              <div>
                <label class="block text-xs text-fg-dim mb-1">عنوان الشريحة (اختياري)</label>
                <input v-model="slide.title" type="text" class="input-field py-2 text-sm" placeholder="يظهر كعنوان كبير؛ إن ترك فارغاً يُستخدم نص البانر">
              </div>
              <div>
                <label class="block text-xs text-fg-dim mb-1">نص فرعي</label>
                <input v-model="slide.subtitle" type="text" class="input-field py-2 text-sm">
              </div>
              <div class="grid sm:grid-cols-2 gap-2">
                <div>
                  <label class="block text-xs text-fg-dim mb-1">رابط الزر (مثل /courses)</label>
                  <input v-model="slide.linkUrl" type="text" class="input-field py-2 text-sm" dir="ltr" placeholder="/portfolio">
                </div>
                <div>
                  <label class="block text-xs text-fg-dim mb-1">نص الزر</label>
                  <input v-model="slide.linkLabel" type="text" class="input-field py-2 text-sm" placeholder="اكتشف المزيد">
                </div>
              </div>
            </div>
          </div>
        </div>
        <input
          ref="sliderImageInput"
          type="file"
          class="hidden"
          accept="image/jpeg,image/png,image/webp,image/gif"
          @change="handleSliderImageChange"
        >
      </div>

      <!-- البانر -->
      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">الصفحة الرئيسية — البانر</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">شارة صغيرة فوق العنوان</label>
          <input v-model="home.heroBadge" type="text" class="input-field">
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">سطر العنوان الأول</label>
            <input v-model="home.heroTitleLine1" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">سطر العنوان الثاني (مميز)</label>
            <input v-model="home.heroTitleLine2" type="text" class="input-field">
          </div>
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">فقرة تحت العنوان</label>
          <textarea v-model="home.heroParagraph" class="textarea-field" rows="3"></textarea>
        </div>
      </div>

      <!-- إحصائيات -->
      <div class="card p-6 space-y-4">
        <div class="flex items-center justify-between border-b border-line pb-2">
          <h2 class="text-lg font-semibold text-fg">أرقام الإحصائيات (تحت البانر)</h2>
          <button type="button" class="btn-outline text-sm py-1" @click="addStat">+ صف</button>
        </div>
        <div v-for="(s, i) in home.stats" :key="i" class="flex items-end gap-2 flex-wrap">
          <div class="flex-1 min-w-[6rem]">
            <label class="block text-xs text-fg-dim mb-1">القيمة</label>
            <input v-model="s.value" type="text" class="input-field py-2 text-sm">
          </div>
          <div class="flex-1 min-w-[6rem]">
            <label class="block text-xs text-fg-dim mb-1">التسمية</label>
            <input v-model="s.label" type="text" class="input-field py-2 text-sm">
          </div>
          <button type="button" class="text-red-400 text-sm px-2 py-2" @click="removeStat(i)">حذف</button>
        </div>
      </div>

      <!-- عن الفنان -->
      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">قسم «عن الفنان»</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">صورة الجانب</label>
          <div
            class="relative border-2 border-dashed border-line rounded-xl p-6 text-center transition-colors max-w-md"
            :class="[
              aboutImageDisplay ? 'p-0 border-0 overflow-hidden' : '',
              aboutImageUploading ? 'cursor-wait' : 'cursor-pointer hover:border-gold/40'
            ]"
            @click="!aboutImageUploading && aboutImageInput?.click()"
          >
            <template v-if="aboutImageDisplay">
              <img
                :src="aboutImageDisplay"
                class="w-full aspect-square object-cover max-h-80"
                alt=""
              >
              <div
                v-if="aboutImageUploading"
                class="absolute inset-0 flex items-center justify-center bg-page/70 text-fg-soft text-sm"
              >
                جارٍ الرفع...
              </div>
            </template>
            <div v-else class="py-4">
              <div class="text-4xl mb-2">🖼️</div>
              <p class="text-fg-mute text-sm">اضغط لرفع صورة القسم</p>
              <p class="text-fg-dim text-xs mt-1">JPG, PNG, WebP, GIF — حتى 10 ميجابايت</p>
            </div>
          </div>
          <input
            ref="aboutImageInput"
            type="file"
            class="hidden"
            accept="image/jpeg,image/png,image/webp,image/gif"
            @change="handleAboutImageChange"
          >
          <div v-if="home.about.imageUrl && !aboutImageUploading" class="mt-2">
            <button type="button" class="text-red-400 text-sm hover:text-red-300" @click.stop="clearAboutImage">
              إزالة الصورة
            </button>
          </div>
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">شارة القسم</label>
          <input v-model="home.about.badge" type="text" class="input-field">
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">عنوان — سطر 1</label>
            <input v-model="home.about.titleLine1" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">عنوان — سطر 2 (مميز)</label>
            <input v-model="home.about.titleLine2" type="text" class="input-field">
          </div>
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">فقرة 1</label>
          <textarea v-model="home.about.paragraph1" class="textarea-field" rows="3"></textarea>
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">فقرة 2</label>
          <textarea v-model="home.about.paragraph2" class="textarea-field" rows="3"></textarea>
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">بطاقة عائمة — رقم</label>
            <input v-model="home.about.cardValue" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">بطاقة عائمة — نص</label>
            <input v-model="home.about.cardLabel" type="text" class="input-field">
          </div>
        </div>
      </div>

      <!-- آراء الطلاب -->
      <div class="card p-6 space-y-4">
        <div class="flex items-center justify-between border-b border-line pb-2">
          <h2 class="text-lg font-semibold text-fg">آراء الطلاب</h2>
          <button type="button" class="btn-outline text-sm py-1" @click="addTestimonial">+ رأي</button>
        </div>
        <div v-for="(t, i) in home.testimonials" :key="i" class="card p-4 bg-input/40 space-y-3 border border-line">
          <div class="flex justify-between items-center">
            <span class="text-fg-dim text-sm">رأي {{ i + 1 }}</span>
            <button type="button" class="text-red-400 text-sm" @click="removeTestimonial(i)">حذف</button>
          </div>
          <div>
            <label class="block text-xs text-fg-dim mb-1">الاسم</label>
            <input v-model="t.name" type="text" class="input-field py-2 text-sm">
          </div>
          <div>
            <label class="block text-xs text-fg-dim mb-1">الصفة (مثلاً طالب)</label>
            <input v-model="t.title" type="text" class="input-field py-2 text-sm">
          </div>
          <div>
            <label class="block text-xs text-fg-dim mb-1">نص الرأي</label>
            <textarea v-model="t.text" class="textarea-field text-sm" rows="3"></textarea>
          </div>
        </div>
      </div>

      <!-- CTA -->
      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">دعوة التسجيل (أسفل الصفحة)</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">العنوان</label>
          <input v-model="home.cta.title" type="text" class="input-field">
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">النص الفرعي</label>
          <textarea v-model="home.cta.subtitle" class="textarea-field" rows="2"></textarea>
        </div>
      </div>

      <div class="flex items-center gap-4">
        <button type="button" class="btn-primary" :disabled="saving" @click="handleSave">
          {{ saving ? 'جارٍ الحفظ...' : 'حفظ كل الإعدادات' }}
        </button>
        <a href="/" target="_blank" class="text-gold text-sm hover:underline">معاينة الموقع ←</a>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { settingsApi } from '@/services/api'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import { asciiFilenameForUpload } from '@/utils/safeUploadFile'
import { resolveMediaUrl } from '@/utils/mediaUrl'
import { toast } from 'vue3-toastify'

const loading = ref(true)
const saving = ref(false)
const aboutImageInput = ref(null)
const aboutImageUploading = ref(false)
const aboutImageBlob = ref(null)
const sliderImageInput = ref(null)
const sliderUploadIndex = ref(null)
const sliderImageUploading = ref(false)
const siteStore = useSiteSettingsStore()

const form = reactive({
  siteName: '',
  siteDescription: '',
  contactEmail: '',
  heroTitle: '',
  heroSubtitle: '',
  siteLogoUrl: ''
})

const siteLogoInput = ref(null)
const siteLogoUploading = ref(false)

const siteLogoDisplay = computed(() => {
  const u = form.siteLogoUrl?.trim()
  return u ? resolveMediaUrl(u) : ''
})

function emptyAbout() {
  return {
    imageUrl: '',
    badge: '👨‍🎨 عن الفنان',
    titleLine1: 'رحلة إبداعية',
    titleLine2: 'لا حدود لها',
    paragraph1: '',
    paragraph2: '',
    cardValue: '+15',
    cardLabel: 'سنة خبرة في الفنون'
  }
}

function emptySlider() {
  return {
    enabled: true,
    intervalSeconds: 8,
    items: []
  }
}

function emptyHome() {
  return {
    heroBadge: '✨ منصة الفن العربية الأولى',
    heroTitleLine1: 'استكشف عالم',
    heroTitleLine2: 'الفن الإبداعي',
    heroParagraph: 'تعلم، استلهم، وأبدع مع أفضل الفنانين العرب.',
    stats: [
      { value: '+200', label: 'عمل فني' },
      { value: '+50', label: 'دورة تعليمية' },
      { value: '+5000', label: 'طالب' },
      { value: '15+', label: 'سنة خبرة' }
    ],
    about: emptyAbout(),
    testimonials: [
      { name: 'أحمد', title: 'طالب', text: 'منصة رائعة!' },
      { name: 'سارة', title: 'مصممة', text: 'دورات ممتازة.' },
      { name: 'محمد', title: 'فنان', text: 'معرض ملهم.' }
    ],
    cta: { title: 'ابدأ رحلتك الإبداعية اليوم', subtitle: 'انضم لآلاف الطلاب وتعلم الفن من أفضل الأساتذة العرب' },
    slider: emptySlider()
  }
}

const home = reactive(emptyHome())

const aboutImageDisplay = computed(() => {
  if (aboutImageBlob.value) return aboutImageBlob.value
  const u = home.about?.imageUrl
  return u ? resolveMediaUrl(u) : ''
})

function revokeAboutBlob() {
  if (aboutImageBlob.value) {
    URL.revokeObjectURL(aboutImageBlob.value)
    aboutImageBlob.value = null
  }
}

async function handleAboutImageChange(e) {
  const file = e.target.files?.[0]
  e.target.value = ''
  if (!file) return
  if (!file.type.startsWith('image/')) {
    toast.error('يرجى اختيار ملف صورة')
    return
  }
  if (file.size > 10 * 1024 * 1024) {
    toast.error('حجم الصورة يتجاوز 10 ميجابايت')
    return
  }
  revokeAboutBlob()
  aboutImageBlob.value = URL.createObjectURL(file)
  aboutImageUploading.value = true
  try {
    const fd = new FormData()
    fd.append('file', file, asciiFilenameForUpload(file))
    const res = await settingsApi.uploadAboutImage(fd)
    const url = res.data?.url
    if (!url) {
      toast.error('لم يُرجع الخادم رابط الصورة')
      return
    }
    home.about.imageUrl = url
    revokeAboutBlob()
    toast.success('تم رفع الصورة — اضغط «حفظ كل الإعدادات» لتثبيتها')
  } catch {
    toast.error('تعذر رفع الصورة')
    revokeAboutBlob()
  } finally {
    aboutImageUploading.value = false
  }
}

function clearAboutImage() {
  revokeAboutBlob()
  home.about.imageUrl = ''
}

function openSiteLogoUpload() {
  siteLogoInput.value?.click()
}

async function handleSiteLogoChange(e) {
  const file = e.target.files?.[0]
  e.target.value = ''
  if (!file) return
  if (!file.type.startsWith('image/')) {
    toast.error('يرجى اختيار ملف صورة')
    return
  }
  if (file.size > 10 * 1024 * 1024) {
    toast.error('حجم الصورة يتجاوز 10 ميجابايت')
    return
  }
  siteLogoUploading.value = true
  try {
    const fd = new FormData()
    fd.append('file', file, asciiFilenameForUpload(file))
    const res = await settingsApi.uploadAboutImage(fd)
    const url = res.data?.url
    if (!url) {
      toast.error('لم يُرجع الخادم رابط الصورة')
      return
    }
    form.siteLogoUrl = url
    toast.success('تم رفع الشعار — اضغط «حفظ كل الإعدادات» لتثبيته')
  } catch {
    toast.error('تعذر رفع الصورة')
  } finally {
    siteLogoUploading.value = false
  }
}

function clearSiteLogo() {
  form.siteLogoUrl = ''
}

function slidePreview(slide) {
  const u = slide?.imageUrl
  return u ? resolveMediaUrl(u) : ''
}

function openSliderUpload(i) {
  sliderUploadIndex.value = i
  sliderImageInput.value?.click()
}

async function handleSliderImageChange(e) {
  const file = e.target.files?.[0]
  e.target.value = ''
  const i = sliderUploadIndex.value
  sliderUploadIndex.value = null
  if (file == null || i === null || typeof i !== 'number') return
  if (!file.type.startsWith('image/')) {
    toast.error('يرجى اختيار ملف صورة')
    return
  }
  if (file.size > 10 * 1024 * 1024) {
    toast.error('حجم الصورة يتجاوز 10 ميجابايت')
    return
  }
  sliderImageUploading.value = true
  try {
    const fd = new FormData()
    fd.append('file', file, asciiFilenameForUpload(file))
    const res = await settingsApi.uploadAboutImage(fd)
    const url = res.data?.url
    if (!url) {
      toast.error('لم يُرجع الخادم رابط الصورة')
      return
    }
    if (!home.slider.items[i]) return
    home.slider.items[i].imageUrl = url
    toast.success('تم رفع صورة الشريحة')
  } catch {
    toast.error('تعذر رفع الصورة')
  } finally {
    sliderImageUploading.value = false
  }
}

function addSliderSlide() {
  home.slider.items.push({
    imageUrl: '',
    title: '',
    subtitle: '',
    linkUrl: '',
    linkLabel: 'اكتشف المزيد'
  })
}

function removeSliderSlide(i) {
  home.slider.items.splice(i, 1)
}

function moveSliderSlide(i, delta) {
  const j = i + delta
  const arr = home.slider.items
  if (j < 0 || j >= arr.length) return
  const t = arr[i]
  arr[i] = arr[j]
  arr[j] = t
}

onMounted(async () => {
  const def = emptyHome()
  try {
    const res = await settingsApi.getPublic()
    const d = res.data
    form.siteName = d.siteName ?? ''
    form.siteDescription = d.siteDescription ?? ''
    form.contactEmail = d.contactEmail ?? ''
    form.heroTitle = d.heroTitle ?? ''
    form.heroSubtitle = d.heroSubtitle ?? ''
    form.siteLogoUrl = d.siteLogoUrl ?? ''
    Object.assign(home, def)
    const hp = d.homePage
    if (hp) {
      Object.assign(home, hp)
      home.about = { ...emptyAbout(), ...(hp.about || {}) }
      home.cta = { ...def.cta, ...(hp.cta || {}) }
      if (Array.isArray(hp.stats) && hp.stats.length)
        home.stats = hp.stats.map(s => ({ value: String(s.value ?? ''), label: String(s.label ?? '') }))
      if (Array.isArray(hp.testimonials) && hp.testimonials.length)
        home.testimonials = hp.testimonials.map(t => ({
          name: String(t.name ?? ''),
          title: String(t.title ?? ''),
          text: String(t.text ?? '')
        }))
      home.slider = {
        enabled: hp.slider?.enabled !== false,
        intervalSeconds: Math.min(120, Math.max(3, Number(hp.slider?.intervalSeconds) || 8)),
        items: Array.isArray(hp.slider?.items)
          ? hp.slider.items.map(s => ({
              imageUrl: String(s.imageUrl ?? ''),
              title: String(s.title ?? ''),
              subtitle: String(s.subtitle ?? ''),
              linkUrl: String(s.linkUrl ?? ''),
              linkLabel: String(s.linkLabel ?? 'اكتشف المزيد')
            }))
          : []
      }
    }
  } catch {
    toast.error('تعذر تحميل الإعدادات')
  } finally {
    loading.value = false
  }
})

function addStat() {
  home.stats.push({ value: '', label: '' })
}

function removeStat(i) {
  if (home.stats.length > 1) home.stats.splice(i, 1)
}

function addTestimonial() {
  home.testimonials.push({ name: '', title: '', text: '' })
}

function removeTestimonial(i) {
  if (home.testimonials.length > 1) home.testimonials.splice(i, 1)
}

async function handleSave() {
  saving.value = true
  try {
    const payload = {
      site_name: form.siteName,
      site_description: form.siteDescription,
      contact_email: form.contactEmail,
      hero_title: form.heroTitle,
      hero_subtitle: form.heroSubtitle,
      site_logo_url: form.siteLogoUrl?.trim() || '',
      home_page_config: JSON.stringify(home)
    }
    await settingsApi.updateBatch(payload)
    toast.success('تم حفظ الإعدادات')
    await siteStore.refresh()
  } catch {
    toast.error('تعذر الحفظ')
  } finally {
    saving.value = false
  }
}
</script>
