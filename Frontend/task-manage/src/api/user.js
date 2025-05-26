import request from '@/utils/axios'

// 验证JWT token是否有效
export function verifyToken() {
  return request({
    url: '/User/Auth',
    method: 'get'
  })
}

// 登录
export function login(data) {
  return request({
    url: '/User/Login',
    method: 'post',
    data
  })
}

// 注册
export function register(data) {
  return request({
    url: '/User/Register',
    method: 'post',
    data
  })
} 