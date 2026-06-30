<template>
  <n-card :bordered="false" class="module-card">
    <template #header>
      <div class="card-header">
        <span style="color: #f1f5f9; font-weight: 600;">🏆 成就墙</span>
        <span style="color: #94a3b8; font-size: 12px;">
          {{ unlockedCount }}/{{ items.length }} 已解锁
        </span>
      </div>
    </template>

    <!-- 分类 Tab -->
    <div class="category-tabs">
      <button v-for="cat in categories" :key="cat" class="tab-btn" :class="{ active: activeCategory === cat }"
        @click="activeCategory = cat">
        {{ cat }}
      </button>
    </div>

    <!-- 成就网格 -->
    <div v-if="filteredItems.length > 0" class="achievement-grid">
      <div v-for="item in filteredItems" :key="item.id" class="achievement-card"
        :class="{ 'achievement-unlocked': item.isUnlocked, 'achievement-locked': !item.isUnlocked }">
        <div class="achievement-icon">{{ item.icon }}</div>
        <div class="achievement-name">{{ item.name }}</div>
        <div class="achievement-progress">
          {{ item.currentCount }} / {{ nextTarget(item) }}
        </div>
        <div class="achievement-status">
          {{ item.isUnlocked ? '✅ 已解锁' : '🔒 进行中' }}
        </div>
        <div v-if="item.isUnlocked && item.unlockedMilestoneIndex >= 0" class="achievement-title">
          {{ item.milestones[item.unlockedMilestoneIndex]?.title }}
        </div>
      </div>
    </div>

    <div v-else class="empty-state">
      <span style="color: #94a3b8;">暂无成就</span>
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { dashboardApi } from '@/services/api'
import type { Achievement } from '@/types/dashboard'

const items = ref<Achievement[]>([])
const activeCategory = ref<string>('全部')

const categories = computed(() => {
  const cats = new Set(items.value.map(i => i.category).filter(Boolean))
  return ['全部', ...cats]
})

const filteredItems = computed(() => {
  if (activeCategory.value === '全部') return items.value
  return items.value.filter(i => i.category === activeCategory.value)
})

const unlockedCount = computed(() => {
  return items.value.filter(i => i.isUnlocked).length
})

const nextTarget = (item: Achievement) => {
  if (item.nextMilestoneCount !== null) return item.nextMilestoneCount
  const last = item.milestones[item.milestones.length - 1]
  return last?.count ?? item.currentCount
}

const loadData = async () => {
  try {
    const data = await dashboardApi.getAchievements()
    items.value = data
  } catch (error) {
    console.error('加载成就数据失败:', error)
  }
}

onMounted(() => loadData())
</script>

<style scoped>
.module-card {
  background: #1e293b !important;
  border-radius: 12px !important;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.category-tabs {
  display: flex;
  gap: 6px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.tab-btn {
  background: #0f172a;
  border: 1px solid #334155;
  border-radius: 16px;
  padding: 4px 14px;
  color: #94a3b8;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.tab-btn.active {
  border-color: #fbbf24;
  color: #fbbf24;
  background: rgba(251, 191, 36, 0.08);
}

.tab-btn:hover {
  border-color: #fbbf24;
  color: #f1f5f9;
}

.achievement-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
}

.achievement-card {
  background: #0f172a;
  border: 2px solid #334155;
  border-radius: 10px;
  padding: 12px;
  text-align: center;
  transition: all 0.2s;
}

.achievement-unlocked {
  border-color: #fbbf24;
  background: rgba(251, 191, 36, 0.04);
}

.achievement-locked {
  opacity: 0.5;
}

.achievement-icon {
  font-size: 28px;
}

.achievement-name {
  color: #e2e8f0;
  font-size: 13px;
  font-weight: 500;
  margin-top: 2px;
}

.achievement-progress {
  color: #94a3b8;
  font-size: 12px;
  margin-top: 2px;
}

.achievement-status {
  font-size: 11px;
  margin-top: 2px;
}

.achievement-unlocked .achievement-status {
  color: #4ade80;
}

.achievement-locked .achievement-status {
  color: #64748b;
}

.achievement-title {
  color: #fbbf24;
  font-size: 12px;
  font-weight: 600;
  margin-top: 2px;
}

.empty-state {
  text-align: center;
  padding: 30px 0;
  color: #94a3b8;
}

@media (max-width: 600px) {
  .achievement-grid {
    grid-template-columns: 1fr;
    /* 手机端一列 */
  }
}
</style>
