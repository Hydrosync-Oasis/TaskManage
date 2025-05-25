<template>
  <el-card class="dag-card">
    <div class="dag-header">
      <el-button 
        type="primary" 
        size="small" 
        @click="showAddTaskDialog"
        :disabled="!projectId"
      >
        <el-icon><Plus /></el-icon>
        添加任务
      </el-button>
    </div>
    <VueFlow v-model="elements" class="vue-flow-wrapper" @nodeClick="onNodeClick">
      <Background pattern-color="#aaa" gap="8" />
      <Controls />
      <MiniMap />
    </VueFlow>
    
    <!-- 添加任务对话框 -->
    <el-dialog
      v-model="addTaskDialogVisible"
      title="添加新任务"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="taskForm"
        :model="newTask"
        :rules="taskRules"
        label-width="100px"
      >
        <el-form-item label="任务标题" prop="title">
          <el-input v-model="newTask.title" placeholder="请输入任务标题" />
        </el-form-item>
        <el-form-item label="任务描述" prop="description">
          <el-input
            v-model="newTask.description"
            type="textarea"
            :rows="4"
            placeholder="请输入任务描述"
          />
        </el-form-item>
        <el-form-item label="截止日期" prop="deadline">
          <el-date-picker
            v-model="newTask.deadline"
            type="datetime"
            placeholder="选择截止日期"
            format="YYYY-MM-DD HH:mm"
            value-format="YYYY-MM-DD HH:mm:ss"
          />
        </el-form-item>
        <el-form-item label="优先级" prop="priority">
          <el-select v-model="newTask.priority" placeholder="请选择优先级">
            <el-option label="低" :value="0" />
            <el-option label="中" :value="1" />
            <el-option label="高" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="前置任务" prop="dependentTaskIds">
          <el-select 
            v-model="newTask.dependentTaskIds" 
            multiple 
            placeholder="请选择前置任务（可多选）"
            :disabled="tasks.length === 0"
          >
            <el-option 
              v-for="task in tasks" 
              :key="task.id" 
              :label="task.title" 
              :value="task.id" 
            />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="addTaskDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleAddTask" :loading="submitting">
            创建
          </el-button>
        </span>
      </template>
    </el-dialog>
  </el-card>
</template>

<script>
import { VueFlow, Background, Controls, MiniMap } from '@vue-flow/core'
import '@vue-flow/core/dist/style.css'
import '@vue-flow/core/dist/theme-default.css'
import '@vue-flow/controls/dist/style.css'
import '@vue-flow/minimap/dist/style.css'
import { ref, watchEffect } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { createTask } from '@/api/task'

export default {
  name: 'DAGCanvas',
  components: {
    VueFlow,
    Background,
    Controls,
    MiniMap,
    Plus,
  },
  props: {
    tasks: {
      type: Array,
      default: () => []
    },
    projectId: {
      type: [Number, String],
      default: null
    }
  },
  emits: ['node-click', 'task-added'],
  setup(props, { emit }) {
    const elements = ref([
      // 示例节点和连线，当没有传入tasks时使用
      {
        id: '1',
        type: 'input',
        label: '开始节点',
        position: { x: 250, y: 0 },
      },
      {
        id: '2',
        label: '处理节点',
        position: { x: 250, y: 100 },
      },
      {
        id: '3',
        type: 'output',
        label: '结束节点',
        position: { x: 250, y: 200 },
      },
      // 示例连线
      {
        id: 'e1-2',
        source: '1',
        target: '2',
        animated: true,
        type: 'straight',
        markerEnd: 'arrowclosed'
      },
      {
        id: 'e2-3',
        source: '2',
        target: '3',
        animated: true,
        type: 'straight',
        markerEnd: 'arrowclosed'
      },
    ])

    // 添加任务相关变量
    const addTaskDialogVisible = ref(false)
    const submitting = ref(false)
    const taskForm = ref(null)
    const newTask = ref({
      title: '',
      description: '',
      deadline: '',
      priority: 1,
      dependentTaskIds: []
    })
    const taskRules = {
      title: [
        { required: true, message: '请输入任务标题', trigger: 'blur' },
        { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
      ],
      description: [
        { required: true, message: '请输入任务描述', trigger: 'blur' }
      ],
      priority: [
        { required: true, message: '请选择优先级', trigger: 'change' }
      ]
    }

    // 显示添加任务对话框
    const showAddTaskDialog = () => {
      if (!props.projectId) {
        ElMessage.warning('请先选择一个项目')
        return
      }
      
      // 重置表单
      newTask.value = {
        title: '',
        description: '',
        deadline: '',
        priority: 1,
        dependentTaskIds: []
      }
      
      addTaskDialogVisible.value = true
    }

    // 处理添加任务
    const handleAddTask = async () => {
      if (!taskForm.value) return
      
      await taskForm.value.validate(async (valid) => {
        if (valid) {
          try {
            submitting.value = true
            
            // 准备任务数据
            const taskData = {
              title: newTask.value.title,
              description: newTask.value.description,
              deadline: newTask.value.deadline,
              priority: newTask.value.priority,
              projectId: props.projectId,
              dependencyTaskIds: newTask.value.dependentTaskIds,
              status: 0
            }
            
            // 调用API创建任务
            await createTask(taskData)
            
            ElMessage.success('任务创建成功')
            addTaskDialogVisible.value = false
            
            // 通知父组件任务已添加，需要刷新任务列表
            emit('task-added')
          } catch (error) {
            console.error('创建任务失败:', error)
            ElMessage.error('创建任务失败，请稍后重试')
          } finally {
            submitting.value = false
          }
        }
      })
    }

    // 计算节点
    const generateNodes = () => {
      if (!props.tasks || props.tasks.length === 0) return []
      
      return props.tasks.map(task => ({
        id: String(task.id),
        label: task.title,
        // 可根据task.status设置不同的样式
        type: task.type || 'default',
        // 使用相对合理的位置布局
        position: { x: Math.random() * 400, y: Math.random() * 400 },
        data: task // 保存完整任务数据，方便后续使用
      }))
    }

    // 计算连线
    const generateEdges = () => {
      if (!props.tasks || props.tasks.length === 0) return []
      
      const result = []
      props.tasks.forEach(task => {
        if (task.dependentNodes && Array.isArray(task.dependentNodes)) {
          task.dependentNodes.forEach(dep => {
            result.push({
              id: `e${dep.id}-${task.id}`,
              source: String(dep.id),
              target: String(task.id),
              animated: true,
              type: 'straight',
              markerEnd: 'arrowclosed'
            })
          })
        }
      })
      return result
    }

    // 当tasks属性变化时，更新流程图
    watchEffect(() => {
      if (props.tasks && props.tasks.length > 0) {
        // 有任务数据时，使用动态生成的节点和边
        const nodes = generateNodes()
        const edges = generateEdges()
        elements.value = [...nodes, ...edges]
      }
      // 没有任务数据时保持默认示例数据
    })

    // 处理节点点击事件
    const onNodeClick = (event, node) => {
      // 如果是示例节点，不触发事件
      if (['1', '2', '3'].includes(node.id)) {
        return
      }
      
      // 找到对应的任务数据
      const taskData = props.tasks.find(task => String(task.id) === node.id)
      if (taskData) {
        // 向父组件发送点击事件，传递选中的任务数据
        emit('node-click', taskData)
      }
    }

    return {
      elements,
      onNodeClick,
      addTaskDialogVisible,
      newTask,
      taskRules,
      taskForm,
      submitting,
      showAddTaskDialog,
      handleAddTask
    }
  }
}
</script>

<style scoped>
.dag-card {
  width: 100%;
  height: 100%;
  position: relative;
}

.dag-header {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 10;
}

.vue-flow-wrapper {
  width: 100%;
  height: 600px;
  background-color: #fff;
}
</style> 