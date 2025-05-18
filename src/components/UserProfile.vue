<template>
  <div class="profile-container">
    <el-card class="box-card">
      <div class="avatar-section">
        <el-avatar :size="100" :src="avatarUrl" shape="circle"></el-avatar>
        <el-upload
          class="upload"
          action="#"
          :show-file-list="false"
          :on-success="handleAvatarUpload"
          :before-upload="beforeUpload"
        >
          <el-button size="mini" type="primary" icon="el-icon-upload">更换头像</el-button>
        </el-upload>
      </div>

      <div class="info-section">
        <el-form label-position="top" label-width="100%">
          <el-form-item label="姓名">
            <el-input v-model="username" disabled></el-input>
          </el-form-item>

          <el-form-item label="当前角色">
            <el-tag type="success" size="medium">{{ role }}</el-tag>
          </el-form-item>

          <el-form-item label="邮箱">
            <el-input
              v-model="email"
              prefix-icon="el-icon-message"
              placeholder="请输入邮箱"
              clearable
            ></el-input>
          </el-form-item>
        </el-form>
      </div>
    </el-card>
  </div>
</template>

<script>
export default {
  name: 'UserProfile',
  data() {
    return {
      username: '张三',
      role: '管理员',
      email: 'zhangsan@example.com',
      avatarUrl: 'https://via.placeholder.com/100', // 默认头像
    }
  },
  methods: {
    handleAvatarUpload(response, file) {
      this.avatarUrl = URL.createObjectURL(file.raw)
    },
    beforeUpload(file) {
      const isImage = file.type.startsWith('image/')
      const isLt2M = file.size / 1024 / 1024 < 2

      if (!isImage) {
        this.$message.error('只能上传图片文件！')
      }
      if (!isLt2M) {
        this.$message.error('图片大小不能超过 2MB！')
      }
      return isImage && isLt2M
    }
  }
}
</script>

<style scoped>
.profile-container {
  max-width: 500px;
  margin: 50px auto;
}

.box-card {
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 4px 18px rgba(0, 0, 0, 0.08);
}

.avatar-section {
  text-align: center;
  margin-bottom: 25px;
}

.upload {
  margin-top: 12px;
}

.info-section {
  padding: 0 10px;
}

.el-form-item {
  margin-bottom: 20px;
}
</style>
