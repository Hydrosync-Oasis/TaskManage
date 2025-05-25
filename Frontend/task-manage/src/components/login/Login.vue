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
        placeholder="请输入用户名"
      />
    </el-form-item>
    <el-form-item
      label="密码" prop="password"
    >
      <el-input
        v-model="form.password" placeholder="请输入密码"
        show-password type="password"
      />
    </el-form-item>
    <el-form-item class="button-item">
      <el-button
        type="primary"
        @click="onSubmit"
      >
        登录
      </el-button>
    </el-form-item>
  </el-form>
</template>

<script setup>

import { reactive, ref } from 'vue';

// eslint-disable-next-line no-undef
const emit = defineEmits(['login']);
const form = reactive({
    username: '',
    password: ''
});

const formRules = reactive({
    username: [{
        validator: function (rule, value, callback) {
            if (!value) {
                return callback(new Error('用户名不能是空。'));
            }
            return callback();
        },
        trigger: 'blur'
    }],
    password: [{
        validator: function(rule, value, callback) {
            if (!value) return callback(new Error('密码不能是空'));

            return callback();
        },
        trigger: 'blur'
    }]
})

const formRef = ref(null);

const onSubmit = function() {
    formRef.value.validate((valid) => {
        if (valid) {
            emit('login', form);
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


