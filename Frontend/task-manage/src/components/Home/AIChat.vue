<template>
  <div class="chat-container">
    <el-scrollbar class="message-scrollbar" ref="scrollbar">
      <div class="message-list">
        <div v-for="(message, index) in messages" :key="index" 
             :class="['message-item', message.type === 'user' ? 'user-message' : 'ai-message']">
          <div class="message-content">
            {{ message.content }}
          </div>
        </div>
      </div>
    </el-scrollbar>
    
    <div class="input-area">
      <el-input
        v-model="inputMessage"
        type="textarea"
        :rows="3"
        placeholder="输入指令，例如：将会议TODO转为任务"
        resize="none"
        @keyup.enter.ctrl="sendMessage"
      />
      <el-button 
        type="primary" 
        circle 
        @click="sendMessage"
        class="send-button"
      >
        <el-icon><Position /></el-icon>
      </el-button>
    </div>
  </div>
</template>

<script>
import { Position } from '@element-plus/icons-vue'
import { ref } from 'vue'

export default {
  name: 'AIChat',
  components: {
    Position
  },
  setup() {
    const messages = ref([])
    const inputMessage = ref('')
    const scrollbar = ref(null)

    const sendMessage = () => {
      if (!inputMessage.value.trim()) return
      
      // 添加用户消息
      messages.value.push({
        type: 'user',
        content: inputMessage.value
      })
      
      // 清空输入框
      inputMessage.value = ''
      
      // 模拟AI回复
      setTimeout(() => {
        messages.value.push({
          type: 'ai',
          content: '这是一个AI回复示例'
        })
        // 滚动到底部
        scrollToBottom()
      }, 500)
    }

    const scrollToBottom = () => {
      setTimeout(() => {
        if (scrollbar.value) {
          scrollbar.value.setScrollTop(9999)
        }
      }, 100)
    }

    return {
      messages,
      inputMessage,
      scrollbar,
      sendMessage
    }
  }
}
</script>

<style scoped>
.chat-container {
  height: 100%;
  display: flex;
  flex-direction: column;
  background-color: #fff;
}

.message-scrollbar {
  flex: 1;
  padding: 20px;
}

.message-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.message-item {
  max-width: 80%;
  display: flex;
}

.user-message {
  margin-left: auto;
}

.ai-message {
  margin-right: auto;
}

.message-content {
  padding: 12px 16px;
  border-radius: 8px;
  word-break: break-word;
}

.user-message .message-content {
  background-color: #e8f4ff;
  color: #222;
}

.ai-message .message-content {
  background-color: #f5f7fa;
  color: #333;
}

.input-area {
  padding: 16px;
  background-color: #fff;
  border-top: 1px solid #e4e7ed;
  display: flex;
  gap: 12px;
  align-items: flex-end;
}

.input-area :deep(.el-textarea__inner) {
  background-color: #f5f7fa;
  border-color: #d9ecff;
  color: #222;
}

.input-area :deep(.el-textarea__inner:focus) {
  border-color: #409eff;
}

.send-button {
  margin-bottom: 4px;
}
</style> 