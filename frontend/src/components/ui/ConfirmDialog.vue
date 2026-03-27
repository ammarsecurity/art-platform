<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="modelValue"
        class="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100] flex items-center justify-center p-4"
        role="dialog"
        aria-modal="true"
        @click.self="onCancel"
      >
        <div
          class="card p-8 max-w-md w-full shadow-2xl border border-line/50 scale-100 transition-transform"
          @click.stop
        >
          <h3 class="text-xl font-bold text-fg mb-3">{{ title }}</h3>
          <div class="text-fg-mute mb-6 leading-relaxed">
            <slot v-if="$slots.default" />
            <template v-else>{{ description }}</template>
          </div>
          <div class="flex gap-3">
            <button
              type="button"
              class="btn-ghost flex-1 justify-center border border-line rounded-lg py-3"
              :disabled="loading"
              @click="onCancel"
            >
              {{ cancelLabel }}
            </button>
            <button
              type="button"
              class="flex-1 py-3 bg-red-500/90 hover:bg-red-500 text-white rounded-lg font-semibold transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              :disabled="loading"
              @click="$emit('confirm')"
            >
              {{ loading ? loadingLabel : confirmLabel }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
defineProps({
  modelValue: { type: Boolean, required: true },
  title: { type: String, default: 'تأكيد الحذف' },
  /** يُستخدم عند عدم تمرير محتوى افتراضي (slot) */
  description: { type: String, default: 'هل أنت متأكد؟ لا يمكن التراجع عن هذا الإجراء.' },
  confirmLabel: { type: String, default: 'حذف نهائياً' },
  cancelLabel: { type: String, default: 'إلغاء' },
  loading: { type: Boolean, default: false },
  loadingLabel: { type: String, default: 'جارٍ التنفيذ...' },
})

const emit = defineEmits(['update:modelValue', 'confirm'])

function onCancel() {
  emit('update:modelValue', false)
}
</script>
