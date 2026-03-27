import { defineStore } from 'pinia'
import { ref } from 'vue'
import { artworkApi, categoryApi } from '@/services/api'

export const useArtworkStore = defineStore('artworks', () => {
  const artworks = ref([])
  const featured = ref([])
  const currentArtwork = ref(null)
  const categories = ref([])
  const pagination = ref({ page: 1, pageSize: 12, totalCount: 0, totalPages: 0 })
  const loading = ref(false)
  const filters = ref({ categoryId: null, search: '', sortBy: 'createdAt', sortOrder: 'desc' })

  async function fetchArtworks(params = {}) {
    loading.value = true
    try {
      const query = { ...filters.value, ...pagination.value, ...params }
      const res = await artworkApi.getAll(query)
      artworks.value = res.data.items
      pagination.value = { ...pagination.value, totalCount: res.data.totalCount, totalPages: res.data.totalPages }
    } finally {
      loading.value = false
    }
  }

  /** أحدث الأعمال المنشورة للصفحة الرئيسية (حسب تاريخ الإضافة، لا اشتراط «مميز») */
  async function fetchFeatured() {
    loading.value = true
    try {
      const res = await artworkApi.getAll({
        page: 1,
        pageSize: 6,
        sortBy: 'createdAt',
        sortOrder: 'desc',
      })
      featured.value = res.data?.items ?? []
    } finally {
      loading.value = false
    }
  }

  async function fetchBySlug(slug) {
    loading.value = true
    try {
      const res = await artworkApi.getBySlug(slug)
      currentArtwork.value = res.data
    } finally {
      loading.value = false
    }
  }

  async function fetchCategories() {
    const res = await categoryApi.getAll()
    categories.value = res.data
  }

  function setFilter(key, value) {
    filters.value[key] = value
    pagination.value.page = 1
    fetchArtworks()
  }

  function setPage(page) {
    pagination.value.page = page
    fetchArtworks()
  }

  return {
    artworks, featured, currentArtwork, categories,
    pagination, loading, filters,
    fetchArtworks, fetchFeatured, fetchBySlug, fetchCategories, setFilter, setPage
  }
})
