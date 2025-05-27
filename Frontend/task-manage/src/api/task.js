import request from '@/utils/axios'

// 创建新任务
export function createTask(data) {
  return request({
    url: '/api/Task/Add',
    method: 'post',
    data
  })
}

// 更新任务
export function updateTask(data) {
  return request({
    url: '/api/Task/Update',
    method: 'put',
    data
  })
}

// 添加任务评论
export function addTaskComment(data) {
  return request({
    url: '/api/Comment/Add',
    method: 'post',
    data
  })
}

// 获取任务信息
export function getTaskInfo(id) {
  return request({
    url: `/api/Task/Info/${id}`,
    method: 'get'
  })
}

// 获取评论详情
export function getCommentById(id) {
  return request({
    url: `/api/Comment/${id}`,
    method: 'get'
  })
}

// 删除任务评论
export function deleteTaskComment(id) {
  return request({
    url: `/api/Comment/Delete/${id}`,
    method: 'delete'
  })
}

// 获取任务的所有评论
export function getTaskComments(taskId) {
  return request({
    url: `/api/Task/${taskId}/Comments`,
    method: 'get'
  })
} 