<template>
  <n-card :bordered="false" class="module-card">
    <template #header>
      <span style="color: #f1f5f9; font-weight: 600;">📋 今日任务</span>
      <span style="color: #94a3b8; font-size: 12px; margin-left: 8px;">
        {{ data.date }}
      </span>
    </template>

    <!-- 任务列表 -->
    <div class="task-list">
      <div v-for="task in data.tasks" :key="task.id" class="task-item" :class="{ 'task-completed': task.isCompleted }">
        <span class="task-icon">{{ task.icon }}</span>
        <span class="task-name">{{ task.name }}</span>
        <div class="task-progress">
          <n-progress type="line" :percentage="(task.current / task.target) * 100" :height="6"
            :color="task.isCompleted ? '#4ade80' : '#fbbf24'" :show-indicator="false" />
          <span class="task-count">{{ task.current }}/{{ task.target }}</span>
        </div>
        <span class="task-status">
          {{ task.isCompleted ? '✅' : '⏳' }}
        </span>
      </div>
    </div>

    <!-- 底部：全部完成状态 -->
    <div class="task-footer">
      <div v-if="data.allCompleted && data.bonusEarned" class="bonus-earned">
        🎉 全部完成！+{{ data.bonusStars }}⭐ 已领取
      </div>
      <div v-else-if="data.allCompleted && !data.bonusEarned" class="bonus-ready">
        🎉 全部完成！领取 +{{ data.bonusStars }}⭐
      </div>
      <div v-else class="bonus-pending">
        完成全部任务，额外 +{{ data.bonusStars }}⭐
      </div>
    </div>
  </n-card>
</template>

<script setup lang="ts">
import type { TasksData } from '@/types/dashboard'

defineProps<{
  data: TasksData
}>()
</script>

<style scoped>
.module-card {
  background: #1e293b !important;
  border-radius: 12px !important;
}

.task-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.task-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 12px;
  background: #0f172a;
  border-radius: 8px;
  border: 1px solid #334155;
  transition: border-color 0.3s;
}

.task-item.task-completed {
  border-color: #4ade80;
}

.task-icon {
  font-size: 20px;
  flex-shrink: 0;
}

.task-name {
  color: #e2e8f0;
  font-size: 14px;
  flex: 1;
  min-width: 60px;
}

.task-progress {
  display: flex;
  align-items: center;
  gap: 8px;
  flex: 2;
  min-width: 80px;
}

.task-progress .n-progress {
  flex: 1;
}

.task-count {
  color: #94a3b8;
  font-size: 12px;
  white-space: nowrap;
  min-width: 32px;
}

.task-status {
  font-size: 16px;
  flex-shrink: 0;
}

.task-footer {
  margin-top: 14px;
  padding: 10px 12px;
  border-radius: 8px;
  text-align: center;
  font-size: 14px;
}

.bonus-earned {
  background: rgba(74, 222, 128, 0.15);
  color: #4ade80;
}

.bonus-ready {
  background: rgba(251, 191, 36, 0.15);
  color: #fbbf24;
}

.bonus-pending {
  background: rgba(148, 163, 184, 0.1);
  color: #94a3b8;
}
</style>
