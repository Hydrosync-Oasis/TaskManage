<template>
  <el-card class="dag-card">
    <VueFlow v-model="elements" class="vue-flow-wrapper" @nodeClick="onNodeClick">
      <Background pattern-color="#aaa" gap="8" />
      <Controls />
      <MiniMap />
    </VueFlow>
  </el-card>
</template>

<script>
import { VueFlow, Background, Controls, MiniMap } from '@vue-flow/core'
import '@vue-flow/core/dist/style.css'
import '@vue-flow/core/dist/theme-default.css'
import '@vue-flow/controls/dist/style.css'
import '@vue-flow/minimap/dist/style.css'
import { ref, watchEffect } from 'vue'

export default {
  name: 'DAGCanvas',
  components: {
    VueFlow,
    Background,
    Controls,
    MiniMap,
  },
  props: {
    tasks: {
      type: Array,
      default: () => []
    }
  },
  emits: ['node-click'],
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
      onNodeClick
    }
  }
}
</script>

<style scoped>
.dag-card {
  width: 100%;
  height: 100%;
}

.vue-flow-wrapper {
  width: 100%;
  height: 600px;
  background-color: #fff;
}
</style> 