<template>
  <el-form
    ref="formRef"
    :model="form"
    label-width="70px"
    class="login-form"
    :rules="formRules"
  >
    <el-form-item
      label="用户名"
      prop="username"
    >
      <el-input
        v-model="form.username"
        placeholder="用户名(3-18位字母数字)"
      />
    </el-form-item>
    <el-form-item
      label="密码"
      prop="password"
    >
      <el-input
        v-model="form.password" placeholder="密码(6-24位数字加字母)"
        show-password type="password"
      />
    </el-form-item>
    <el-form-item
      label="重复密码" prop="password2"
    >
      <el-input
        v-model="form.password2"
        placeholder="重复密码"
        show-password
        type="password"
      />
    </el-form-item>
    <el-form-item class="button-item">
      <el-button
        type="primary" @click="onSubmit"
      >
        登录
      </el-button>
    </el-form-item>
  </el-form>
</template>

<script setup>

import {reactive, ref} from 'vue';

const emit = defineEmits(['register']);
const form = reactive({
  username: '',
  password: '',
  password2: ''
});

const formRules = reactive({
  username: [{
    validator: function (rule, value, callback) {
      if (!value) {
        return callback(new Error('用户名不能是空。'));
      }
      const regex = /^[A-Za-z\d]{3,18}$/;
      const res = regex.test(value);

      if (!res) return callback(new Error('必须是3-18位数字加字母'))
      return callback();
    },
    trigger: 'blur'
  }],
  password2: [{
    validator: function (rule, value, callback) {
      if (value !== form.password) return callback(new Error('两次密码不一致'));
      callback();
    },
    trigger: 'blur'
  }],
  password: [{
    validator: function (rule, value, callback) {
      if (!value) return callback(new Error('密码不能是空'));
      const regex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,24}$/;
      const res = regex.test(value);

      if (!res) return callback(new Error('必须是6-24位数字加字母'))
      return callback();
    },
    trigger: 'blur'
  }]
})

const formRef = ref(null);

const onSubmit = function () {
  formRef.value.validate((valid) => {
    if (valid) {
      emit('register', form);
    }
  })
}

</script>

<style scoped>
.login-form {
  padding: 20px;
  display: flex;
  flex-direction: column;
}

.button-item {
  text-align: center;
}
</style>


