import axios from 'axios'

// 创建 axios 实例，配置基础 URL
// ⚠️ 注意：这里的端口号 (如 5000) 需要替换成你的 Taye.WebAPI 实际运行的端口
const apiClient = axios.create({
  baseURL: 'https://localhost:5001', // 你的 WebAPI 地址
  timeout: 10000,
})

// 请求拦截器（可选）：在这里可以统一添加 token 等
apiClient.interceptors.request.use(
  (config) => {
    // 你可以在这里添加认证信息，例如:
    // config.headers.Authorization = `Bearer ${token}`
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// 响应拦截器（可选）：统一处理错误或数据格式
apiClient.interceptors.response.use(
  (response) => {
    return response.data // 直接返回数据，简化调用
  },
  (error) => {
    // 在这里统一处理 API 错误，比如弹窗提示
    console.error('API 请求失败:', error)
    return Promise.reject(error)
  }
)

// 定义接口返回的数据类型（根据你的后端实际返回结构调整）
export interface DashboardSummary {
  totalEmeralds: number
  weeklyEarned: number
  weeklySpent: number
  rank: string
  // ... 更多字段
}

// 导出具体的 API 请求方法
export const dashboardApi = {
  // 获取总览数据
  getSummary: () => {
    return apiClient.get<DashboardSummary>('/api/dashboard/summary')
  },
  // 获取近期动态
  getRecentActivities: (limit: number = 20) => {
    return apiClient.get('/api/dashboard/recent', { params: { limit } })
  },
  // ... 后续可以继续添加获取成就、挑战等方法
}

// 也可以默认导出 apiClient，以便在特殊情况下使用
export default apiClient
