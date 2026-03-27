import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  root: path.resolve(__dirname, '.'),
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  server: {
    port: 4435,
    proxy: {
      '/api': {
        target: 'http://127.0.0.1:5567',
        changeOrigin: true,
        timeout: 600_000,
        proxyTimeout: 600_000
      },
      // Static files from ASP.NET (wwwroot/uploads) — same origin as API
      '/uploads': {
        target: 'http://127.0.0.1:5567',
        changeOrigin: true
      }
    }
  }
})
