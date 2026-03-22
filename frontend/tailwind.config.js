import path from 'path'
import { fileURLToPath } from 'url'
const __dirname = path.dirname(fileURLToPath(import.meta.url))

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    path.join(__dirname, 'index.html'),
    path.join(__dirname, 'src/**/*.{vue,js,ts}'),
  ],
  theme: {
    extend: {
      colors: {
        dark: {
          DEFAULT: '#1E1E1E',
          100: '#2A2A2A',
          200: '#333333',
          300: '#404040',
          400: '#525252',
        },
        gold: {
          DEFAULT: '#C9A96E',
          light: '#DFC08A',
          dark: '#A8874A',
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
