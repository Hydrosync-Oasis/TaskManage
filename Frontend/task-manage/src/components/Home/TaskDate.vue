<template>
  <div class="task-date-container">
    <div v-if="task" class="task-info">
      <div class="date-item">
        <span class="label">开始日期：</span>
        <span class="value">{{ formatDate(task.startDate) }}</span>
      </div>
      <div class="date-item">
        <span class="label">截止日期：</span>
        <span class="value" :class="{ 'overdue': isOverdue }">{{ formatDate(task.deadline) }}</span>
      </div>
      <div class="date-item" v-if="daysLeft !== null">
        <span class="label">剩余天数：</span>
        <el-tag :type="getTagType" size="small">{{ daysLeft }} 天</el-tag>
      </div>
      
      <div class="date-update" v-if="canUpdateDates">
        <el-divider>更新日期</el-divider>
        <el-form :model="dateForm" label-width="100px">
          <el-form-item label="开始日期">
            <el-date-picker 
              v-model="dateForm.startDate" 
              type="date" 
              placeholder="选择开始日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
          <el-form-item label="截止日期">
            <el-date-picker 
              v-model="dateForm.deadline" 
              type="date" 
              placeholder="选择截止日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="handleUpdateDates" :loading="updating">
              更新日期
            </el-button>
          </el-form-item>
        </el-form>
      </div>
    </div>
    <div v-else class="no-task">
      请从图表中选择一个任务节点
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
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

const dateForm = ref({
  startDate: '',
  deadline: ''
})

const updating = ref(false)
const canUpdateDates = ref(false)

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return '未设置'
  const date = new Date(dateString)
  return date.toLocaleDateString()
}

// 计算是否已过期
const isOverdue = ref(false)

// 计算剩余天数
const daysLeft = computed(() => {
  if (!props.task?.deadline) return null
  
  const deadline = new Date(props.task.deadline)
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  
  const diffTime = deadline - today
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  
  return diffDays
})

// 获取标签类型
const getTagType = computed(() => {
  if (daysLeft.value === null) return ''
  if (daysLeft.value < 0) return 'danger'
  if (daysLeft.value <= 3) return 'warning'
  if (daysLeft.value <= 7) return 'info'
  return 'success'
})

// 处理更新日期
const handleUpdateDates = async () => {
  if (!props.task) return
  
  // 验证日期
  if (dateForm.value.startDate && dateForm.value.deadline) {
    const start = new Date(dateForm.value.startDate)
    const end = new Date(dateForm.value.deadline)
    if (start > end) {
      ElMessage.error('开始日期不能晚于截止日期')
      return
    }
  }
  
  updating.value = true
  try {
    await updateTask({
      id: props.task.id,
      startDate: dateForm.value.startDate || undefined,
      deadline: dateForm.value.deadline || undefined
    })
    
    ElMessage.success('任务日期更新成功')
  } catch (error) {
    ElMessage.error('更新任务日期失败')
    console.error('更新任务日期失败:', error)
  } finally {
    updating.value = false
  }
}

// 当任务变化时，更新日期数据和权限
watch(() => props.task, (newTask) => {
  if (newTask) {
    if (newTask.deadline) {
      const deadline = new Date(newTask.deadline)
      isOverdue.value = deadline < new Date() && newTask.status !== 2 // 2表示已完成
    } else {
      isOverdue.value = false
    }
    
    // 检查是否有权限更新日期
    const currentUserId = getUserIdFromToken()
    const userRole = getUserRoleFromToken()
    canUpdateDates.value = newTask.createUserId === parseInt(currentUserId) || 
                          userRole === 'ProjectAdmin'
  } else {
    isOverdue.value = false
    canUpdateDates.value = false
  }
}, { immediate: true })
</script>

<style scoped>
.task-date-container {
  padding: 15px;
}

.task-info {
  background-color: #f5f7fa;
  border-radius: 8px;
  padding: 15px;
  border: 1px solid #e4e7ed;
}

.date-item {
  margin-bottom: 10px;
  display: flex;
  align-items: center;
}

.label {
  font-weight: bold;
  color: #606266;
  margin-right: 10px;
  min-width: 80px;
}

.value {
  color: #303133;
}

.value.overdue {
  color: #f56c6c;
  font-weight: bold;
}

.date-update {
  margin-top: 20px;
}

.no-task {
  color: #909399;
  text-align: center;
  padding: 20px 0;
  font-style: italic;
}
</style>