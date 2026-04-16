<template>
  <RouterView />
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useAppRouteSeo } from '@/composables/useAppRouteSeo'

useAppRouteSeo()

const auth = useAuthStore()
onMounted(() => auth.init())
</script>
