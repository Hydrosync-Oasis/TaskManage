<template>
  <el-card class="dag-card">
    <div class="dag-header-left">
      <div class="button-group">
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
    </div>
    
    <div class="dag-footer">
      <el-button
        type="info"
        size="small"
        @click="toggleAIChat"
      >
        <el-icon><ChatDotRound /></el-icon>
        {{ showAIChat ? '隐藏AI助手' : '显示AI助手' }}
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
    
    <div class="topo-order-container" v-if="topoGroups.length > 0">
      <div class="topo-order">
        <h3 class="topo-title">拓扑排序顺序</h3>
        <div class="topo-groups">
          <div 
            v-for="(group, groupIndex) in topoGroups" 
            :key="groupIndex" 
            class="topo-group"
            :class="{'group-even': groupIndex % 2 === 0, 'group-odd': groupIndex % 2 === 1}"
          >
            <div class="group-label">组 {{ groupIndex + 1 }}:</div>
            <div class="group-tags">
              <el-tag 
                v-for="taskId in group" 
                :key="getTaskId(taskId)" 
                size="small" 
                class="topo-tag"
                :type="groupIndex % 2 === 0 ? '' : 'info'"
              >
                {{ getTaskTitle(taskId) }}
              </el-tag>
            </div>
          </div>
        </div>
      </div>
    </div>
    
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
import { ref, watchEffect, nextTick, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus, ChatDotRound } from '@element-plus/icons-vue'
import { createTask } from '@/api/task'
import request from '@/utils/axios'

export default {
  name: 'DAGCanvas',
  components: {
    VueFlow,
    Background,
    Controls,
    MiniMap,
    Plus,
    ChatDotRound,
  },
  props: {
    tasks: {
      type: Array,
      default: () => []
    },
    projectId: {
      type: [Number, String],
      default: null
    },
    showAIChat: {
      type: Boolean,
      default: true
    }
  },
  emits: ['node-click', 'task-added', 'toggle-ai-chat'],
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
    // 当前选中的节点ID
    const selectedNodeId = ref(null)

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

    // 拓扑排序结果 - 改为二维数组
    const topoGroups = ref([])
    const showAIChat = ref(true)
    
    // 获取任务标题的辅助函数
    const getTaskTitle = (taskId) => {
      // 如果taskId是对象且有title属性，说明后端返回的是整个TaskDto对象
      if (taskId && typeof taskId === 'object' && taskId.title !== undefined) {
        return taskId.title
      }
      // 否则taskId就是任务ID
      const task = props.tasks.find(t => t.id === taskId)
      return task ? task.title : `Task ${taskId}`
    }
    
    // 获取任务ID的辅助函数
    const getTaskId = (taskId) => {
      if (taskId && typeof taskId === 'object' && taskId.id !== undefined) {
        return taskId.id
      }
      return taskId
    }
    
    // 切换AI聊天框显示状态
    const toggleAIChat = () => {
      showAIChat.value = !showAIChat.value
      emit('toggle-ai-chat', showAIChat.value)
    }
    
    // 从API获取拓扑排序结果
    const fetchTopoOrder = async () => {
      if (!props.projectId) {
        topoGroups.value = []
        return
      }
      
      try {
        const response = await request.get(`/api/Project/${props.projectId}/Tasks/Topological`)
        if (response.data && Array.isArray(response.data)) {
          topoGroups.value = response.data
          console.log('拓扑排序结果(分组):', topoGroups.value)
        }
      } catch (error) {
        console.error('获取拓扑排序失败:', error)
        // 如果API调用失败，使用本地计算的拓扑排序作为备份
        calculateTopoOrderLocally()
      }
    }
    
    // 本地计算拓扑排序（作为备份）
    const calculateTopoOrderLocally = () => {
      if (!props.tasks || props.tasks.length === 0) {
        topoGroups.value = []
        return
      }
      
      // 创建邻接表表示图
      const graph = {}
      // 记录每个节点的入度
      const inDegree = {}
      
      // 初始化
      props.tasks.forEach(task => {
        graph[task.id] = []
        inDegree[task.id] = 0
      })
      
      // 构建图和计算入度
      props.tasks.forEach(task => {
        if (task.dependencyTaskIds && Array.isArray(task.dependencyTaskIds)) {
          task.dependencyTaskIds.forEach(depId => {
            // 添加边: depId -> task.id
            graph[depId].push(task.id)
            // 增加task的入度
            inDegree[task.id] = (inDegree[task.id] || 0) + 1
          })
        }
      })
      
      // 拓扑排序结果 - 二维数组，每个子数组表示可并行执行的任务
      const result = []
      
      // 当还有节点未处理时继续
      while (Object.keys(inDegree).length > 0) {
        // 找出所有入度为0的节点作为当前层
        const currentLayer = []
        Object.keys(inDegree).forEach(nodeId => {
          if (inDegree[nodeId] === 0) {
            currentLayer.push(parseInt(nodeId))
            // 从入度表中移除
            delete inDegree[nodeId]
          }
        })
        
        // 如果没有入度为0的节点但图中还有节点，说明有环
        if (currentLayer.length === 0 && Object.keys(inDegree).length > 0) {
          console.warn('图中存在环，无法完成拓扑排序')
          break
        }
        
        // 将当前层添加到结果中
        if (currentLayer.length > 0) {
          result.push(currentLayer)
        }
        
        // 减少所有相邻节点的入度
        currentLayer.forEach(node => {
          graph[node].forEach(neighbor => {
            inDegree[neighbor]--
          })
        })
      }
      
      topoGroups.value = result
      console.log('本地计算的拓扑排序结果:', result)
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

        // 如果是选中的节点，添加高亮样式
        if (String(task.id) === selectedNodeId.value) {
          style = { 
            ...style, 
            boxShadow: '0 0 0 3px #409eff, 0 4px 12px rgba(0, 0, 0, 0.15)',
            zIndex: 1000
          }
          className += ' selected-node'
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

    // 当tasks属性变化时，获取拓扑排序
    watchEffect(() => {
      // 显式依赖 props.tasks 和每个task的 dependencyTaskIds 以确保变化时触发更新
      if (props.tasks) {
        // 强制监听每个任务的dependencyTaskIds
        const dependencyCount = props.tasks.reduce((count, task) => {
          return count + (task.dependencyTaskIds?.length || 0)
        }, 0)
        
        // 输出调试信息
        console.log(`任务总数: ${props.tasks.length}, 依赖关系总数: ${dependencyCount}`)
        
        // 从API获取拓扑排序
        fetchTopoOrder()
        
        if (props.tasks.length > 0) {
          // 先清空现有元素，确保重新生成
          elements.value = []
          
          // 有任务数据时，使用动态生成的节点和边
          const nodes = generateNodes()
          const edges = generateEdges()
          
          // 使用nextTick确保DOM更新
          nextTick(() => {
            elements.value = [...nodes, ...edges]
            
            // 再次使用nextTick确保元素已渲染后再调整视图
            nextTick(() => {
              try {
                const { fitView } = useVueFlow()
                fitView({ padding: 0.2, includeHiddenNodes: false })
              } catch (err) {
                console.error('调整视图失败:', err)
              }
            })
          })
        }
      } else if (props.projectId) {
        // 有项目ID但没有任务数据时，清空示例数据
        elements.value = []
        topoGroups.value = []
      } else {
        // 没有项目ID时，使用示例数据
        elements.value = [...sampleElements]
        topoGroups.value = []
      }
    })

    // 处理节点点击事件
    const { onNodeClick } = useVueFlow(); 

    onNodeClick(({ node }) => {
      // 找到对应的任务数据
      const taskData = props.tasks.find(task => String(task.id) === node.id)
      
      if (taskData) {
        // 设置选中的节点ID
        selectedNodeId.value = node.id
        
        // 只更新样式，不重新生成节点
        const currentNode = elements.value.find(el => el.id === node.id)
        if (currentNode) {
          currentNode.style = {
            ...currentNode.style,
            boxShadow: '0 0 0 3px #409eff, 0 4px 12px rgba(0, 0, 0, 0.15)'
          }
          currentNode.className = (currentNode.className || '') + ' selected-node'
        }
        
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
      defaultEdgeOptions,
      topoGroups,
      getTaskTitle,
      getTaskId,
      toggleAIChat,
      showAIChat
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

.dag-header-left {
  position: absolute;
  top: 10px;
  left: 10px;
  z-index: 10;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 10px;
}

.dag-footer {
  position: absolute;
  bottom: 10px;
  right: 10px;
  z-index: 10;
}

.button-group {
  display: flex;
  gap: 8px;
}

.topo-order-container {
  position: absolute;
  top: 60px;
  left: 10px;
  z-index: 10;
  max-width: 300px;
  max-height: 400px;
  overflow-y: auto;
}

.topo-order {
  background-color: rgba(255, 255, 255, 0.95);
  padding: 12px;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  border: 1px solid #e0e6ed;
}

.topo-title {
  margin-top: 0;
  margin-bottom: 10px;
  font-size: 16px;
  color: #303133;
  text-align: center;
  border-bottom: 1px solid #ebeef5;
  padding-bottom: 8px;
}

.topo-groups {
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-height: 320px;
  overflow-y: auto;
}

.topo-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
  padding: 8px;
  border-radius: 6px;
}

.group-label {
  font-weight: bold;
  font-size: 14px;
  color: #606266;
}

.group-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
}

.group-even {
  background-color: rgba(236, 245, 255, 0.7);
}

.group-odd {
  background-color: rgba(246, 249, 252, 0.7);
}

.topo-tag {
  margin-right: 0;
  white-space: nowrap;
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

:deep(.selected-node) {
  box-shadow: 0 0 0 3px #409eff, 0 4px 12px rgba(0, 0, 0, 0.15) !important;
  z-index: 1000 !important;
}

@keyframes dashdraw {
  from {
    stroke-dashoffset: 10;
  }
}
</style> 