import axios from 'axios'
import errorHandler from './errorHandler'

// 创建 axios 实例
const service = axios.create({
  timeout: 5000 // 请求超时时间
})

// 请求拦截器
service.interceptors.request.use(
  config => {
    // 从 localStorage 获取 token
    const token = localStorage.getItem('token')
    if (token) {
      // 如果存在 token，添加到请求头
      config.headers['Authorization'] = `Bearer ${token}`
    }
    return config
  },
  error => {
    return Promise.reject(error)
  }
)

// 响应拦截器
service.interceptors.response.use(
  response => {
    return response
  },
  error => {
    // 使用错误处理服务处理错误
    errorHandler.handleApiError(error)
    
    // 返回 Promise.reject 以便调用方可以进一步处理错误
    return Promise.reject(error)
  }
)

export default service