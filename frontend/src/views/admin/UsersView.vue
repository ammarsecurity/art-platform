<template>
  <div>
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-white">المستخدمون</h1>
        <p class="text-gray-400 mt-1">إدارة حسابات المستخدمين وأدوارهم</p>
      </div>
      <span class="text-gray-400 text-sm">{{ totalCount }} مستخدم</span>
    </div>

    <!-- Search -->
    <div class="card p-4 mb-6">
      <input v-model="search" @input="debouncedFetch" type="text" class="input-field"
        placeholder="بحث بالاسم أو رقم الهاتف...">
    </div>

    <!-- Table -->
    <div class="card overflow-hidden">
      <div v-if="loading" class="p-8 text-center text-gray-500">جارٍ التحميل...</div>

      <div v-else-if="users.length === 0" class="p-10 text-center text-gray-500">
        <div class="text-4xl mb-3">👥</div>
        <p>لا توجد نتائج</p>
      </div>

      <table v-else class="w-full">
        <thead>
          <tr class="border-b border-dark-300 text-right">
            <th class="p-4 text-sm text-gray-400 font-medium">المستخدم</th>
            <th class="p-4 text-sm text-gray-400 font-medium hidden md:table-cell">الهاتف</th>
            <th class="p-4 text-sm text-gray-400 font-medium hidden lg:table-cell">الدورات</th>
            <th class="p-4 text-sm text-gray-400 font-medium">الدور</th>
            <th class="p-4 text-sm text-gray-400 font-medium">الحالة</th>
            <th class="p-4 text-sm text-gray-400 font-medium">إجراءات</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-dark-300">
          <tr v-for="user in users" :key="user.id" class="hover:bg-dark-300/30 transition-colors">
            <td class="p-4">
              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-full bg-gold/20 text-gold flex items-center justify-center text-sm font-bold shrink-0">
                  {{ user.name.charAt(0) }}
                </div>
                <div>
                  <p class="text-white text-sm font-medium">{{ user.name }}</p>
                  <p class="text-gray-500 text-xs hidden sm:block">{{ formatDate(user.createdAt) }}</p>
                </div>
              </div>
            </td>
            <td class="p-4 text-gray-300 text-sm hidden md:table-cell" dir="ltr">{{ user.phone }}</td>
            <td class="p-4 text-gray-400 text-sm hidden lg:table-cell">{{ user.enrollmentCount || 0 }} دورة</td>
            <td class="p-4">
              <select :value="user.role" @change="handleRoleChange(user, $event.target.value)"
                class="text-xs bg-dark-300 text-white rounded-lg px-2 py-1.5 border-0 outline-none cursor-pointer">
                <option value="Student">طالب</option>
                <option value="Admin">أدمن</option>
              </select>
            </td>
            <td class="p-4">
              <span class="text-xs px-2 py-1 rounded-full"
                :class="user.isActive ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'">
                {{ user.isActive ? 'نشط' : 'معطّل' }}
              </span>
            </td>
            <td class="p-4">
              <button @click="handleToggleStatus(user)"
                class="text-xs px-3 py-1.5 rounded-lg transition-colors"
                :class="user.isActive ? 'bg-red-500/20 hover:bg-red-500/30 text-red-400' : 'bg-green-500/20 hover:bg-green-500/30 text-green-400'">
                {{ user.isActive ? 'تعطيل' : 'تفعيل' }}
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="p-4 border-t border-dark-300 flex items-center justify-between">
        <button :disabled="page === 1" @click="page--; fetchUsers()"
          class="px-4 py-2 text-sm bg-dark-300 hover:bg-dark-200 text-white rounded-lg transition-colors disabled:opacity-40">السابق</button>
        <span class="text-gray-400 text-sm">{{ page }} / {{ totalPages }}</span>
        <button :disabled="page === totalPages" @click="page++; fetchUsers()"
          class="px-4 py-2 text-sm bg-dark-300 hover:bg-dark-200 text-white rounded-lg transition-colors disabled:opacity-40">التالي</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { userApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const users = ref([])
const loading = ref(false)
const search = ref('')
const page = ref(1)
const totalCount = ref(0)
const pageSize = 20
const totalPages = computed(() => Math.ceil(totalCount.value / pageSize))

let debounceTimer = null
function debouncedFetch() {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => { page.value = 1; fetchUsers() }, 400)
}

onMounted(fetchUsers)

async function fetchUsers() {
  loading.value = true
  try {
    const res = await userApi.getAll({ page: page.value, pageSize, search: search.value || undefined })
    users.value = res.data?.items || []
    totalCount.value = res.data?.totalCount || 0
  } catch {
    // backend offline
  } finally {
    loading.value = false
  }
}

async function handleRoleChange(user, newRole) {
  if (newRole === user.role) return
  try {
    const res = await userApi.updateRole(user.id, newRole)
    user.role = res.data?.role || newRole
    toast.success('تم تحديث الدور')
  } catch { /* handled by interceptor */ }
}

async function handleToggleStatus(user) {
  const action = user.isActive ? 'تعطيل' : 'تفعيل'
  if (!confirm(`${action} حساب ${user.name}؟`)) return
  try {
    const res = await userApi.toggleStatus(user.id)
    user.isActive = res.data?.isActive ?? !user.isActive
    toast.success(res.message || `تم ${action} الحساب`)
  } catch { /* handled by interceptor */ }
}

function formatDate(dateStr) {
  return new Date(dateStr).toLocaleDateString('ar-SA', { year: 'numeric', month: 'short', day: 'numeric' })
}
</script>
