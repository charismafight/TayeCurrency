<template>
  <n-card :bordered="false" class="module-card">
    <template #header>
      <div class="card-header">
        <span style="color: #f1f5f9; font-weight: 600;">📖 配方书</span>
        <span style="color: #94a3b8; font-size: 12px;">共 {{ items.length }} 种</span>
      </div>
    </template>

    <div v-if="items.length > 0" class="recipe-grid">
      <div v-for="item in items" :key="item.id" class="recipe-card">
        <div class="recipe-icon">{{ item.icon }}</div>
        <div class="recipe-name">{{ item.name }}</div>
        <div class="recipe-cost">⭐ {{ item.cost }}</div>
      </div>
    </div>

    <div v-else class="empty-state">
      <span style="color: #94a3b8;">📭 暂无配方</span>
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { dashboardApi } from '@/services/api'
import type { CraftingItem } from '@/types/dashboard'

const items = ref<CraftingItem[]>([])

const loadData = async () => {
  try {
    const data = await dashboardApi.getCraftingItems()
    items.value = data
  } catch (error) {
    console.error('加载配方数据失败:', error)
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

.recipe-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.recipe-card {
  background: #0f172a;
  border: 2px solid #334155;
  border-radius: 12px;
  padding: 16px 12px;
  text-align: center;
  transition: all 0.2s;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  cursor: default;
}

.recipe-card:hover {
  border-color: #fbbf24;
  background: #1a2332;
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(251, 191, 36, 0.08);
}

.recipe-icon {
  font-size: 32px;
  line-height: 1.2;
}

.recipe-name {
  color: #e2e8f0;
  font-size: 13px;
  font-weight: 500;
  line-height: 1.3;
  min-height: 36px;
  display: flex;
  align-items: center;
}

.recipe-cost {
  color: #fbbf24;
  font-size: 14px;
  font-weight: 600;
}

.empty-state {
  text-align: center;
  padding: 30px 0;
  color: #94a3b8;
}
</style>
