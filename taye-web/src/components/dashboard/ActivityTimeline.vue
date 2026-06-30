<template>
  <n-card :bordered="false" class="module-card">
    <template #header>
      <span style="color: #f1f5f9; font-weight: 600;">📜 星轨日志</span>
      <span style="color: #94a3b8; font-size: 12px; margin-left: 8px;">最近 {{ items.length }} 条</span>
    </template>

    <!-- 筛选 -->
    <div class="filter-bar">
      <n-button size="small" :type="filter === 'all' ? 'primary' : 'default'" @click="filter = 'all'">全部</n-button>
      <n-button size="small" :type="filter === 'reward' ? 'primary' : 'default'" @click="filter = 'reward'">✨
        奖励</n-button>
      <n-button size="small" :type="filter === 'spend' ? 'primary' : 'default'" @click="filter = 'spend'">🛒
        消费</n-button>
      <n-button size="small" :type="filter === 'punish' ? 'primary' : 'default'" @click="filter = 'punish'">❌
        惩罚</n-button>
    </div>

    <!-- 时间线 -->
    <div class="timeline" v-if="groupedItems.length > 0">
      <div v-for="group in groupedItems" :key="group.date" class="timeline-group">
        <div class="timeline-date">{{ group.date }}</div>
        <div v-for="item in group.items" :key="item.id" class="timeline-item">
          <span class="item-time">{{ formatTime(item.date) }}</span>
          <span class="item-icon">{{ getIcon(item.reason) }}</span>
          <span class="item-reason">{{ item.reason }}</span>
          <span class="item-stars" :class="getStarClass(item.starCount)">
            {{ item.starCount > 0 ? '+' : '' }}{{ item.starCount }}
          </span>
          <span v-if="item.imagePath" class="item-photo" @click="viewPhoto(item.imagePath!)">📷</span>
        </div>
      </div>
    </div>

    <div v-else class="empty-state">
      <span style="color: #94a3b8;">暂无记录</span>
    </div>

    <!-- 分页 -->
    <div class="pagination" v-if="totalCount > pageSize">
      <n-button size="small" :disabled="page <= 1" @click="loadData(page - 1)">上一页</n-button>
      <span style="color: #94a3b8; font-size: 13px;">第 {{ page }} / {{ totalPages }} 页</span>
      <n-button size="small" :disabled="page >= totalPages" @click="loadData(page + 1)">下一页</n-button>
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { dashboardApi } from '@/services/api'
import type { ActivityItem } from '@/types/dashboard'

const items = ref<ActivityItem[]>([])
const totalCount = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = ref<'all' | 'reward' | 'spend' | 'punish'>('all')

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value))

const loadData = async (p?: number) => {
  const targetPage = p ?? page.value
  try {
    const type = filter.value === 'all' ? undefined : filter.value
    const data = await dashboardApi.getActivities(targetPage, pageSize.value, type)
    items.value = data.items
    totalCount.value = data.totalCount
    page.value = data.page
  } catch (error) {
    console.error('加载星轨日志失败:', error)
  }
}

// 按日期分组
const groupedItems = computed(() => {
  const groups: { date: string; items: ActivityItem[] }[] = []
  const map = new Map<string, ActivityItem[]>()

  for (const item of items.value) {
    const dateKey = new Date(item.date).toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' })
    if (!map.has(dateKey)) map.set(dateKey, [])
    map.get(dateKey)!.push(item)
  }

  for (const [date, items] of map) {
    groups.push({ date, items })
  }
  return groups
})

const formatTime = (dateStr: string) => {
  const d = new Date(dateStr)
  return d.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
}

const getIcon = (reason: string) => {
  const map: Record<string, string> = {
    '晚上21:30': '🌙',
    '吃饭光盘': '🍽️',
    '主动完成常规作业': '📋',
    '吃完饭清理桌面并收碗': '🧹',
    '读完一本书并写一篇心得': '📚',
    '购买零食': '🍭',
    '购买玩具': '🧸',
    '玩游戏': '🎮',
    '忘记洗脸': '🧼',
    '尿尿忘记冲厕所': '🚽',
    '老师反馈': '📋',
  }
  for (const [key, icon] of Object.entries(map)) {
    if (reason.includes(key)) return icon
  }
  return '📌'
}

const getStarClass = (count: number) => {
  if (count > 0) return 'star-positive'
  if (count < 0) return 'star-negative'
  return 'star-zero'
}

const viewPhoto = (path: string) => {
  window.open(path, '_blank')
}

watch(filter, () => {
  page.value = 1
  loadData(1)
})

onMounted(() => loadData())
</script>

<style scoped>
.module-card {
  background: #1e293b !important;
  border-radius: 12px !important;
}

.filter-bar {
  display: flex;
  gap: 6px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.timeline {
  max-height: 400px;
  overflow-y: auto;
}

.timeline-group {
  margin-bottom: 12px;
}

.timeline-date {
  color: #94a3b8;
  font-size: 12px;
  font-weight: 600;
  padding: 4px 0 4px 4px;
  border-bottom: 1px solid #334155;
  margin-bottom: 6px;
}

.timeline-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 14px;
  color: #e2e8f0;
}

.timeline-item:hover {
  background: #334155;
}

.item-time {
  color: #64748b;
  font-size: 12px;
  min-width: 44px;
}

.item-icon {
  font-size: 16px;
}

.item-reason {
  flex: 1;
  color: #e2e8f0;
  font-size: 13px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.item-stars {
  font-weight: 600;
  font-size: 14px;
  min-width: 36px;
  text-align: right;
}

.star-positive {
  color: #4ade80;
}

.star-negative {
  color: #f87171;
}

.star-zero {
  color: #94a3b8;
}

.item-photo {
  cursor: pointer;
  font-size: 14px;
}

.empty-state {
  text-align: center;
  padding: 30px 0;
  color: #94a3b8;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 12px;
  margin-top: 12px;
}

@media (max-width: 600px) {
  .timeline-item {
    flex-wrap: wrap;
    gap: 4px;
  }

  .item-reason {
    flex: 1 1 100%;
    white-space: normal;
    word-break: break-word;
  }

  .item-time {
    min-width: 40px;
    font-size: 11px;
  }

  .item-stars {
    font-size: 13px;
  }
}
</style>
