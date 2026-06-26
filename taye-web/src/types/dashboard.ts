// 英雄资料卡数据
export interface HeroProfileData {
  playerName: string
  avatarUrl?: string
  starBalance: number      // 今日星星余额（原 emeraldBalance）
  yesterdayBalance: number // 昨日余额
  weeklyEarned: number
  weeklySpent: number
  weeklyPunished: number
  rank: string
  nextRank: string
  expPercent: number
  totalStars: number
}

// 成就
export interface Achievement {
  id: number
  name: string
  icon: string
  category: '学业' | '生活' | '校园'
  count: number      // 本周完成次数
  emeralds: number   // 获得星币
  unlocked: boolean
}

// 挑战
export interface Challenge {
  id: number
  name: string
  icon: string
  currentWeek: number  // 本周次数
  target: number       // 目标（0 = 完全避免）
  status: 'conquered' | 'active' | 'danger'
  streak: number       // 连续达标天数
}

// 动态
export interface Activity {
  id: number
  time: string
  icon: string
  name: string
  change: string  // '+3' 或 '-1'
  photo?: string  // 照片URL
}

// 合成台
export interface CraftingItem {
  id: number
  name: string
  icon: string
  cost: number
  available: boolean
  status: 'craftable' | 'insufficient' | 'pending'
}
