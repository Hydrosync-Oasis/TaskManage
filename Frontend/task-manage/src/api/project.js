import request from '@/utils/axios'

// 获取所有项目
export function getAllProjects() {
  return request({
    url: '/api/Project/AllProjects',
    method: 'get'
  })
}

// 获取项目详情
export function getProjectById(id) {
  return request({
    url: `/api/Project/${id}`,
    method: 'get'
  })
}

// 创建新项目
export function createProject(data) {
  return request({
    url: '/api/Project/Create',
    method: 'post',
    data
  })
}

// 更新项目信息（需要Admin角色）
export function updateProject(id, data) {
  return request({
    url: `/api/Project/Update/${id}`,
    method: 'put',
    data
  })
}

// 删除项目（需要Admin角色）
export function deleteProject(id) {
  return request({
    url: `/api/Project/Delete/${id}`,
    method: 'delete'
  })
}

// 获取项目所有任务
export function getProjectTasks(projectId) {
  return request({
    url: `/api/Project/${projectId}/Tasks`,
    method: 'get'
  })
} 