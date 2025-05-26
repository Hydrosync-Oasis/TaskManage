/**
 * 解析JWT Token
 * @param {string} token - JWT token字符串
 * @returns {Object} 解析后的payload部分
 */
export function parseJwt(token) {
  if (!token) return null;
  
  try {
    // 将JWT token分割为三部分：header.payload.signature
    const base64Url = token.split('.')[1];
    // base64解码
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    // 解码payload部分并解析为JSON对象
    const payload = JSON.parse(window.atob(base64));
    return payload;
  } catch (error) {
    console.error('解析JWT token失败:', error);
    return null;
  }
}

/**
 * 从JWT Token中获取用户ID
 * @returns {string|null} 用户ID或null
 */
export function getUserIdFromToken() {
  const token = localStorage.getItem('token');
  if (!token) return null;
  
  const payload = parseJwt(token);
  if (!payload) return null;
  
  // ClaimTypes.NameIdentifier在JWT中通常是'nameid'或'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
  return payload.nameid || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
}

/**
 * 从JWT Token中获取用户角色
 * @returns {string|null} 用户角色或null
 */
export function getUserRoleFromToken() {
  const token = localStorage.getItem('token');
  if (!token) return null;
  
  const payload = parseJwt(token);
  if (!payload) return null;
  
  // ClaimTypes.Role在JWT中通常是'role'或'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
  return payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
}

/**
 * 从JWT Token中获取用户名
 * @returns {string|null} 用户名或null
 */
export function getUsernameFromToken() {
  const token = localStorage.getItem('token');
  if (!token) return null;
  
  const payload = parseJwt(token);
  if (!payload) return null;
  
  // ClaimTypes.Name在JWT中通常是'unique_name'或'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
  return payload.unique_name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
} 