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
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
// 移除未使用的导入
// import { updateTask } from '@/api/task'

/* eslint-disable no-undef */
const props = defineProps({
  task: {
    type: Object,
    default: null
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
  
  // 这里应该是从父组件传入的所有任务列表
  // 为了演示，我们使用模拟数据
  const allTasks = [
    { id: 1, title: '需求分析' },
    { id: 2, title: '系统设计' },
    { id: 3, title: '前端开发' },
    { id: 4, title: '后端开发' },
    { id: 5, title: '测试' }
  ]
  
  // 排除自己和已有的依赖
  return allTasks.filter(task => {
    // 排除自己
    if (task.id === props.task.id) return false
    
    // 排除已有的依赖
    const alreadyDependent = dependentNodes.value.some(dep => dep.id === task.id)
    return !alreadyDependent
  })
})

// 处理添加前驱节点
const handleAddDependency = async () => {
  if (!props.task || !selectedDependency.value) return
  
  updating.value = true
  try {
    // 这里应该调用API添加依赖关系
    // 为了演示，我们直接更新本地数据
    const newDependency = availableTasks.value.find(task => task.id === selectedDependency.value)
    if (newDependency) {
      dependentNodes.value.push(newDependency)
      ElMessage.success('前驱节点添加成功')
      selectedDependency.value = ''
    }
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
    // 这里应该调用API移除依赖关系
    // 为了演示，我们直接更新本地数据
    dependentNodes.value = dependentNodes.value.filter(dep => dep.id !== dependency.id)
    ElMessage.success('前驱节点移除成功')
  } catch (error) {
    ElMessage.error('移除前驱节点失败')
    console.error('移除前驱节点失败:', error)
  } finally {
    updating.value = false
  }
}

// 当任务变化时，更新前驱节点数据
watch(() => props.task, (newTask) => {
  if (newTask) {
    dependentNodes.value = newTask.dependentNodes || []
    
    // 检查是否有权限更新前驱节点（这里可以根据实际权限逻辑调整）
    const currentUserId = localStorage.getItem('userId')
    canUpdateDependencies.value = newTask.createUserId === parseInt(currentUserId) || 
                                 localStorage.getItem('userRole') === 'ProjectAdmin'
  } else {
    dependentNodes.value = []
    canUpdateDependencies.value = false
  }
}, { immediate: true })
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