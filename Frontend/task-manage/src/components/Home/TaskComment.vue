<template>
  <div class="comment-section">
    <div class="comment-list">
      <div v-if="comments.length === 0" class="empty-comment">
        <el-empty description="暂无评论" :image-size="100">
          <template #description>
            <p>该任务暂无评论，快来添加第一条评论吧！</p>
          </template>
        </el-empty>
      </div>
      <div v-else v-for="comment in commentWithUsernames" :key="comment.commentId" class="comment-item">
        <div class="comment-header">
          <span class="comment-author">{{ comment.username }}</span>
          <span class="comment-time">{{ formatTime(comment.createdTime) }}</span>
          <el-button 
            v-if="canDeleteComment(comment)"
            type="text" 
            class="delete-btn"
            @click="handleDeleteComment(comment.commentId)"
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
        :disabled="!taskId"
      />
      <el-button 
        type="primary" 
        @click="handleAddComment"
        :disabled="!newComment.trim() || !taskId"
      >
        发表评论
      </el-button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { addTaskComment, deleteTaskComment, getTaskComments } from '@/api/task'
import { getUserIdFromToken, getUserRoleFromToken } from '@/utils/jwtUtils'
import { getUserInfo } from '@/api/user'

/* eslint-disable no-undef */
const props = defineProps({
  taskId: {
    type: Number,
    required: true
  }
})
/* eslint-enable no-undef */

const comments = ref([])
const usernames = ref({}) // 存储用户ID到用户名的映射
const newComment = ref('')
const loading = ref(false)

// 合并评论和用户名的计算属性
const commentWithUsernames = computed(() => {
  return comments.value.map(comment => ({
    ...comment,
    username: usernames.value[comment.userId] || `用户ID: ${comment.userId}`
  }))
})

// 格式化时间
const formatTime = (time) => {
  if (!time) return ''
  const date = new Date(time)
  return date.toLocaleString()
}

// 检查是否可以删除评论
const canDeleteComment = (comment) => {
  // 使用JWT工具函数获取用户ID和角色
  const currentUserId = getUserIdFromToken()
  const userRole = getUserRoleFromToken()
  return userRole === 'ProjectAdmin' || comment.userId === parseInt(currentUserId)
}

// 获取用户名
const fetchUsername = async (userId) => {
  if (!userId || usernames.value[userId]) return
  
  try {
    const response = await getUserInfo(userId)
    const userData = response.data
    
    // 尝试各种可能的用户名属性名
    const possibleProps = ['Username', 'username', 'userName', 'UserName', 'name']
    const usernameKey = Object.keys(userData).find(key => 
      possibleProps.some(prop => key.toLowerCase() === prop.toLowerCase())
    )
    
    if (usernameKey && userData[usernameKey]) {
      usernames.value[userId] = userData[usernameKey]
    } else {
      usernames.value[userId] = `用户ID: ${userId}`
    }
  } catch (error) {
    console.error(`获取用户${userId}信息失败:`, error)
    usernames.value[userId] = `用户ID: ${userId}`
  }
}

// 获取评论列表
const fetchComments = async () => {
  if (!props.taskId) {
    comments.value = []
    return
  }
  
  loading.value = true
  try {
    const response = await getTaskComments(props.taskId)
    console.log('获取到的评论数据:', response.data)
    
    // 确保每个评论都有commentId字段(后端返回的是CommentId)
    comments.value = response.data.map(comment => ({
      commentId: comment.commentId,
      taskId: comment.taskId,
      userId: comment.userId,
      content: comment.content,
      createdTime: comment.createdTime
    }))
    
    // 为每个评论获取用户名
    for (const comment of comments.value) {
      fetchUsername(comment.userId)
    }
  } catch (error) {
    if (error.response && error.response.status === 404) {
      // 如果返回404(没有评论)，设置为空数组而不是报错
      comments.value = []
    } else {
      console.error('获取评论失败:', error)
      comments.value = []
    }
  } finally {
    loading.value = false
  }
}

// 添加评论
const handleAddComment = async () => {
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
    console.error('发表评论失败:', error)
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
      console.error('删除评论失败:', error)
    }
  }
}

// 当taskId变化时重新获取评论
watch(() => props.taskId, (newVal) => {
  if (newVal) {
    fetchComments()
  } else {
    comments.value = []
  }
})

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

.empty-comment {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 40px 0;
  color: #909399;
}
</style> 