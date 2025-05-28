<template>
  <div class="task-predecessor-container">
    <div v-if="task" class="task-info">
      <div v-if="hasDependencies" class="dependencies-list">
        <div class="section-title">前驱任务：</div>
        <el-table :data="dependentNodes" style="width: 100%">
          <el-table-column prop="id" label="ID" width="60" />
          <el-table-column prop="title" label="任务名称" />
          <el-table-column label="操作" width="80">
            <template #default="scope">
              <el-button 
                type="danger" 
                size="small" 
                icon="Delete" 
                circle
                @click="handleRemoveDependency(scope.row)"
                v-if="canUpdateDependencies"
              />
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div v-else class="no-dependencies">
        该任务没有前驱节点
      </div>
      
      <div class="dependency-update" v-if="canUpdateDependencies">
        <el-divider>添加前驱节点</el-divider>
        <div class="update-form">
          <el-select v-model="selectedDependency" placeholder="选择前驱任务" style="width: 100%">
            <el-option
              v-for="availableTask in availableTasks"
              :key="availableTask.id"
              :label="`${availableTask.id} - ${availableTask.title}`"
              :value="availableTask.id"
            />
          </el-select>
          <el-button type="primary" style="margin-top: 10px" @click="handleAddDependency" :loading="updating">
            添加
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
import { ref, computed, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { getTaskInfo, updateTask } from '@/api/task'
import { getUserIdFromToken, getUserRoleFromToken } from '@/utils/jwtUtils'

/* eslint-disable no-undef */
const props = defineProps({
  task: {
    type: Object,
    default: null
  },
  allTasks: {
    type: Array,
    default: () => []
  }
})
/* eslint-enable no-undef */

const dependentNodes = ref([])
const selectedDependency = ref('')
const updating = ref(false)
const canUpdateDependencies = ref(false)

// 计算是否有依赖项
const hasDependencies = computed(() => {
  return dependentNodes.value && dependentNodes.value.length > 0
})

// 计算可选的前驱任务（排除自己和已有的依赖）
const availableTasks = computed(() => {
  if (!props.task) return []
  
  // 使用从父组件传入的所有任务列表
  return props.allTasks.filter(task => {
    // 排除自己
    if (task.id === props.task.id) return false
    
    // 排除已有的依赖
    const alreadyDependent = props.task.dependencyTaskIds && 
                           props.task.dependencyTaskIds.includes(task.id)
    return !alreadyDependent
  })
})

// 获取前驱任务的详细信息
const fetchDependentTasksInfo = async () => {
  if (!props.task || !props.task.dependencyTaskIds || !props.task.dependencyTaskIds.length) {
    dependentNodes.value = []
    return
  }
  
  try {
    const promises = props.task.dependencyTaskIds.map(id => getTaskInfo(id))
    const responses = await Promise.all(promises)
    dependentNodes.value = responses.map(res => res.data)
  } catch (error) {
    console.error('获取前驱任务详情失败:', error)
    ElMessage.error('获取前驱任务详情失败')
  }
}

// 处理添加前驱节点
const handleAddDependency = async () => {
  if (!props.task || !selectedDependency.value) return
  
  updating.value = true
  try {
    // 准备更新的数据
    const taskData = {
      id: props.task.id,
      dependencyTaskIds: [...(props.task.dependencyTaskIds || []), selectedDependency.value]
    }
    
    // 调用API更新任务
    await updateTask(taskData)
    
    // 更新本地状态
    ElMessage.success('前驱节点添加成功')
    selectedDependency.value = ''
    
    // 触发父组件刷新任务列表
    emit('task-updated')
  } catch (error) {
    ElMessage.error('添加前驱节点失败')
    console.error('添加前驱节点失败:', error)
  } finally {
    updating.value = false
  }
}

// 处理移除前驱节点
const handleRemoveDependency = async (dependency) => {
  if (!props.task) return
  
  updating.value = true
  try {
    // 准备更新的数据
    const taskData = {
      id: props.task.id,
      dependencyTaskIds: (props.task.dependencyTaskIds || []).filter(id => id !== dependency.id)
    }
    
    // 调用API更新任务
    await updateTask(taskData)
    
    // 更新本地状态
    ElMessage.success('前驱节点移除成功')
    
    // 触发父组件刷新任务列表
    emit('task-updated')
  } catch (error) {
    ElMessage.error('移除前驱节点失败')
    console.error('移除前驱节点失败:', error)
  } finally {
    updating.value = false
  }
}

// 当任务变化时，获取前驱节点数据
watch(() => props.task, (newTask) => {
  if (newTask) {
    fetchDependentTasksInfo()
    
    // 检查是否有权限更新前驱节点（使用jwtUtils获取用户信息）
    const currentUserId = getUserIdFromToken()
    const userRole = getUserRoleFromToken()
    canUpdateDependencies.value = newTask.createUserId === parseInt(currentUserId) || 
                                userRole === 'ProjectAdmin'
  } else {
    dependentNodes.value = []
    canUpdateDependencies.value = false
  }
}, { immediate: true })

// 定义emit以便通知父组件任务已更新
const emit = defineEmits(['task-updated'])
</script>

<style scoped>
.task-predecessor-container {
  padding: 15px;
}

.task-info {
  background-color: #f5f7fa;
  border-radius: 8px;
  padding: 15px;
  border: 1px solid #e4e7ed;
}

.section-title {
  font-weight: bold;
  color: #606266;
  margin-bottom: 10px;
}

.no-dependencies {
  color: #909399;
  font-style: italic;
  padding: 10px 0;
}

.dependency-update {
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