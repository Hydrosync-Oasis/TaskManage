import request from '@/utils/axios'

// 创建新任务（需要Admin角色）
export function createTask(data) {
  return request({
    url: '/api/Task/insert',
    method: 'post',
    data
  })
}

// 更新任务（仅创建者可更新）
export function updateTask(data) {
  return request({
    url: '/api/Task/update',
    method: 'post',
    data
  })
}

// 添加任务评论
export function addTaskComment(data) {
  return request({
    url: '/api/Task/comment/add',
    method: 'post',
    data
  })
}

// 获取任务的所有评论
export function getTaskComments(taskId) {
  return request({
    url: `/api/Task/comment/task/${taskId}`,
    method: 'get'
  })
}

// 删除任务评论（需要Admin角色或评论作者）
export function deleteTaskComment(id) {
  return request({
    url: `/api/Task/comment/${id}`,
    method: 'delete'
  })
} 