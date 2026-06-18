<template>
  <div class="hero-profile-wrapper">
    <div class="hero-banner">
      <!-- 装饰星币 -->
      <div class="deco-stars">
        <span v-for="i in 8" :key="i" class="star" :style="{
          top: Math.random() * 90 + '%',
          left: Math.random() * 90 + '%',
          animationDelay: Math.random() * 3 + 's',
          fontSize: (Math.random() * 12 + 8) + 'px'
        }">✦</span>
      </div>

      <div class="hero-content">
        <!-- 左列 -->
        <div class="hero-left">
          <div class="avatar-section">
            <div class="avatar-frame">
              <n-avatar :size="88" :src="data.avatarUrl || ''" class="hero-avatar" />
              <div class="level-badge">
                <span class="level-icon">⚡</span>
                <span class="level-number">{{ currentLevel }}</span>
              </div>
            </div>
            <div class="rank-badge">{{ data.rank }}</div>
          </div>

          <div class="player-info">
            <div class="greeting">
              <span class="greeting-icon">👋</span>
              <span class="greeting-text">{{ greetingText }}</span>
            </div>
            <h1 class="player-name">
              {{ data.playerName }}
              <span class="player-badge">🎮</span>
            </h1>
            <div class="player-stats">
              <n-tag size="small" round color="#4ade80" :bordered="false">📈 本周 +{{ data.weeklyEarned }}</n-tag>
              <n-tag size="small" round color="#f87171" :bordered="false">📉 本周 -{{ data.weeklySpent }}</n-tag>
              <n-tag size="small" round color="#fbbf24" :bordered="false">🏅 {{ data.totalStars }} 累计</n-tag>
            </div>
          </div>
        </div>

        <!-- 右列 -->
        <div class="hero-right">
          <!-- ⭐ 星币卡片（原绿宝石卡片） -->
          <div class="star-card">
            <div class="star-icon-wrapper">
              <span class="star-icon">⭐</span>
              <span class="star-glow"></span>
            </div>
            <div class="star-info">
              <div class="star-amount-wrapper">
                <span class="star-amount" :class="{
                  'amount-up': changeDirection === 'up',
                  'amount-down': changeDirection === 'down'
                }">
                  {{ animatedBalance }}
                </span>
                <span class="star-label">星币</span>
                <span v-if="changeDirection === 'up'" class="change-indicator up">↑</span>
                <span v-if="changeDirection === 'down'" class="change-indicator down">↓</span>
              </div>
              <div class="star-sub">
                <span :class="['day-diff', dayDiffClass]">
                  {{ dayDiffText }}
                </span>
                <span class="divider">·</span>
                <span>{{ data.playerName }} 的资产</span>
              </div>
            </div>
          </div>

          <!-- 经验条 -->
          <div class="exp-section">
            <div class="exp-header">
              <span class="exp-rank-current">{{ data.rank }}</span>
              <div class="exp-rank-next">
                <span>➜</span>
                <span class="exp-rank-next-name">{{ data.nextRank }}</span>
              </div>
            </div>
            <n-progress type="line" :percentage="data.expPercent" :height="12" color="#fbbf24" :show-indicator="false"
              class="exp-bar" />
            <div class="exp-footer">
              <span class="exp-percent">{{ data.expPercent }}%</span>
              <span class="exp-hint">距离 <strong>{{ data.nextRank }}</strong> 还需 {{ 100 - data.expPercent }}%</span>
            </div>
          </div>

          <!-- 快捷操作 -->
          <div class="quick-actions">
            <n-button size="small" ghost round @click="emit('viewAchievements')">🏆 成就</n-button>
            <n-button size="small" ghost round @click="emit('viewCrafting')">🔨 合成台</n-button>
            <n-button size="small" ghost round @click="emit('refreshData')">🔄 刷新</n-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import type { HeroProfileData } from '@/types/dashboard'

const props = defineProps<{
  data: HeroProfileData
}>()

const emit = defineEmits<{
  (e: 'viewAchievements'): void
  (e: 'viewCrafting'): void
  (e: 'refreshData'): void
}>()

// ===== 等级计算 =====
const currentLevel = computed(() => {
  const total = props.data.totalStars
  if (total < 50) return 1
  if (total < 150) return 2
  if (total < 300) return 3
  if (total < 500) return 4
  if (total < 800) return 5
  return 66
})

// ===== 问候语 =====
const greetingText = computed(() => {
  const hour = new Date().getHours()
  if (hour < 6) return '夜深了，还在冒险吗？'
  if (hour < 9) return '早上好，新的一天开始了！'
  if (hour < 12) return '上午好，保持专注！'
  if (hour < 14) return '中午好，记得吃饭哦！'
  if (hour < 18) return '下午好，继续加油！'
  if (hour < 21) return '晚上好，今天收获如何？'
  return '夜深了，准备休息吧！'
})

// ===== 今日 vs 昨日 =====
const todayBalance = ref(props.data.starBalance)
const yesterdayBalance = ref(props.data.yesterdayBalance)

const dayDiff = computed(() => {
  return todayBalance.value - yesterdayBalance.value
})

const dayDiffText = computed(() => {
  const diff = dayDiff.value
  if (diff > 0) return `↑ 今日 +${diff}`
  if (diff < 0) return `↓ 今日 ${diff}`
  return '— 与昨日持平'
})

const dayDiffClass = computed(() => {
  const diff = dayDiff.value
  if (diff > 0) return 'diff-up'
  if (diff < 0) return 'diff-down'
  return 'diff-flat'
})

// ===== 数字动画 =====
const animatedBalance = ref(props.data.starBalance)
const changeDirection = ref<'up' | 'down' | null>(null)
const isAnimating = ref(false)

const easeOutCubic = (t: number): number => {
  return 1 - Math.pow(1 - t, 3)
}

const animateNumber = (from: number, to: number, duration: number = 1200) => {
  if (isAnimating.value) return
  const diff = to - from
  if (diff === 0) {
    animatedBalance.value = to
    changeDirection.value = null
    return
  }

  changeDirection.value = diff > 0 ? 'up' : 'down'
  isAnimating.value = true

  const startTime = performance.now()

  const update = (currentTime: number) => {
    const elapsed = currentTime - startTime
    const progress = Math.min(elapsed / duration, 1)
    const easedProgress = easeOutCubic(progress)
    const currentValue = from + diff * easedProgress
    animatedBalance.value = Math.round(currentValue)

    if (progress < 1) {
      requestAnimationFrame(update)
    } else {
      animatedBalance.value = to
      isAnimating.value = false
      setTimeout(() => {
        changeDirection.value = null
      }, 800)
    }
  }

  requestAnimationFrame(update)
}

// ===== 监听数据变化 =====
watch(
  () => props.data.starBalance,
  (newVal, oldVal) => {
    if (oldVal === undefined) {
      animatedBalance.value = newVal
      todayBalance.value = newVal
      return
    }
    if (newVal !== oldVal) {
      todayBalance.value = newVal
      animateNumber(oldVal, newVal)
    }
  },
  { immediate: true }
)

// ===== 页面加载时：从昨日余额滚动到今日余额 =====
onMounted(() => {
  const from = props.data.yesterdayBalance
  const to = props.data.starBalance
  animatedBalance.value = from
  todayBalance.value = to
  setTimeout(() => {
    animateNumber(from, to, 1500)
  }, 300)
})
</script>

<style scoped>
/* ===== 布局样式 ===== */
.hero-profile-wrapper {
  width: 100%;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
}

.hero-banner {
  position: relative;
  background: linear-gradient(145deg, #0f172a 0%, #1e1b4b 40%, #312e81 80%, #1e1b4b 100%);
  padding: 28px 32px;
  min-height: 170px;
  overflow: hidden;
}

.deco-stars {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  overflow: hidden;
}

.deco-stars .star {
  position: absolute;
  color: rgba(251, 191, 36, 0.15);
  animation: twinkle 3s ease-in-out infinite alternate;
}

@keyframes twinkle {
  0% {
    opacity: 0.1;
    transform: scale(0.8);
  }

  100% {
    opacity: 0.6;
    transform: scale(1.2);
  }
}

.hero-content {
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: stretch;
  gap: 24px;
  flex-wrap: wrap;
}

.hero-left {
  display: flex;
  align-items: center;
  gap: 20px;
  flex: 1 1 320px;
  min-width: 260px;
}

.avatar-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
  flex-shrink: 0;
}

.avatar-frame {
  position: relative;
  padding: 4px;
  border-radius: 50%;
  background: linear-gradient(135deg, #fbbf24, #f97316);
  box-shadow: 0 0 30px rgba(251, 191, 36, 0.25);
}

.hero-avatar {
  border: 3px solid #0f172a;
  display: block;
}

/* ===== 等级徽章 ===== */
.level-badge {
  position: absolute;
  bottom: -4px;
  right: -4px;
  background: #1e293b;
  border: 2px solid #fbbf24;
  border-radius: 50%;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0px;
  padding: 0;
  box-sizing: border-box;
  overflow: hidden;
  white-space: nowrap;
}

.level-badge .level-icon {
  font-size: 8px;
  line-height: 1;
  flex-shrink: 0;
}

.level-badge .level-number {
  font-size: 8px;
  font-weight: 700;
  color: #fbbf24;
  line-height: 1;
  flex-shrink: 0;
}

/* ===== 手机端适配 ===== */
@media (max-width: 480px) {
  .level-badge {
    width: 26px;
    height: 26px;
    bottom: -3px;
    right: -3px;
  }

  .level-badge .level-icon,
  .level-badge .level-number {
    font-size: 8px;
  }
}

.rank-badge {
  font-size: 12px;
  font-weight: 600;
  color: #fbbf24;
  background: rgba(251, 191, 36, 0.15);
  padding: 2px 16px;
  border-radius: 20px;
  border: 1px solid rgba(251, 191, 36, 0.2);
  white-space: nowrap;
}

.player-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.greeting {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
  color: #94a3b8;
}

.greeting-icon {
  font-size: 16px;
}

.greeting-text {
  font-weight: 400;
}

.player-name {
  font-size: 30px;
  font-weight: 700;
  color: #ffffff;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
  letter-spacing: 0.5px;
}

.player-badge {
  font-size: 20px;
}

.player-stats {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 2px;
}

.hero-right {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 12px;
  flex: 0 1 auto;
  min-width: 220px;
}

/* ===== ⭐ 星币卡片 ===== */
.star-card {
  display: flex;
  align-items: center;
  gap: 16px;
  background: rgba(251, 191, 36, 0.08);
  border: 1px solid rgba(251, 191, 36, 0.15);
  border-radius: 16px;
  padding: 12px 24px 12px 16px;
  backdrop-filter: blur(4px);
}

.star-icon-wrapper {
  position: relative;
  flex-shrink: 0;
}

.star-icon {
  font-size: 36px;
  display: block;
}

.star-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 60px;
  height: 60px;
  transform: translate(-50%, -50%);
  background: radial-gradient(circle, rgba(251, 191, 36, 0.2), transparent 70%);
  border-radius: 50%;
  pointer-events: none;
  animation: glowPulse 2s ease-in-out infinite alternate;
}

@keyframes glowPulse {
  0% {
    transform: translate(-50%, -50%) scale(0.8);
    opacity: 0.4;
  }

  100% {
    transform: translate(-50%, -50%) scale(1.4);
    opacity: 1;
  }
}

.star-info {
  display: flex;
  flex-direction: column;
}

.star-amount-wrapper {
  display: flex;
  align-items: baseline;
  gap: 8px;
}

.star-amount {
  font-size: 36px;
  font-weight: 800;
  color: #fbbf24;
  line-height: 1;
  font-variant-numeric: tabular-nums;
  transition: none;
}

/* 增加时闪烁 */
.amount-up {
  animation: flashGreen 0.8s ease;
}

@keyframes flashGreen {
  0% {
    color: #fbbf24;
    transform: scale(1);
  }

  25% {
    color: #4ade80;
    transform: scale(1.2);
  }

  50% {
    color: #fbbf24;
    transform: scale(0.95);
  }

  75% {
    color: #4ade80;
    transform: scale(1.05);
  }

  100% {
    color: #fbbf24;
    transform: scale(1);
  }
}

/* 减少时闪烁 */
.amount-down {
  animation: flashRed 0.8s ease;
}

@keyframes flashRed {
  0% {
    color: #fbbf24;
    transform: scale(1);
  }

  25% {
    color: #f87171;
    transform: scale(0.85);
  }

  50% {
    color: #fbbf24;
    transform: scale(1.05);
  }

  75% {
    color: #f87171;
    transform: scale(0.95);
  }

  100% {
    color: #fbbf24;
    transform: scale(1);
  }
}

.star-label {
  font-size: 16px;
  font-weight: 500;
  color: #94a3b8;
}

.star-sub {
  font-size: 12px;
  color: #64748b;
  margin-top: 2px;
}

/* ===== 今日 vs 昨日 对比颜色 ===== */
.day-diff {
  font-weight: 600;
  font-size: 14px;
}

.day-diff.diff-up {
  color: #22c55e;
}

.day-diff.diff-down {
  color: #ef4444;
}

.day-diff.diff-flat {
  color: #94a3b8;
}

.change-indicator {
  font-size: 20px;
  font-weight: 700;
  margin-left: 4px;
  animation: bounceArrow 0.6s ease;
}

.change-indicator.up {
  color: #4ade80;
}

.change-indicator.down {
  color: #f87171;
}

@keyframes bounceArrow {
  0% {
    transform: translateY(0);
    opacity: 0;
  }

  50% {
    transform: translateY(-8px);
    opacity: 1;
  }

  100% {
    transform: translateY(0);
    opacity: 1;
  }
}

.divider {
  color: #334155;
  margin: 0 4px;
}

/* ===== 经验条 ===== */
.exp-section {
  width: 100%;
}

.exp-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
  color: #94a3b8;
  margin-bottom: 2px;
}

.exp-rank-current {
  font-weight: 600;
  color: #fbbf24;
}

.exp-rank-next {
  display: flex;
  align-items: center;
  gap: 6px;
}

.exp-rank-next-name {
  color: #94a3b8;
}

.exp-bar {
  margin: 0;
}

.exp-footer {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
  color: #64748b;
  margin-top: 2px;
}

.exp-percent {
  font-weight: 600;
  color: #e2e8f0;
}

.quick-actions {
  display: flex;
  gap: 8px;
  justify-content: flex-end;
  margin-top: 4px;
}

.quick-actions .n-button {
  color: #94a3b8;
  border-color: #334155;
  font-size: 12px;
  padding: 4px 12px;
}

.quick-actions .n-button:hover {
  color: #fbbf24;
  border-color: #fbbf24;
}

/* ===== 响应式 ===== */
@media (max-width: 768px) {
  .hero-banner {
    padding: 20px;
    min-height: auto;
  }

  .hero-content {
    flex-direction: column;
    gap: 16px;
  }

  .hero-left {
    flex: 1 1 auto;
    flex-wrap: wrap;
    justify-content: center;
    text-align: center;
  }

  .player-info {
    align-items: center;
  }

  .player-stats {
    justify-content: center;
  }

  .hero-right {
    flex: 1 1 auto;
    width: 100%;
  }

  .star-card {
    justify-content: center;
  }

  .quick-actions {
    justify-content: center;
  }

  .player-name {
    font-size: 24px;
  }

  .star-amount {
    font-size: 28px;
  }

  .deco-stars {
    display: none;
  }
}

@media (max-width: 480px) {
  .hero-banner {
    padding: 14px;
  }

  .avatar-frame .n-avatar {
    width: 64px !important;
    height: 64px !important;
  }

  .level-badge {
    width: 26px;
    height: 26px;
    font-size: 10px;
    bottom: -2px;
    right: -2px;
  }

  .player-name {
    font-size: 20px;
  }

  .star-card {
    padding: 10px 16px;
  }

  .star-amount {
    font-size: 24px;
  }

  .star-icon {
    font-size: 28px;
  }
}
</style>
