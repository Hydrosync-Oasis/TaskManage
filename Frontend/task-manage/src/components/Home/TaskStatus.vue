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
import { ref, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { updateTask } from '@/api/task'

/* eslint-disable no-undef */
const props = defineProps({
  task: {
    type: Object,
    default: null
  }
})
/* eslint-enable no-undef */

const selectedStatus = ref('')
const canUpdateStatus = ref(false)

// 状态选项
const statusOptions = [
  { value: 'NotStarted', label: '未开始' },
  { value: 'InProgress', label: '进行中' },
  { value: 'Completed', label: '已完成' },
  { value: 'Blocked', label: '已阻塞' },
  { value: 'Deferred', label: '已延期' }
]

// 根据状态获取显示文本
const getStatusText = (status) => {
  const option = statusOptions.find(item => item.value === status)
  return option ? option.label : '未知状态'
}

// 根据状态获取标签类型
const getStatusType = (status) => {
  switch (status) {
    case 'NotStarted': return 'info'
    case 'InProgress': return 'warning'
    case 'Completed': return 'success'
    case 'Blocked': return 'danger'
    case 'Deferred': return ''
    default: return 'info'
  }
}

// 处理状态变更
const handleStatusChange = async () => {
  if (!props.task || !selectedStatus.value) return
  
  try {
    await updateTask({
      id: props.task.id,
      status: selectedStatus.value
    })
    ElMessage.success('任务状态更新成功')
  } catch (error) {
    ElMessage.error('更新任务状态失败')
    console.error('更新任务状态失败:', error)
    // 恢复原状态
    selectedStatus.value = props.task.status
  }
}

// 当任务变化时，更新选中的状态
watch(() => props.task, (newTask) => {
  if (newTask) {
    selectedStatus.value = newTask.status
    
    // 检查是否有权限更新状态（这里可以根据实际权限逻辑调整）
    const currentUserId = localStorage.getItem('userId')
    canUpdateStatus.value = newTask.createUserId === parseInt(currentUserId) || 
                           localStorage.getItem('userRole') === 'ProjectAdmin'
  } else {
    selectedStatus.value = ''
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