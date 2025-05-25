<template>
  <div class="login-container">
    <el-card class="login-card">
      <div class="login-top">
        {{ title }}
      </div>

      <component
        :is="title !== '登录' ? Login : Register"
        @login="onLogin"
        @register="onRegister"
      />

      <!-- 新增：注册/找回密码 -->
      <div class="extra-options">
        <el-button
          type="text"
          @click="handleLoginRegister"
        >
          {{ title }}
        </el-button>
        <el-button
          type="text"
          @click="onForgotPassword"
        >
          找回密码
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import {onMounted, ref} from 'vue'
import Login from './Login.vue';
import Register from "@/components/login/Register.vue";
import axios from 'axios';
import {useRoute, useRouter} from "vue-router";
import {ElMessage} from 'element-plus';

let props = defineProps({
  error: String,
  source: String
});
const title = ref("注册");

const routeData = useRoute();
const router = useRouter();

onMounted(function () {
  if (props.error) {
    ElMessage({
      message: '你还未登录，登陆后自动转到你输入的网页。',
      type: 'error'
    })
  }
});


const handleLoginRegister = () => {
  if (title.value === '登录') {
    title.value = '注册';
  } else {
    title.value = '登录';
  }
}

const onRegister = (form) => {
  axios.post('https://localhost:7062/User/Register', {
    username: form.username,
    password: form.password,
  }).then((res) => {
        if (res.status === 200) {
          ElMessage({
            message: '注册成功，请登录',
            type: 'success'
          });
          title.value = '注册';
        } else {
          ElMessage({
            message: res.data,
            type: 'error'
          })
        }
      }
  )
      .catch((reason) => {
        ElMessage({
          message: reason.response.data,
          type: "error"
        })
      })
}

const onLogin = (form) => {
  axios.post('https://localhost:7062/User/Login', {
    username: form.username,
    password: form.password,
  }).then((res) => {
        if (res.status === 200) {
          localStorage.setItem('token', res.data);
          let source = routeData.query['source'];
          if (!source) source = '/home';
          router.push({
            path: source
          });
        }
      }
  )
      .catch((reason) => {
        ElMessage({
          message: reason.response.data,
          type: "error"
        })
      })
}

const onForgotPassword = () => {
  ElMessage({
    message: '目前未实现',
    type: 'error'
  });
  // 可以跳转到找回密码页
}


</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: #369;
}

.login-card {
  width: 500px;
  padding: 0;
  border-radius: 10px;
  background: #eee;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}

.login-top {
  background: #136;
  color: #fff;
  text-align: center;
  line-height: 50px;
  font-size: 18px;
  border-top-left-radius: 10px;
  border-top-right-radius: 10px;
}


.extra-options {
  display: flex;
  justify-content: space-between;
  margin-top: 10px;
  padding: 0 10px;
}
</style>
