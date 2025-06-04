<template>
  <div class="task-status-container">
    <div v-if="task" class="task-info">
      <div class="status-display">
        <span class="label">当前状态：</span>
        <el-tag :type="getStatusType(task.status)" size="large">
          {{ getStatusText(task.status) }}
        </el-tag>
      </div>
      
      <div class="status-update" v-if="canUpdateStatus">
        <span class="label">更新状态：</span>
        <el-select v-model="selectedStatus" placeholder="选择新状态" size="large" @change="handleStatusChange">
          <el-option
            v-for="item in statusOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </div>
    </div>
    <div v-else class="no-task">
      请从图表中选择一个任务节点
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, defineEmits } from 'vue'
import { ElMessage } from 'element-plus'
import { updateTask } from '@/api/task'
import { getUserIdFromToken, getUserRoleFromToken } from '@/utils/jwtUtils'

/* eslint-disable no-undef */
const props = defineProps({
  task: {
    type: Object,
    default: null
  }
})
/* eslint-enable no-undef */

const selectedStatus = ref(0)
const canUpdateStatus = ref(false)

// 状态选项 - 使用整数值
const statusOptions = [
  { value: 0, label: '未开始(Ready)' },
  { value: 1, label: '进行中(Doing)' },
  { value: 2, label: '已完成(Done)' }
]

// 根据状态获取显示文本
const getStatusText = (status) => {
  // 确保状态是数字
  const statusNum = Number(status)
  const option = statusOptions.find(item => item.value === statusNum)
  return option ? option.label : '未知状态'
}

// 根据状态获取标签类型
const getStatusType = (status) => {
  // 确保状态是数字
  const statusNum = Number(status)
  switch (statusNum) {
    case 0: return 'info'    // Ready
    case 1: return 'warning' // Doing
    case 2: return 'success' // Done
    default: return 'info'
  }
}

const emit = defineEmits(['task-updated'])

// 处理状态变更
const handleStatusChange = async () => {
  if (!props.task || selectedStatus.value === null) return
  
  try {
    await updateTask({
      id: props.task.id,
      status: selectedStatus.value
    })
    emit('task-updated')
    ElMessage.success('任务状态更新成功')
  } catch (error) {
    ElMessage.error('更新任务状态失败')
    console.error('更新任务状态失败:', error)
    // 恢复原状态
    selectedStatus.value = Number(props.task.status)
  }
}

// 当任务变化时，更新状态和权限
watch(() => props.task, (newTask) => {
  if (newTask) {
    // 确保状态是数字
    selectedStatus.value = Number(newTask.status)
    
    // 检查是否有权限更新状态
    const currentUserId = getUserIdFromToken()
    const userRole = getUserRoleFromToken()
    canUpdateStatus.value = newTask.createUserId === parseInt(currentUserId) || 
                           userRole === 'ProjectAdmin'
  } else {
    selectedStatus.value = 0
    canUpdateStatus.value = false
  }
}, { immediate: true })
</script>

<style scoped>
.task-status-container {
  padding: 15px;
}

.task-info {
  background-color: #f5f7fa;
  border-radius: 8px;
  padding: 15px;
  border: 1px solid #e4e7ed;
}

.status-display {
  margin-bottom: 15px;
  display: flex;
  align-items: center;
}

.status-update {
  margin-top: 15px;
}

.label {
  font-weight: bold;
  color: #606266;
  margin-right: 10px;
}

.no-task {
  color: #909399;
  text-align: center;
  padding: 20px 0;
  font-style: italic;
}
</style>