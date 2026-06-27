<template>
  <div class="dashboard-container">
    <section class="section-hero">
      <HeroProfile :data="heroData" @view-achievements="() => { }" @view-crafting="() => { }"
        @refresh-data="handleRefresh" />
    </section>

    <!-- 中部：今日任务 + 合成台 -->
    <section class="section-main">
      <div class="main-left">
        <DailyTasks :data="tasksData" />
      </div>
      <div class="main-right">
        <CraftingTable :items="craftingData" />
      </div>
    </section>

    <!-- 底部 -->
    <section class="section-bottom">
      <div class="bottom-left">
        <ActivityTimeline />
      </div>
      <div class="bottom-right">
        <AchievementWall :achievements="achievementData" />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import HeroProfile from '@/components/dashboard/HeroProfile.vue'
import DailyTasks from '@/components/dashboard/DailyTasks.vue'
import AchievementWall from '@/components/dashboard/AchievementWall.vue'
import ActivityTimeline from '@/components/dashboard/ActivityTimeline.vue'
import CraftingTable from '@/components/dashboard/CraftingTable.vue'
import type { HeroProfileData, Achievement, TasksData, Activity, CraftingItem } from '@/types/dashboard'
import { dashboardApi } from '@/services/api'

const heroData = ref<HeroProfileData>({
  playerName: 'Taye',
  avatarUrl: '',
  starBalance: 0,
  yesterdayBalance: 0,
  weeklyEarned: 0,
  weeklySpent: 0,
  weeklyPunished: 0,
  rank: '🌱初入世界',
  nextRank: '🔥勇敢探险家',
  expPercent: 0,
  totalStars: 0
})

const tasksData = ref<TasksData>({
  date: '',
  tasks: [],
  allCompleted: false,
  bonusStars: 1,
  bonusEarned: false
})

const loadTasks = async () => {
  try {
    const data = await dashboardApi.getTasks()
    tasksData.value = data
  } catch (error) {
    console.error('加载任务数据失败:', error)
  }
}

const achievementData = ref<Achievement[]>([])
const activityData = ref<Activity[]>([])
const craftingData = ref<CraftingItem[]>([])

const loadProfile = async () => {
  try {
    const data = await dashboardApi.getProfile()
    heroData.value = data
  } catch (error) {
    console.error('加载资料卡数据失败:', error)
  }
}

const handleRefresh = () => {
  window.location.reload()
}

onMounted(() => {
  loadProfile()
  loadTasks()
})
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
