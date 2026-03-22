<template>
  <div>
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-white">رسائل التواصل</h1>
      <p class="text-gray-400 mt-1">{{ unreadCount }} رسالة غير مقروءة</p>
    </div>

    <div class="grid lg:grid-cols-3 gap-6 h-[calc(100vh-200px)]">
      <!-- Messages list -->
      <div class="card overflow-hidden flex flex-col">
        <div class="p-4 border-b border-dark-300 flex items-center justify-between">
          <h3 class="text-white font-semibold">الرسائل</h3>
          <select v-model="filter" @change="fetchMessages()" class="input-field py-1 text-sm w-32">
            <option value="all">الكل</option>
            <option value="unread">غير مقروء</option>
          </select>
        </div>
        <div class="flex-1 overflow-y-auto">
          <div v-if="loading" class="p-6 text-center text-gray-500">جارٍ التحميل...</div>
          <div v-for="msg in messages" :key="msg.id"
            @click="selectMessage(msg)"
            class="p-4 border-b border-dark-300 cursor-pointer transition-colors hover:bg-dark-200"
            :class="selected?.id === msg.id ? 'bg-dark-200 border-r-2 border-r-gold' : ''">
            <div class="flex items-start justify-between mb-2">
              <div class="flex items-center gap-2">
                <div class="w-8 h-8 rounded-full bg-gold/20 flex items-center justify-center text-gold text-xs font-bold flex-shrink-0">
                  {{ msg.name.charAt(0) }}
                </div>
                <div>
                  <p class="text-white text-sm font-medium flex items-center gap-2">
                    {{ msg.name }}
                    <span v-if="!msg.isRead" class="w-2 h-2 rounded-full bg-gold"></span>
                  </p>
                  <p class="text-gray-500 text-xs">{{ msg.email }}</p>
                </div>
              </div>
              <span class="text-gray-500 text-xs">{{ formatDate(msg.createdAt) }}</span>
            </div>
            <p class="text-gray-400 text-xs font-medium truncate">{{ msg.subject }}</p>
          </div>
        </div>
      </div>

      <!-- Message detail -->
      <div class="lg:col-span-2 card overflow-hidden flex flex-col">
        <div v-if="!selected" class="flex-1 flex items-center justify-center text-gray-500">
          <div class="text-center">
            <div class="text-5xl mb-3">📨</div>
            <p>اختر رسالة لعرضها</p>
          </div>
        </div>
        <template v-else>
          <div class="p-6 border-b border-dark-300 flex items-center justify-between">
            <div>
              <h3 class="text-white font-bold text-lg">{{ selected.subject }}</h3>
              <p class="text-gray-400 text-sm mt-1">من: {{ selected.name }} — {{ selected.email }}</p>
              <p v-if="selected.phone" class="text-gray-400 text-sm">الجوال: {{ selected.phone }}</p>
            </div>
            <div class="flex items-center gap-3">
              <span class="text-gray-500 text-xs">{{ formatDate(selected.createdAt) }}</span>
              <button @click="handleDelete(selected.id)"
                class="px-3 py-1.5 text-xs bg-red-500/20 text-red-400 rounded-lg hover:bg-red-500/30 transition-colors">حذف</button>
            </div>
          </div>
          <div class="flex-1 overflow-y-auto p-6">
            <div class="card-glass p-6 rounded-2xl">
              <p class="text-gray-300 leading-relaxed text-base whitespace-pre-wrap">{{ selected.message }}</p>
            </div>
          </div>
          <div class="p-4 border-t border-dark-300">
            <a :href="`mailto:${selected.email}?subject=رداً على: ${selected.subject}`"
              class="btn-primary w-full justify-center">
              📧 الرد عبر البريد الإلكتروني
            </a>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { contactApi } from '@/services/api'
import { toast } from 'vue3-toastify'

const messages = ref([])
const selected = ref(null)
const loading = ref(false)
const filter = ref('all')

const unreadCount = computed(() => messages.value.filter(m => !m.isRead).length)

onMounted(fetchMessages)

async function fetchMessages() {
  loading.value = true
  try {
    const res = await contactApi.getAll({ page: 1, pageSize: 50 })
    messages.value = res.data.items
  } finally { loading.value = false }
}

async function selectMessage(msg) {
  selected.value = msg
  if (!msg.isRead) {
    await contactApi.markRead(msg.id)
    msg.isRead = true
  }
}

async function handleDelete(id) {
  await contactApi.delete(id)
  toast.success('تم حذف الرسالة')
  if (selected.value?.id === id) selected.value = null
  messages.value = messages.value.filter(m => m.id !== id)
}

function formatDate(d) {
  return new Date(d).toLocaleDateString('ar-SA', { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' })
}
</script>
