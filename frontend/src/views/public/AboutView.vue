<template>
  <div class="pt-24 pb-20">
    <!-- Hero -->
    <section class="px-4 max-w-6xl mx-auto mb-24">
      <div class="grid md:grid-cols-2 gap-16 items-center">
        <div>
          <div class="badge-gold mb-6">👨‍🎨 عن الفنان</div>
          <h1 class="text-5xl font-bold text-white mb-6 leading-tight">
            قصة الإبداع <br><span class="text-gradient">والشغف بالفن</span>
          </h1>
          <p class="text-gray-400 text-lg leading-relaxed mb-6">
            بدأت رحلتي مع الفن منذ أكثر من 15 عاماً، عندما اكتشفت أن اللوحة البيضاء هي أفضل صديق يمكنك أن تجد.
          </p>
          <p class="text-gray-400 text-lg leading-relaxed mb-8">
            درست الفنون الجميلة وتخصصت في الرسم الزيتي والفن الرقمي، ثم انتقلت إلى تعليم الفن للأجيال الجديدة.
          </p>
          <div class="flex flex-wrap gap-4">
            <RouterLink to="/portfolio" class="btn-primary">🖼️ شاهد أعمالي</RouterLink>
            <RouterLink to="/contact" class="btn-outline">تواصل معي</RouterLink>
          </div>
        </div>
        <div class="relative">
          <div class="aspect-[4/5] rounded-3xl overflow-hidden">
            <img src="https://picsum.photos/400/500?grayscale" alt="الفنان" class="w-full h-full object-cover">
          </div>
          <div class="absolute -bottom-6 -right-6 card p-5">
            <div class="text-3xl font-bold text-gold">15+</div>
            <div class="text-gray-400 text-sm">سنة خبرة</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Skills -->
    <section class="bg-dark-100 py-20 mb-24">
      <div class="max-w-6xl mx-auto px-4">
        <h2 class="section-title text-center mb-16">المهارات والتخصصات</h2>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
          <div v-for="skill in skills" :key="skill.name" class="card p-6 text-center group hover:border-gold/40 transition-all">
            <div class="text-4xl mb-3">{{ skill.icon }}</div>
            <h3 class="text-white font-semibold mb-2">{{ skill.name }}</h3>
            <div class="h-1.5 bg-dark-300 rounded-full overflow-hidden">
              <div class="h-full bg-gold rounded-full transition-all duration-1000"
                   :style="{ width: `${skill.level}%` }"></div>
            </div>
            <p class="text-gold text-xs mt-2">{{ skill.level }}%</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Awards -->
    <section class="max-w-6xl mx-auto px-4 mb-24">
      <h2 class="section-title text-center mb-16">الجوائز والإنجازات</h2>
      <div class="space-y-4">
        <div v-for="award in awards" :key="award.title" class="card p-6 flex items-center gap-6">
          <div class="w-12 h-12 rounded-xl bg-gold/10 border border-gold/20 flex items-center justify-center text-2xl flex-shrink-0">🏆</div>
          <div class="flex-1">
            <h3 class="text-white font-bold">{{ award.title }}</h3>
            <p class="text-gray-400 text-sm">{{ award.org }}</p>
          </div>
          <span class="badge-gold text-sm">{{ award.year }}</span>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
const skills = [
  { icon: '🖌️', name: 'رسم زيتي', level: 95 },
  { icon: '💻', name: 'فن رقمي', level: 88 },
  { icon: '📷', name: 'تصوير', level: 75 },
  { icon: '✏️', name: 'رسم يدوي', level: 92 },
]

const awards = [
  { title: 'أفضل فنان عربي', org: 'مهرجان الفنون العربية', year: '2024' },
  { title: 'جائزة الإبداع', org: 'وزارة الثقافة', year: '2023' },
  { title: 'المعلم المميز', org: 'منصة تعليمية دولية', year: '2022' },
]
</script>
