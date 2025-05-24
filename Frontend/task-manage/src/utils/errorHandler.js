import { ElMessage, ElNotification } from 'element-plus'

// 错误类型映射
const errorTypeMap = {
  400: { title: '请求错误', type: 'warning' },
  401: { title: '未授权', type: 'error' },
  403: { title: '禁止访问', type: 'error' },
  404: { title: '资源不存在', type: 'warning' },
  500: { title: '服务器错误', type: 'error' },
}

/**
 * 处理API错误
 * @param {Object} error - Axios错误对象
 * @param {Object} options - 配置选项
 * @param {string} options.notificationType - 通知类型: 'message'或'notification'
 */
export const handleApiError = (error, options = { notificationType: 'message' }) => {
  if (!error.response) {
    // 网络错误或请求被取消
    showError({
      title: '网络错误',
      message: '无法连接到服务器，请检查您的网络连接',
      type: 'error'
    }, options.notificationType)
    return
  }

  const { status, data } = error.response
  const errorInfo = errorTypeMap[status] || { title: '未知错误', type: 'error' }
  
  // 提取错误信息
  const errorMessage = data.error || data.message || '操作失败'
  
  showError({
    title: errorInfo.title,
    message: errorMessage,
    type: errorInfo.type
  }, options.notificationType)
  
  // 特殊处理：401未授权
  if (status === 401) {
    // 可以在这里添加重定向到登录页的逻辑
    console.log('用户未授权，需要重新登录')
  }
}

/**
 * 显示错误信息
 * @param {Object} errorInfo - 错误信息对象
 * @param {string} type - 显示类型: 'message'或'notification'
 */
export const showError = (errorInfo, type = 'message') => {
  if (type === 'notification') {
    ElNotification({
      title: errorInfo.title,
      message: errorInfo.message,
      type: errorInfo.type,
      duration: 5000
    })
  } else {
    ElMessage({
      message: errorInfo.message,
      type: errorInfo.type,
      showClose: true,
      duration: 5000
    })
  }
}

export default {
  handleApiError,
  showError
}