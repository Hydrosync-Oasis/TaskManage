<template>
  <div class="comment-section">
    <div class="comment-list">
      <div v-for="comment in comments" :key="comment.id" class="comment-item">
        <div class="comment-header">
          <span class="comment-author">{{ comment.owner?.username || '未知用户' }}</span>
          <span class="comment-time">{{ formatTime(comment.createdTime) }}</span>
        </div>
        <div class="comment-content">{{ comment.content }}</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
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

// 格式化时间
const formatTime = (time) => {
  if (!time) return ''
  const date = new Date(time)
  return date.toLocaleString()
}

// 获取评论列表
const fetchComments = async () => {
  try {
    const response = await request({
      url: `/api/Comment/${props.taskId}`,
      method: 'get'
    })
    comments.value = response.data
  } catch (error) {
    ElMessage.error('获取评论失败')
    console.error('Failed to fetch comments:', error)
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

.comment-content {
  color: #333;
  line-height: 1.5;
}
</style> 