/** CommonJS لتفادي أخطاء تحميل PostCSS في وضع التطوير (Vite + Windows) */
module.exports = {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
}
