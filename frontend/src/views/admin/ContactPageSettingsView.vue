<template>
  <div class="w-full max-w-none space-y-8">
    <div>
      <h1 class="text-3xl font-bold text-fg">صفحة التواصل</h1>
      <p class="text-fg-mute mt-1">تعديل النصوص، الحقول، خيارات الموضوع، معلومات التواصل، وروابط السوشيال — تنعكس على /contact بعد الحفظ.</p>
    </div>

    <div v-if="loading" class="card p-12 text-center text-fg-dim">جارٍ التحميل...</div>

    <template v-else>
      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">رأس الصفحة</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">الشارة (فوق العنوان)</label>
          <input v-model="contact.headerBadge" type="text" class="input-field">
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">العنوان الرئيسي</label>
          <input v-model="contact.headerTitle" type="text" class="input-field">
        </div>
        <div>
          <label class="block text-sm text-fg-mute mb-2">النص التوضيحي</label>
          <textarea v-model="contact.headerSubtitle" class="textarea-field" rows="2"></textarea>
        </div>
      </div>

      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">نموذج الإرسال</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">عنوان البطاقة</label>
          <input v-model="contact.formCardTitle" type="text" class="input-field">
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">تسمية الاسم</label>
            <input v-model="contact.labelName" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">تسمية الجوال</label>
            <input v-model="contact.labelPhone" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">تسمية البريد</label>
            <input v-model="contact.labelEmail" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">تسمية الموضوع</label>
            <input v-model="contact.labelSubject" type="text" class="input-field">
          </div>
          <div class="sm:col-span-2">
            <label class="block text-sm text-fg-mute mb-2">تسمية الرسالة</label>
            <input v-model="contact.labelMessage" type="text" class="input-field">
          </div>
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">نص توضيحي — الاسم</label>
            <input v-model="contact.placeholderName" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">نص توضيحي — الجوال</label>
            <input v-model="contact.placeholderPhone" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">نص توضيحي — البريد</label>
            <input v-model="contact.placeholderEmail" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">نص القائمة — اختر الموضوع</label>
            <input v-model="contact.subjectOptionPlaceholder" type="text" class="input-field">
          </div>
          <div class="sm:col-span-2">
            <label class="block text-sm text-fg-mute mb-2">نص توضيحي — الرسالة</label>
            <input v-model="contact.placeholderMessage" type="text" class="input-field">
          </div>
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm text-fg-mute mb-2">نص زر الإرسال</label>
            <input v-model="contact.submitButtonText" type="text" class="input-field">
          </div>
          <div>
            <label class="block text-sm text-fg-mute mb-2">نص أثناء الإرسال</label>
            <input v-model="contact.submitSendingText" type="text" class="input-field">
          </div>
          <div class="sm:col-span-2">
            <label class="block text-sm text-fg-mute mb-2">رسالة نجاح بعد الإرسال</label>
            <input v-model="contact.toastSuccessMessage" type="text" class="input-field">
          </div>
        </div>
        <div>
          <div class="flex justify-between items-center mb-2">
            <span class="text-sm text-fg-mute">خيارات الموضوع (في القائمة)</span>
            <button type="button" class="btn-outline text-sm py-1" @click="addSubjectOption">+ خيار</button>
          </div>
          <div v-for="(opt, i) in contact.subjectOptions" :key="i" class="flex gap-2 mb-2">
            <input v-model="contact.subjectOptions[i]" type="text" class="input-field py-2 text-sm flex-1">
            <button type="button" class="text-red-400 text-sm px-2 py-2" @click="removeSubjectOption(i)">حذف</button>
          </div>
        </div>
      </div>

      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">معلومات التواصل (العمود الأيمن)</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">عنوان القسم</label>
          <input v-model="contact.infoSectionTitle" type="text" class="input-field">
        </div>
        <div class="flex justify-end">
          <button type="button" class="btn-outline text-sm py-1" @click="addInfoRow">+ صف</button>
        </div>
        <div v-for="(row, i) in contact.infoRows" :key="i" class="card p-4 bg-input/40 border border-line space-y-2">
          <div class="flex justify-between items-center">
            <span class="text-fg-dim text-sm">صف {{ i + 1 }}</span>
            <button type="button" class="text-red-400 text-sm" @click="removeInfoRow(i)">حذف</button>
          </div>
          <div class="grid sm:grid-cols-3 gap-2">
            <input v-model="row.icon" type="text" class="input-field py-2 text-sm" placeholder="أيقونة">
            <input v-model="row.label" type="text" class="input-field py-2 text-sm sm:col-span-2" placeholder="التسمية">
          </div>
          <input v-model="row.value" type="text" class="input-field py-2 text-sm" placeholder="القيمة المعروضة">
        </div>
      </div>

      <div class="card p-6 space-y-4">
        <h2 class="text-lg font-semibold text-fg border-b border-line pb-2">روابط التواصل الاجتماعي</h2>
        <div>
          <label class="block text-sm text-fg-mute mb-2">عنوان القسم</label>
          <input v-model="contact.socialSectionTitle" type="text" class="input-field">
        </div>
        <div class="flex justify-end">
          <button type="button" class="btn-outline text-sm py-1" @click="addSocialLink">+ رابط</button>
        </div>
        <div v-for="(s, i) in contact.socialLinks" :key="i" class="card p-4 bg-input/40 border border-line space-y-2">
          <div class="flex justify-between items-center">
            <span class="text-fg-dim text-sm">رابط {{ i + 1 }}</span>
            <button type="button" class="text-red-400 text-sm" @click="removeSocialLink(i)">حذف</button>
          </div>
          <div class="grid sm:grid-cols-3 gap-2">
            <input v-model="s.name" type="text" class="input-field py-2 text-sm" placeholder="الاسم">
            <input v-model="s.icon" type="text" class="input-field py-2 text-sm" placeholder="أيقونة">
            <input v-model="s.url" type="text" class="input-field py-2 text-sm" placeholder="https://..." dir="ltr">
          </div>
        </div>
      </div>

      <div class="flex items-center gap-4">
        <button type="button" class="btn-primary" :disabled="saving" @click="handleSave">
          {{ saving ? 'جارٍ الحفظ...' : 'حفظ صفحة التواصل' }}
        </button>
        <a href="/contact" target="_blank" class="text-gold text-sm hover:underline">معاينة الصفحة ←</a>
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

function emptyContact() {
  return {
    headerBadge: '📬 تواصل معنا',
    headerTitle: 'نسعد بتواصلك',
    headerSubtitle: 'هل لديك استفسار أو مشروع تريد التعاون فيه؟ أرسل لنا رسالتك',
    formCardTitle: 'أرسل رسالة',
    labelName: 'الاسم الكامل *',
    labelPhone: 'رقم الجوال',
    labelEmail: 'البريد الإلكتروني *',
    labelSubject: 'الموضوع *',
    labelMessage: 'الرسالة *',
    placeholderName: 'محمد العمري',
    placeholderPhone: '+966 5x xxx xxxx',
    placeholderEmail: 'example@email.com',
    placeholderMessage: 'اكتب رسالتك هنا...',
    subjectOptionPlaceholder: 'اختر موضوع الرسالة',
    subjectOptions: ['استفسار عن الدورات', 'طلب تعاون', 'شراء عمل فني', 'أخرى'],
    submitButtonText: 'إرسال الرسالة 📨',
    submitSendingText: 'جارٍ الإرسال...',
    toastSuccessMessage: 'تم إرسال رسالتك بنجاح! سنتواصل معك قريباً 😊',
    infoSectionTitle: 'معلومات التواصل',
    infoRows: [
      { icon: '📧', label: 'البريد الإلكتروني', value: 'info@artplatform.com' },
      { icon: '📱', label: 'رقم الجوال', value: '+966 50 000 0000' },
      { icon: '🕐', label: 'ساعات العمل', value: 'السبت - الخميس، 9 ص - 6 م' },
      { icon: '📍', label: 'الموقع', value: 'الرياض، المملكة العربية السعودية' }
    ],
    socialSectionTitle: 'تابعني على منصات التواصل',
    socialLinks: [
      { name: 'Instagram', icon: '📷', url: 'https://instagram.com' },
      { name: 'Twitter', icon: '𝕏', url: 'https://twitter.com' },
      { name: 'YouTube', icon: '▶', url: 'https://youtube.com' }
    ]
  }
}

const contact = reactive(emptyContact())

onMounted(async () => {
  const def = emptyContact()
  try {
    const res = await settingsApi.getPublic()
    const d = res.data
    const cp = d.contactPage
    if (cp) {
      Object.assign(contact, def, cp)
      contact.subjectOptions = Array.isArray(cp.subjectOptions) && cp.subjectOptions.length
        ? cp.subjectOptions.map(String)
        : [...def.subjectOptions]
      contact.infoRows = Array.isArray(cp.infoRows) && cp.infoRows.length
        ? cp.infoRows.map((r) => ({
            icon: String(r.icon ?? ''),
            label: String(r.label ?? ''),
            value: String(r.value ?? '')
          }))
        : def.infoRows.map((r) => ({ ...r }))
      contact.socialLinks = Array.isArray(cp.socialLinks) && cp.socialLinks.length
        ? cp.socialLinks.map((s) => ({
            name: String(s.name ?? ''),
            icon: String(s.icon ?? ''),
            url: String(s.url ?? '')
          }))
        : def.socialLinks.map((s) => ({ ...s }))
    }
  } catch {
    toast.error('تعذر تحميل الإعدادات')
  } finally {
    loading.value = false
  }
})

function addSubjectOption() {
  contact.subjectOptions.push('')
}

function removeSubjectOption(i) {
  if (contact.subjectOptions.length > 1) contact.subjectOptions.splice(i, 1)
}

function addInfoRow() {
  contact.infoRows.push({ icon: '📌', label: '', value: '' })
}

function removeInfoRow(i) {
  if (contact.infoRows.length > 1) contact.infoRows.splice(i, 1)
}

function addSocialLink() {
  contact.socialLinks.push({ name: '', icon: '🔗', url: '' })
}

function removeSocialLink(i) {
  if (contact.socialLinks.length > 1) contact.socialLinks.splice(i, 1)
}

async function handleSave() {
  saving.value = true
  try {
    const payload = {
      contact_page_config: JSON.stringify(contact)
    }
    await settingsApi.updateBatch(payload)
    toast.success('تم حفظ صفحة التواصل')
    await siteStore.refresh()
  } catch {
    toast.error('تعذر الحفظ')
  } finally {
    saving.value = false
  }
}
</script>
