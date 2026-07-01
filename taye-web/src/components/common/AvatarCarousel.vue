<template>
  <div class="avatar-carousel" @click="next">
    <Transition name="fade" mode="out-in">
      <img :key="currentIndex" :src="currentSrc" :alt="alt" class="avatar-image" @error="handleImageError" />
    </Transition>
    <!-- 指示器（圆点） -->
    <div v-if="images.length > 1" class="indicators">
      <span v-for="(_, i) in images" :key="i" class="dot" :class="{ active: i === currentIndex }" />
    </div>
    <!-- 切换提示（可选的） -->
    <div class="hint" v-if="images.length > 1">点击切换</div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

const props = defineProps<{
  images: string[]           // 头像图片 URL 列表
  alt?: string
  interval?: number          // 自动轮播间隔（毫秒），默认 5000
}>()

const currentIndex = ref(0)
let timer: ReturnType<typeof setInterval> | null = null

const currentSrc = computed(() => {
  return props.images[currentIndex.value] || props.images[0]
})

// 下一张
const next = () => {
  if (props.images.length <= 1) return
  currentIndex.value = (currentIndex.value + 1) % props.images.length
}

// 处理图片加载失败（显示默认占位）
const handleImageError = (e: Event) => {
  const img = e.target as HTMLImageElement
  img.src = '/default-avatar.png'
}

// 自动轮播
const startAutoPlay = () => {
  if (props.images.length <= 1 || props.interval === 0) return
  timer = setInterval(next, props.interval || 5000)
}

const stopAutoPlay = () => {
  if (timer) {
    clearInterval(timer)
    timer = null
  }
}

onMounted(() => {
  startAutoPlay()
})

onUnmounted(() => {
  stopAutoPlay()
})
</script>

<style scoped>
.avatar-carousel {
  position: relative;
  width: 88px;
  height: 88px;
  border-radius: 50%;
  overflow: hidden;
  cursor: pointer;
  flex-shrink: 0;
}

.avatar-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 50%;
  display: block;
}

/* 淡入淡出过渡 */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.6s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* 指示器圆点 */
.indicators {
  position: absolute;
  bottom: -6px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 4px;
  padding: 2px 6px;
  background: rgba(0, 0, 0, 0.4);
  border-radius: 10px;
  backdrop-filter: blur(4px);
}

.dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.3);
  transition: all 0.3s;
}

.dot.active {
  background: #fbbf24;
  width: 14px;
  border-radius: 3px;
}

.hint {
  position: absolute;
  bottom: -24px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 10px;
  color: #94a3b8;
  opacity: 0.6;
  white-space: nowrap;
  pointer-events: none;
}
</style>
