import { createRouter, createWebHashHistory } from 'vue-router'
import Login from '../components/login/LoginView.vue'
import Home from '../components/Home/HomeView.vue'


const router = createRouter({
    history: createWebHashHistory(),
    routes: [
        {path: '/login', name:'登录', component: Login, params: {}},
        {path: '/home', name:'主页', component: Home, params: {}}
    ]
}
);

export default router;