import request from '@/utils/axios'

// 获取所有项目
export function getAllProjects() {
  return request({
    url: '/api/Project',
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
    url: '/api/Project',
    method: 'post',
    data
  })
}

// 更新项目信息（需要Admin角色）
export function updateProject(id, data) {
  return request({
    url: `/api/Project/${id}`,
    method: 'put',
    data
  })
}

// 删除项目（需要Admin角色）
export function deleteProject(id) {
  return request({
    url: `/api/Project/${id}`,
    method: 'delete'
  })
} 