import type { HeroProfileData } from '@/types/dashboard'
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
}

export default apiClient
