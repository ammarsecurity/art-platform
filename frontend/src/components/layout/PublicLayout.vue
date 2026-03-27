<template>
  <div class="min-h-screen bg-page flex flex-col">
    <AppNavbar />
    <main class="flex-1">
      <RouterView />
    </main>
    <AppFooter />
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useSiteSettingsStore } from '@/stores/siteSettings'
import AppNavbar from './AppNavbar.vue'
import AppFooter from './AppFooter.vue'

const site = useSiteSettingsStore()
onMounted(() => site.load())
</script>
