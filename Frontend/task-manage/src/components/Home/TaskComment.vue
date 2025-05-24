<template>
  <div class="comment-section">
    <div class="comment-list">
      <div v-for="comment in comments" :key="comment.id" class="comment-item">
        <div class="comment-header">
          <span class="comment-author">{{ comment.owner?.username || '未知用户' }}</span>
          <span class="comment-time">{{ formatTime(comment.createdTime) }}</span>
          <el-button 
            v-if="canDeleteComment(comment)"
            type="text" 
            class="delete-btn"
            @click="handleDeleteComment(comment.id)"
          >
            删除
          </el-button>
        </div>
        <div class="comment-content">{{ comment.content }}</div>
      </div>
    </div>
    
    <div class="comment-input">
      <el-input
        v-model="newComment"
        type="textarea"
        :rows="3"
        placeholder="请输入评论内容"
      />
      <el-button 
        type="primary" 
        @click="handleAddComment"
        :disabled="!newComment.trim()"
      >
        发表评论
      </el-button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import request from '@/utils/axios'

/* eslint-disable no-undef */
const props = defineProps({
  taskId: {
    type: Number,
    required: true
  }
})
/* eslint-enable no-undef */

const comments = ref([])
const newComment = ref('')

// 格式化时间
const formatTime = (time) => {
  if (!time) return ''
  const date = new Date(time)
  return date.toLocaleString()
}

// 检查是否可以删除评论
const canDeleteComment = (comment) => {
  const currentUserId = localStorage.getItem('userId')
  const userRole = localStorage.getItem('userRole')
  return userRole === 'ProjectAdmin' || comment.owner?.id === parseInt(currentUserId)
}

// 获取评论列表
const fetchComments = async () => {
  try {
    const response = await request({
      url: `/api/Task/comment/task/${props.taskId}`,
      method: 'get'
    })
    comments.value = response.data
  } catch (error) {
    // 错误已由响应拦截器处理，这里可以添加额外的处理逻辑
    console.error('Failed to fetch comments:', error)
    comments.value = [] // 确保评论列表为空数组而不是undefined
  }
}

// 添加评论
const handleAddComment = async () => {
  if (!newComment.value.trim()) {
    ElMessage.warning('评论内容不能为空')
    return
  }

  try {
    await request({
      url: '/api/Task/comment/add',
      method: 'post',
      data: {
        taskId: props.taskId,
        content: newComment.value.trim()
      }
    })
    
    ElMessage.success('评论发表成功')
    newComment.value = ''
    await fetchComments() // 刷新评论列表
  } catch (error) {
    console.error('Failed to add comment:', error)
  }
}

// 删除评论
const handleDeleteComment = async (commentId) => {
  try {
    await ElMessageBox.confirm('确定要删除这条评论吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    await request({
      url: `/api/Task/comment/${commentId}`,
      method: 'delete'
    })

    ElMessage.success('评论删除成功')
    await fetchComments() // 刷新评论列表
  } catch (error) {
    if (error !== 'cancel') {
      console.error('Failed to delete comment:', error)
    }
  }
}

onMounted(() => {
  fetchComments()
})
</script>

<style scoped>
.comment-section {
  padding: 20px;
}

.comment-list {
  margin-bottom: 20px;
}

.comment-item {
  padding: 15px;
  border-bottom: 1px solid #eee;
}

.comment-header {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
}

.comment-author {
  font-weight: bold;
  margin-right: 10px;
}

.comment-time {
  color: #999;
  font-size: 12px;
  margin-right: 10px;
}

.delete-btn {
  margin-left: auto;
  color: #f56c6c;
}

.comment-content {
  color: #333;
  line-height: 1.5;
}

.comment-input {
  margin-top: 20px;
}

.comment-input .el-button {
  margin-top: 10px;
  float: right;
}
</style> 