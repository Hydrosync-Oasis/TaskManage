<template>
  <div class="task-priority-container">
    <div v-if="task" class="task-info">
      <div class="priority-display">
        <span class="label">优先级：</span>
        <el-rate
          v-model="priority"
          disabled
          show-score
          text-color="#ff9900"
          score-template="{value}"
        />
      </div>
      
      <div class="priority-update" v-if="canUpdatePriority">
        <el-divider>更新优先级</el-divider>
        <div class="update-form">
          <el-rate
            v-model="newPriority"
            show-score
            text-color="#ff9900"
            score-template="{value}"
          />
          <el-button type="primary" size="small" @click="handleUpdatePriority" :loading="updating">
            更新
          </el-button>
        </div>
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

const priority = ref(0)
const newPriority = ref(0)
const updating = ref(false)
const canUpdatePriority = ref(false)

// 处理更新优先级
const handleUpdatePriority = async () => {
  if (!props.task) return
  
  updating.value = true
  try {
    await updateTask({
      id: props.task.id,
      priority: newPriority.value
    })
    
    ElMessage.success('任务优先级更新成功')
    priority.value = newPriority.value
  } catch (error) {
    ElMessage.error('更新任务优先级失败')
    console.error('更新任务优先级失败:', error)
    newPriority.value = priority.value
  } finally {
    updating.value = false
  }
}

// 当任务变化时，更新优先级数据
watch(() => props.task, (newTask) => {
  if (newTask) {
    priority.value = newTask.priority || 0
    newPriority.value = newTask.priority || 0
    
    // 检查是否有权限更新优先级（这里可以根据实际权限逻辑调整）
    const currentUserId = localStorage.getItem('userId')
    canUpdatePriority.value = newTask.createUserId === parseInt(currentUserId) || 
                             localStorage.getItem('userRole') === 'ProjectAdmin'
  } else {
    priority.value = 0
    newPriority.value = 0
    canUpdatePriority.value = false
  }
}, { immediate: true })
</script>

<style scoped>
.task-priority-container {
  padding: 15px;
}

.task-info {
  background-color: #f5f7fa;
  border-radius: 8px;
  padding: 15px;
  border: 1px solid #e4e7ed;
}

.priority-display {
  display: flex;
  align-items: center;
}

.label {
  font-weight: bold;
  color: #606266;
  margin-right: 10px;
}

.priority-update {
  margin-top: 20px;
}

.update-form {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.no-task {
  color: #909399;
  text-align: center;
  padding: 20px 0;
  font-style: italic;
}
</style> 