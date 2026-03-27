<template>
  <div>
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-fg">المستخدمون</h1>
      <p class="text-fg-mute mt-1">عرض وإدارة جميع الحسابات المسجّلة</p>
    </div>

    <div v-if="loading" class="card p-12 text-center text-fg-dim">
      جارٍ التحميل...
    </div>

    <div v-else-if="error" class="card p-8 text-center text-red-400">
      {{ error }}
    </div>

    <div v-else class="card overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b border-line text-fg-mute text-right">
              <th class="py-4 px-4 font-medium">#</th>
              <th class="py-4 px-4 font-medium">الاسم</th>
              <th class="py-4 px-4 font-medium">الجوال</th>
              <th class="py-4 px-4 font-medium">الدور</th>
              <th class="py-4 px-4 font-medium">الحالة</th>
              <th class="py-4 px-4 font-medium">تاريخ التسجيل</th>
              <th class="py-4 px-4 font-medium w-44">إجراءات</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="u in users"
              :key="u.id"
              class="border-b border-line/60 hover:bg-input/40 transition-colors"
            >
              <td class="py-3 px-4 text-fg-dim">{{ u.id }}</td>
              <td class="py-3 px-4 text-fg font-medium">{{ u.name }}</td>
              <td class="py-3 px-4 text-fg-soft" dir="ltr">{{ u.phone }}</td>
              <td class="py-3 px-4">
                <span
                  class="text-xs px-2 py-1 rounded-md border"
                  :class="u.role === 'Admin' ? 'bg-gold/15 text-gold border-gold/30' : 'bg-line/50 text-fg-soft border-line'"
                >
                  {{ roleLabel(u.role) }}
                </span>
              </td>
              <td class="py-3 px-4">
                <span :class="u.isActive ? 'text-green-400' : 'text-red-400'">
                  {{ u.isActive ? 'نشط' : 'معطّل' }}
                </span>
              </td>
              <td class="py-3 px-4 text-fg-mute">{{ formatDate(u.createdAt) }}</td>
              <td class="py-3 px-4">
                <div class="flex flex-wrap gap-1 justify-end">
                  <button
                    type="button"
                    class="text-xs px-2 py-1 rounded border border-line text-fg-soft hover:text-fg hover:border-gold/40"
                    @click="openEdit(u)"
                  >
                    تعديل
                  </button>
                  <button
                    type="button"
                    class="text-xs px-2 py-1 rounded border border-line text-fg-soft hover:text-amber-200 hover:border-amber-500/40"
                    @click="openPassword(u)"
                  >
                    كلمة المرور
                  </button>
                  <button
                    type="button"
                    class="text-xs px-2 py-1 rounded border border-red-500/40 text-red-400/90 hover:bg-red-500/10 disabled:opacity-40 disabled:cursor-not-allowed"
                    :disabled="u.id === currentUserId"
                    :title="u.id === currentUserId ? 'لا يمكن حذف حسابك الحالي' : ''"
                    @click="confirmDelete(u)"
                  >
                    حذف
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <p v-if="!users.length" class="p-8 text-center text-fg-dim">لا يوجد مستخدمون</p>
    </div>

    <Teleport to="body">
      <div
        v-if="showEditModal"
        class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4"
        @click.self="showEditModal = false"
      >
        <div class="card p-8 max-w-md w-full max-h-[90vh] overflow-y-auto">
          <h3 class="text-xl font-bold text-fg mb-6">تعديل المستخدم</h3>
          <form class="space-y-4" @submit.prevent="saveEdit">
            <div>
              <label class="block text-sm text-fg-mute mb-2">الاسم *</label>
              <input v-model="editForm.name" type="text" class="input-field" required minlength="2" maxlength="100">
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">الجوال *</label>
              <input v-model="editForm.phone" type="tel" class="input-field" dir="ltr" required>
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">الدور *</label>
              <select v-model="editForm.role" class="input-field">
                <option value="Student">طالب</option>
                <option value="Admin">مدير</option>
              </select>
            </div>
            <div class="flex items-center gap-2">
              <input id="edit-active" v-model="editForm.isActive" type="checkbox" class="rounded border-line">
              <label for="edit-active" class="text-sm text-fg-soft">الحساب نشط</label>
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">نبذة</label>
              <textarea v-model="editForm.bio" class="textarea-field" rows="3" maxlength="500" placeholder="اختياري"></textarea>
            </div>
            <div class="flex gap-3 pt-2">
              <button type="button" class="btn-ghost flex-1 justify-center border border-line rounded-lg py-3" @click="showEditModal = false">
                إلغاء
              </button>
              <button type="submit" class="btn-primary flex-1 justify-center" :disabled="savingEdit">
                {{ savingEdit ? 'جارٍ الحفظ...' : 'حفظ' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>

    <Teleport to="body">
      <div
        v-if="showPasswordModal"
        class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4"
        @click.self="showPasswordModal = false"
      >
        <div class="card p-8 max-w-md w-full">
          <h3 class="text-xl font-bold text-fg mb-2">تغيير كلمة المرور</h3>
          <p class="text-sm text-fg-dim mb-6" v-if="passwordTarget">
            للمستخدم: <span class="text-fg-soft">{{ passwordTarget.name }}</span>
          </p>
          <form class="space-y-4" @submit.prevent="savePassword">
            <div>
              <label class="block text-sm text-fg-mute mb-2">كلمة المرور الجديدة *</label>
              <input v-model="passwordForm.newPassword" type="password" class="input-field" autocomplete="new-password" minlength="8" required>
              <p class="text-xs text-fg-dim mt-1">8 أحرف على الأقل</p>
            </div>
            <div>
              <label class="block text-sm text-fg-mute mb-2">تأكيد كلمة المرور *</label>
              <input v-model="passwordForm.confirm" type="password" class="input-field" autocomplete="new-password" minlength="8" required>
            </div>
            <div class="flex gap-3 pt-2">
              <button type="button" class="btn-ghost flex-1 justify-center border border-line rounded-lg py-3" @click="showPasswordModal = false">
                إلغاء
              </button>
              <button type="submit" class="btn-primary flex-1 justify-center" :disabled="savingPassword">
                {{ savingPassword ? 'جارٍ الحفظ...' : 'حفظ' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>

    <ConfirmDialog
      v-model="showDeleteDialog"
      title="تأكيد حذف المستخدم"
      :loading="deleting"
      loading-label="جارٍ الحذف..."
      @confirm="handleDelete"
    >
      هل أنت متأكد من حذف المستخدم
      <span class="text-fg font-medium">{{ deleteTarget?.name }}</span>
      ؟ سيتم حذف بياناته المرتبطة حسب إعدادات النظام.
    </ConfirmDialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { adminApi } from '@/services/api'
import { useAuthStore } from '@/stores/auth'
import { toast } from 'vue3-toastify'
import ConfirmDialog from '@/components/ui/ConfirmDialog.vue'

const auth = useAuthStore()
const loading = ref(true)
const error = ref('')
const users = ref([])

const currentUserId = computed(() => auth.user?.id ?? null)

const showEditModal = ref(false)
const editTarget = ref(null)
const savingEdit = ref(false)
const editForm = reactive({
  name: '',
  phone: '',
  role: 'Student',
  isActive: true,
  bio: '',
})

const showPasswordModal = ref(false)
const passwordTarget = ref(null)
const savingPassword = ref(false)
const passwordForm = reactive({ newPassword: '', confirm: '' })

const showDeleteDialog = ref(false)
const deleteTarget = ref(null)
const deleting = ref(false)

function roleLabel(role) {
  if (role === 'Admin') return 'مدير'
  if (role === 'Student') return 'طالب'
  return role || '—'
}

function formatDate(d) {
  if (!d) return '—'
  try {
    return new Date(d).toLocaleDateString('ar-SA', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  } catch {
    return '—'
  }
}

async function loadUsers() {
  loading.value = true
  error.value = ''
  try {
    const res = await adminApi.getUsers()
    users.value = Array.isArray(res.data) ? res.data : []
  } catch {
    error.value = 'تعذر تحميل قائمة المستخدمين'
    users.value = []
  } finally {
    loading.value = false
  }
}

function openEdit(u) {
  editTarget.value = u
  editForm.name = u.name
  editForm.phone = u.phone
  editForm.role = u.role === 'Admin' ? 'Admin' : 'Student'
  editForm.isActive = !!u.isActive
  editForm.bio = u.bio ?? ''
  showEditModal.value = true
}

async function saveEdit() {
  if (!editTarget.value) return
  savingEdit.value = true
  try {
    await adminApi.updateUser(editTarget.value.id, {
      name: editForm.name.trim(),
      phone: editForm.phone.trim(),
      role: editForm.role,
      isActive: editForm.isActive,
      bio: editForm.bio?.trim() || null,
    })
    toast.success('تم تحديث المستخدم')
    showEditModal.value = false
    await loadUsers()
    if (editTarget.value.id === currentUserId.value) {
      await auth.init()
    }
  } catch {
    // رسالة الخطأ من interceptor (ما عدا 422 و 500)
  } finally {
    savingEdit.value = false
  }
}

function openPassword(u) {
  passwordTarget.value = u
  passwordForm.newPassword = ''
  passwordForm.confirm = ''
  showPasswordModal.value = true
}

async function savePassword() {
  if (!passwordTarget.value) return
  if (passwordForm.newPassword !== passwordForm.confirm) {
    toast.error('كلمتا المرور غير متطابقتين')
    return
  }
  savingPassword.value = true
  try {
    await adminApi.resetUserPassword(passwordTarget.value.id, {
      newPassword: passwordForm.newPassword,
    })
    toast.success('تم تغيير كلمة المرور')
    showPasswordModal.value = false
  } catch {
    // رسالة الخطأ من interceptor
  } finally {
    savingPassword.value = false
  }
}

function confirmDelete(u) {
  if (u.id === currentUserId.value) return
  deleteTarget.value = u
  showDeleteDialog.value = true
}

async function handleDelete() {
  if (!deleteTarget.value) return
  deleting.value = true
  try {
    await adminApi.deleteUser(deleteTarget.value.id)
    toast.success('تم حذف المستخدم')
    showDeleteDialog.value = false
    deleteTarget.value = null
    await loadUsers()
  } catch {
    // رسالة الخطأ من interceptor
  } finally {
    deleting.value = false
  }
}

onMounted(async () => {
  if (!auth.user) {
    await auth.init()
  }
  await loadUsers()
})
</script>
