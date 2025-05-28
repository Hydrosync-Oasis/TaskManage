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
    <VueFlow 
      v-model="elements" 
      class="vue-flow-wrapper"
      :default-edge-options="defaultEdgeOptions"
    >
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
import { VueFlow, Background, Controls, MiniMap, useVueFlow } from '@vue-flow/core'
import '@vue-flow/core/dist/style.css'
import '@vue-flow/core/dist/theme-default.css'
import '@vue-flow/controls/dist/style.css'
import '@vue-flow/minimap/dist/style.css'
import { ref, watchEffect, nextTick } from 'vue'
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
    // 连线默认配置
    const defaultEdgeOptions = {
      animated: true,
      style: { strokeWidth: 2, stroke: '#3e9eff' },
      markerEnd: {
        type: 'arrowclosed',
        width: 20,
        height: 20,
        color: '#3e9eff',
      },
    }
    
    // 示例元素数据
    const sampleElements = [
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
    ];
    
    // 初始化为空数组
    const elements = ref([])

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
              deadline: new Date(newTask.value.deadline.replace(" ", "T")).toISOString(),
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
      
      // 计算每个节点的层级
      const nodeDepths = {}
      const nodeMap = {}
      
      // 创建节点映射
      props.tasks.forEach(task => {
        nodeMap[task.id] = task
      })
      
      // 计算节点深度的函数
      const calculateDepth = (taskId, visited = new Set()) => {
        // 防止循环依赖
        if (visited.has(taskId)) return 0
        visited.add(taskId)
        
        const task = nodeMap[taskId]
        if (!task || !task.dependencyTaskIds || task.dependencyTaskIds.length === 0) {
          return 0 // 没有依赖的节点在最顶层
        }
        
        // 计算所有依赖节点的最大深度
        let maxDepth = 0
        task.dependencyTaskIds.forEach(depId => {
          const depDepth = calculateDepth(depId, new Set(visited))
          maxDepth = Math.max(maxDepth, depDepth)
        })
        
        // 当前节点的深度是其依赖节点的最大深度 + 1
        return maxDepth + 1
      }
      
      // 为每个节点计算深度
      props.tasks.forEach(task => {
        nodeDepths[task.id] = calculateDepth(task.id)
      })
      
      // 按层级对节点进行分组
      const levelGroups = {}
      Object.keys(nodeDepths).forEach(taskId => {
        const depth = nodeDepths[taskId]
        if (!levelGroups[depth]) {
          levelGroups[depth] = []
        }
        levelGroups[depth].push(Number(taskId))
      })
      
      // 计算每个层级的节点数量，用于水平布局
      const levelCounts = {}
      Object.keys(levelGroups).forEach(level => {
        levelCounts[level] = levelGroups[level].length
      })
      
      // 生成节点位置
      return props.tasks.map(task => {
        const depth = nodeDepths[task.id]
        const nodesInLevel = levelCounts[depth]
        const indexInLevel = levelGroups[depth].indexOf(task.id)
        
        // 计算水平位置：均匀分布在画布宽度上
        const xStep = 600 / (nodesInLevel + 1)
        const x = (indexInLevel + 1) * xStep
        
        // 计算垂直位置：根据层级决定
        const y = depth * 120 + 50
        
        // 根据任务状态设置不同样式
        let style = {}
        let className = ''
        
        // 根据任务优先级设置不同的边框颜色
        if (task.priority === 2) { // 高优先级
          style = { border: '2px solid #f56c6c', fontWeight: 'bold' }
          className = 'high-priority'
        } else if (task.priority === 1) { // 中优先级
          style = { border: '2px solid #e6a23c' }
          className = 'medium-priority'
        }
        
        // 根据任务状态设置不同的背景色
        if (task.status === 2) { // 已完成
          style = { ...style, backgroundColor: '#f0f9eb', color: '#67c23a' }
          className += ' completed'
        } else if (task.status === 1) { // 进行中
          style = { ...style, backgroundColor: '#ecf5ff', color: '#409eff' }
          className += ' in-progress'
        }
        
        return {
          id: String(task.id),
          label: task.title,
          type: task.type || 'default',
          position: { x, y },
          data: task,
          style,
          className
        }
      })
    }

    // 计算连线
    const generateEdges = () => {
      if (!props.tasks || props.tasks.length === 0) return []
      
      const result = []
      props.tasks.forEach(task => {
        if (task.dependencyTaskIds && Array.isArray(task.dependencyTaskIds)) {
          task.dependencyTaskIds.forEach(depId => {
            result.push({
              id: `e${depId}-${task.id}`,
              source: String(depId),
              target: String(task.id),
              animated: true,
              type: 'smoothstep',
              style: { strokeWidth: 2, stroke: '#3e9eff' },
              markerEnd: {
                type: 'arrowclosed',
                width: 20,
                height: 20,
                color: '#3e9eff',
              }
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

        // 调整视图以适应所有节点
        nextTick(() => {
          const { fitView } = useVueFlow()
          fitView({ padding: 0.2, includeHiddenNodes: false })
        })
      } else if (props.projectId) {
        // 有项目ID但没有任务数据时，清空示例数据
        elements.value = []
      } else {
        // 没有项目ID时，使用示例数据
        elements.value = [...sampleElements]
      }
    })

    // 处理节点点击事件
    const { onNodeClick } = useVueFlow(); 

    onNodeClick(({ node }) => {
      // 找到对应的任务数据
      const taskData = props.tasks.find(task => String(task.id) === node.id)
      
      if (taskData) {
        // 向父组件发送点击事件，传递选中的任务数据
        emit('node-click', taskData)
      }
    });

    return {
      elements,
      onNodeClick,
      addTaskDialogVisible,
      newTask,
      taskRules,
      taskForm,
      submitting,
      showAddTaskDialog,
      handleAddTask,
      defaultEdgeOptions
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

:deep(.vue-flow__node) {
  padding: 10px 15px;
  border-radius: 5px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  min-width: 150px;
  text-align: center;
  font-weight: 500;
  border: 1px solid #dcdfe6;
  background-color: white;
  transition: all 0.3s;
}

:deep(.vue-flow__node:hover) {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}

:deep(.vue-flow__edge-path) {
  stroke-width: 2;
}

:deep(.vue-flow__edge.animated .vue-flow__edge-path) {
  stroke-dasharray: 5;
  animation: dashdraw 0.5s linear infinite;
}

:deep(.high-priority) {
  border-width: 2px;
  border-color: #f56c6c;
}

:deep(.medium-priority) {
  border-width: 2px;
  border-color: #e6a23c;
}

:deep(.completed) {
  background-color: #f0f9eb;
  color: #67c23a;
}

:deep(.in-progress) {
  background-color: #ecf5ff;
  color: #409eff;
}

@keyframes dashdraw {
  from {
    stroke-dashoffset: 10;
  }
}
</style> 