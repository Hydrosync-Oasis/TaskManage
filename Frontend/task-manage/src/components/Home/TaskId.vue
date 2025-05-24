<template>
  <div class="task-id-container">
    <div v-if="task" class="task-info">
      <div class="info-item">
        <span class="label">任务ID：</span>
        <span class="value">{{ task.id }}</span>
      </div>
      <div class="info-item">
        <span class="label">任务标题：</span>
        <span class="value">{{ task.title }}</span>
      </div>
      <div class="info-item">
        <span class="label">所属项目：</span>
        <span class="value">{{ task.projectId }}</span>
      </div>
    </div>
    <div v-else class="no-task">
      请从图表中选择一个任务节点
    </div>
  </div>
</template>

<script setup>
/* eslint-disable no-undef */
defineProps({
  task: {
    type: Object,
    default: null
  }
})
/* eslint-enable no-undef */
</script>

<style scoped>
.task-id-container {
  padding: 15px;
}

.task-info {
  background-color: #f5f7fa;
  border-radius: 8px;
  padding: 15px;
  border: 1px solid #e4e7ed;
}

.info-item {
  margin-bottom: 10px;
}

.info-item:last-child {
  margin-bottom: 0;
}

.label {
  font-weight: bold;
  color: #606266;
  margin-right: 5px;
}

.value {
  color: #303133;
}

.no-task {
  color: #909399;
  text-align: center;
  padding: 20px 0;
  font-style: italic;
}
</style> 