<template>
  <div class="dashboard-container">
    <section class="section-hero">
      <HeroProfile :data="heroData" @view-achievements="() => { }" @view-crafting="() => { }"
        @refresh-data="() => { }" />
    </section>

    <!-- 其他模块占位 -->
    <section class="section-main">
      <div class="main-left">
        <AchievementWall :achievements="achievementData" />
      </div>
      <div class="main-right">
        <ChallengeList :challenges="challengeData" />
      </div>
    </section>
    <section class="section-bottom">
      <div class="bottom-left">
        <ActivityTimeline :activities="activityData" />
      </div>
      <div class="bottom-right">
        <CraftingTable :items="craftingData" />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import HeroProfile from '@/components/dashboard/HeroProfile.vue'
import AchievementWall from '@/components/dashboard/AchievementWall.vue'
import ChallengeList from '@/components/dashboard/ChallengeList.vue'
import ActivityTimeline from '@/components/dashboard/ActivityTimeline.vue'
import CraftingTable from '@/components/dashboard/CraftingTable.vue'
import type { HeroProfileData, Achievement, Challenge, Activity, CraftingItem } from '@/types/dashboard'

// ===== 英雄数据 =====
const heroData = ref<HeroProfileData>({
  playerName: 'Taye',
  avatarUrl: '',
  starBalance: 42,
  yesterdayBalance: 50,
  weeklyEarned: 18,
  weeklySpent: 7,
  rank: '⚔️ 屠龙者',
  nextRank: '🌟 传奇英雄',
  expPercent: 68,
  totalStars: 9999
})

// ===== 其他模拟数据（占位） =====
const achievementData = ref<Achievement[]>([])
const challengeData = ref<Challenge[]>([])
const activityData = ref<Activity[]>([])
const craftingData = ref<CraftingItem[]>([])
</script>

<style scoped>
.dashboard-container {
  min-height: 100vh;
  background: #0f172a;
  padding: 20px;
  max-width: 1280px;
  margin: 0 auto;
}

.section-hero {
  margin-bottom: 24px;
}

.section-main {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
  margin-bottom: 24px;
}

.section-bottom {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
}

@media (max-width: 992px) {
  .section-main {
    grid-template-columns: 1fr;
  }

  .section-bottom {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 600px) {
  .dashboard-container {
    padding: 12px;
  }

  .section-hero {
    margin-bottom: 16px;
  }

  .section-main {
    gap: 16px;
    margin-bottom: 16px;
  }

  .section-bottom {
    gap: 16px;
  }
}
</style>
