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
          <el-button size="mini" type="primary" icon="el-icon-upload">上传头像</el-button>
        </el-upload>
      </div>

      <div class="info-section">
        <h2>{{ username }}</h2>
        <p>当前角色：</p>
        <el-tag type="success" size="medium">{{ role }}</el-tag>
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
      avatarUrl: 'https://via.placeholder.com/100', // 默认头像占位
    }
  },
  methods: {
    handleAvatarUpload(response, file) {
      // 模拟上传成功，直接展示上传的本地图片
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
}

.avatar-section {
  text-align: center;
  margin-bottom: 20px;
}

.upload {
  margin-top: 10px;
}

.info-section {
  text-align: center;
}

.info-section h2 {
  margin: 10px 0 5px;
}
</style>
