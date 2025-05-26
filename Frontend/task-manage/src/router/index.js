import { createRouter, createWebHashHistory } from 'vue-router'
import Login from '../components/login/LoginView.vue'
import Home from '../components/Home/HomeView.vue'
import { verifyToken } from '../api/user'

const router = createRouter({
    history: createWebHashHistory(),
    routes: [
        { path: '/login', name: 'Login', component: Login,
            props: route => {
                return {
                    error: route.query.error,
                    source: route.query.source
                }
            }
        },
        { path: '/home', name: 'Home', component: Home },
        {
            path: '/',
            redirect: '/home'
        }
    ]
});

router.beforeEach(async (to, from) => {
    if (to.name !== 'Login') {
        // 检查是否有token
        const token = localStorage.getItem('token')
        if (!token) {
            return {
                path: '/login',
                query: {
                    error: true,
                    source: to.fullPath
                }
            };
        }
        
        try {
            // 验证token是否有效
            await verifyToken()
            // token有效，继续导航
            return true
        } catch (error) {
            // token无效，清除本地token并跳转到登录页
            localStorage.removeItem('token')
            return {
                path: '/login',
                query: {
                    error: true,
                    source: to.fullPath
                }
            };
        }
    }
    return true
})

export default router