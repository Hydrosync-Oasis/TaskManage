import { ElMessage, ElNotification } from 'element-plus'

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
  let errorInfo = { title: '未知错误', type: 'error' }
  
  // 根据状态码设置错误信息
  switch (status) {
    case 400:
      errorInfo = { title: '请求错误', type: 'warning' }
      break
    case 401:
      errorInfo = { title: '未授权', type: 'error' }
      break
    case 403:
      errorInfo = { title: '禁止访问', type: 'error' }
      break
    case 404:
      errorInfo = { title: '资源不存在', type: 'warning' }
      break
    case 500:
      errorInfo = { title: '服务器错误', type: 'error' }
      break
  }
  
  // 提取错误信息
  const errorMessage = data.error || data.message || '操作失败'
  
  showError({
    title: errorInfo.title,
    message: errorMessage,
    type: errorInfo.type
  }, options.notificationType)
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