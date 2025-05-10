<template>
  <el-card class="dag-card">
    <VueFlow v-model="elements" class="vue-flow-wrapper">
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

export default {
  name: 'DAGCanvas',
  components: {
    VueFlow,
    Background,
    Controls,
    MiniMap,
  },
  data() {
    return {
      elements: [
        // 示例节点
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
        },
        {
          id: 'e2-3',
          source: '2',
          target: '3',
          animated: true,
        },
      ],
    }
  },
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