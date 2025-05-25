import { createRouter, createWebHashHistory } from 'vue-router'
import Login from '../components/login/LoginView.vue'
import Home from '../components/Home/HomeView.vue'


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

router.beforeEach((to, from) => {
    if (to.name !== 'Login') {
        if (!localStorage.getItem('token')) {
            return { 
                path: '/Login',
                query: {
                    error: true,
                    source: from.path
                }
            };
        }
    }
})

export default router;