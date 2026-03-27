import path from 'path'
import { fileURLToPath } from 'url'
const __dirname = path.dirname(fileURLToPath(import.meta.url))

/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: [
    path.join(__dirname, 'index.html'),
    path.join(__dirname, 'src/**/*.{vue,js,ts,css}'),
  ],
  theme: {
    extend: {
      colors: {
        /** ألوان دلالية للوضع الفاتح/الداكن — تُعرَّف في main.css */
        page: 'rgb(var(--color-page) / <alpha-value>)',
        surface: 'rgb(var(--color-surface) / <alpha-value>)',
        input: 'rgb(var(--color-input) / <alpha-value>)',
        line: 'rgb(var(--color-line) / <alpha-value>)',
        /** متداخل ليعمل @apply (مثل text-fg-soft) بشكل موثوق */
        fg: {
          DEFAULT: 'rgb(var(--color-fg) / <alpha-value>)',
          soft: 'rgb(var(--color-fg-soft) / <alpha-value>)',
          mute: 'rgb(var(--color-fg-mute) / <alpha-value>)',
          dim: 'rgb(var(--color-fg-dim) / <alpha-value>)',
        },
        dark: {
          DEFAULT: '#1E1E1E',
          100: '#2A2A2A',
          200: '#333333',
          300: '#404040',
          400: '#525252',
        },
        /** ذهبي متكيف مع الثيم — يُعرَّف في main.css (html.light / html.dark) */
        gold: {
          DEFAULT: 'rgb(var(--color-gold) / <alpha-value>)',
          light: 'rgb(var(--color-gold-light) / <alpha-value>)',
          dark: 'rgb(var(--color-gold-deep) / <alpha-value>)',
        },
        accent: '#E84393',
      },
      fontFamily: {
        arabic: ['Cairo', 'Tajawal', 'Arial', 'sans-serif'],
        display: ['Scheherazade New', 'serif'],
      },
      animation: {
        'fade-in': 'fadeIn 0.6s ease-out',
        'slide-up': 'slideUp 0.5s ease-out',
        'float': 'float 3s ease-in-out infinite',
      },
      keyframes: {
        fadeIn: { '0%': { opacity: '0' }, '100%': { opacity: '1' } },
        slideUp: { '0%': { transform: 'translateY(30px)', opacity: '0' }, '100%': { transform: 'translateY(0)', opacity: '1' } },
        float: { '0%, 100%': { transform: 'translateY(0)' }, '50%': { transform: 'translateY(-10px)' } },
      },
    },
  },
  plugins: [],
}
