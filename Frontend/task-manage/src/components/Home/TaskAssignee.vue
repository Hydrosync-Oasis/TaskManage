<template>
  <div class="task-assignee-container">
    <div v-if="task" class="task-info">
      <div class="assignee-display">
        <span class="label">创建人：</span>
        <span class="value">{{ task.createUserName || '未知' }}</span>
      </div>
      <div class="assignee-display">
        <span class="label">负责人：</span>
        <span class="value">{{ task.assigneeName || '未分配' }}</span>
      </div>
      
      <div class="assignee-update" v-if="canUpdateAssignee">
        <el-divider>更新负责人</el-divider>
        <div class="update-form">
          <el-select v-model="selectedAssignee" placeholder="选择负责人" style="width: 100%">
            <el-option
              v-for="user in userOptions"
              :key="user.id"
              :label="user.name"
              :value="user.id"
            />
          </el-select>
          <el-button type="primary" style="margin-top: 10px" @click="handleUpdateAssignee" :loading="updating">
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
import { ref, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { updateTask } from '@/api/task'
import { getUserInfo } from '@/api/user'
import { getUserIdFromToken, getUserRoleFromToken } from '@/utils/jwtUtils'

/* eslint-disable no-undef */
const props = defineProps({
  task: {
    type: Object,
    default: null
  }
})
/* eslint-enable no-undef */

const selectedAssignee = ref('')
const updating = ref(false)
const canUpdateAssignee = ref(false)

// 模拟用户列表，实际应从后端获取
const userOptions = ref([
  { id: 1, name: '张三' },
  { id: 2, name: '李四' },
  { id: 3, name: '王五' },
  { id: 4, name: '赵六' }
])

// 处理更新负责人
const handleUpdateAssignee = async () => {
  if (!props.task) return
  
  updating.value = true
  try {
    await updateTask({
      id: props.task.id,
      assigneeId: selectedAssignee.value || null
    })
    
    ElMessage.success('任务负责人更新成功')
  } catch (error) {
    ElMessage.error('更新任务负责人失败')
    console.error('更新任务负责人失败:', error)
  } finally {
    updating.value = false
  }
}

// 当任务变化时，更新权限
watch(() => props.task, (newTask) => {
  if (newTask) {
    // 检查是否有权限更新任务负责人
    const currentUserId = getUserIdFromToken()
    const userRole = getUserRoleFromToken()
    canUpdateAssignee.value = newTask.createUserId === parseInt(currentUserId) || 
                              userRole === 'ProjectAdmin'
  } else {
    selectedAssignee.value = ''
    canUpdateAssignee.value = false
  }
}, { immediate: true })
</script>

<style scoped>
.task-assignee-container {
  padding: 15px;
}

.task-info {
  background-color: #f5f7fa;
  border-radius: 8px;
  padding: 15px;
  border: 1px solid #e4e7ed;
}

.assignee-display {
  margin-bottom: 10px;
}

.assignee-display:last-of-type {
  margin-bottom: 0;
}

.label {
  font-weight: bold;
  color: #606266;
  margin-right: 10px;
}

.value {
  color: #303133;
}

.assignee-update {
  margin-top: 20px;
}

.update-form {
  display: flex;
  flex-direction: column;
}

.no-task {
  color: #909399;
  text-align: center;
  padding: 20px 0;
  font-style: italic;
}
</style>