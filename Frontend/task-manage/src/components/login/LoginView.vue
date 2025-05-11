<template>
  <div class="login-container">
    <el-card class="login-card">
      <div class="login-top">登录窗口</div>
      <el-form :model="form" label-width="70px" class="login-form">
        <el-form-item label="用户名">
          <el-input v-model="form.username" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input
            v-model="form.password"
            placeholder="请输入密码"
            show-password
            type="password"
          />
        </el-form-item>
        <el-form-item class="button-item">
          <el-button type="primary" @click="onSubmit">登录</el-button>
        </el-form-item>

        <!-- 新增：注册/找回密码 -->
        <div class="extra-options">
          <el-button type="text" @click="onRegister">注册账号</el-button>
          <el-button type="text" @click="onForgotPassword">找回密码</el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  username: '',
  password: ''
})

const onSubmit = () => {
  console.log('登录数据:', form)
}

const onRegister = () => {
  console.log('点击注册')
  // 你可以跳转注册页：router.push('/register') 等
}

const onForgotPassword = () => {
  console.log('点击找回密码')
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
  width: 300px;
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

.login-form {
  padding: 20px;
  display: flex;
  flex-direction: column;
}

.button-item {
  text-align: center;
}

.extra-options {
  display: flex;
  justify-content: space-between;
  margin-top: 10px;
  padding: 0 10px;
}
</style>
