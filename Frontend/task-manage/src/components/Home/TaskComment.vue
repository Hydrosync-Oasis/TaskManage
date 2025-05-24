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
import { ref, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { addTaskComment, getTaskComment, deleteTaskComment } from '@/api/task'

/* eslint-disable no-undef */
const props = defineProps({
  taskId: {
    type: [Number, String],
    default: null
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
  if (!props.taskId) {
    comments.value = []
    return
  }
  
  try {
    const response = await getTaskComment(props.taskId)
    comments.value = response.data
  } catch (error) {
    ElMessage.error('获取评论失败')
    console.error('Failed to fetch comments:', error)
    comments.value = []
  }
}

// 添加评论
const handleAddComment = async () => {
  if (!props.taskId) {
    ElMessage.warning('请先选择一个任务')
    return
  }
  
  if (!newComment.value.trim()) {
    ElMessage.warning('评论内容不能为空')
    return
  }

  try {
    await addTaskComment({
      taskId: props.taskId,
      content: newComment.value.trim()
    })
    
    ElMessage.success('评论发表成功')
    newComment.value = ''
    await fetchComments() // 刷新评论列表
  } catch (error) {
    ElMessage.error('评论发表失败')
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

    await deleteTaskComment(commentId)

    ElMessage.success('评论删除成功')
    await fetchComments() // 刷新评论列表
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('评论删除失败')
      console.error('Failed to delete comment:', error)
    }
  }
}

// 监听任务ID变化，重新获取评论
watch(() => props.taskId, (newTaskId) => {
  if (newTaskId) {
    fetchComments()
  } else {
    comments.value = []
  }
}, { immediate: true })

onMounted(() => {
  if (props.taskId) {
    fetchComments()
  }
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