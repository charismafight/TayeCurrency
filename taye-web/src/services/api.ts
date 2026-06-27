import type { HeroProfileData, TasksData, ActivitiesData } from '@/types/dashboard'
import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
})

apiClient.interceptors.request.use(
  (config) => {
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

apiClient.interceptors.response.use(
  (response) => {
    return response.data
  },
  (error) => {
    console.error('API 请求失败:', error)
    return Promise.reject(error)
  }
)

const get = async <T>(url: string): Promise<T> => {
  const res = await apiClient.get<T>(url)
  return res.data as T
}

export const dashboardApi = {
  getProfile: () => get<HeroProfileData>('/dashboard/profile'),
  getTasks: () => get<TasksData>('/dashboard/tasks'),
  getActivities: (page: number = 1, pageSize: number = 20, type?: string) => {
    const params = new URLSearchParams()
    params.append('page', String(page))
    params.append('pageSize', String(pageSize))
    if (type) params.append('type', type)
    return get<ActivitiesData>(`/dashboard/activities?${params.toString()}`)
  },
}

export default apiClient
