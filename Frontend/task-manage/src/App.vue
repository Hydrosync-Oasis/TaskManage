<template>
  <el-container class="app-container">
    <el-header class="app-header">
      <el-row align="middle" style="width: 100%">
        <el-col :span="8" style="display: flex; align-items: center; gap: 16px;">
          <el-select v-model="selectedFile" placeholder="File: Task1" style="width: 180px" size="large">
            <el-option label="Task1" value="Task1" />
            <el-option label="Task2" value="Task2" />
          </el-select>
        </el-col>
      </el-row>
    </el-header>
    <el-main class="app-main">
      <el-row :gutter="24" style="height: 100%">
        <el-col :span="5" class="left-panel">
          <el-collapse accordion>
            <el-collapse-item title="展示任务评论" name="0">
              <TaskComment />
            </el-collapse-item>
            <el-collapse-item title="制定日期，截止日期" name="1">
              <TaskDate />
            </el-collapse-item>
            <el-collapse-item title="优先级" name="2">
              <TaskPriority />
            </el-collapse-item>
            <el-collapse-item title="分配人员" name="3">
              <TaskAssignee />
            </el-collapse-item>
            <el-collapse-item title="任务编号" name="4">
              <TaskId />
            </el-collapse-item>
            <el-collapse-item title="前驱节点" name="5">
              <TaskPredecessor />
            </el-collapse-item>
            <el-collapse-item title="任务状态" name="6">
              <TaskStatus />
            </el-collapse-item>
          </el-collapse>
        </el-col>
        <el-col :span="13" class="center-panel">
          <div class="dag-placeholder light-card">
            <DAGCanvas />
          </div>
        </el-col>
        <el-col :span="6" class="right-panel light-card">
          <AIChat />
        </el-col>
      </el-row>
    </el-main>
  </el-container>
</template>

<script>
import { ref } from 'vue'
import DAGCanvas from './components/DAGCanvas.vue'
import AIChat from './components/AIChat.vue'
import TaskComment from './components/TaskComment.vue'
import TaskDate from './components/TaskDate.vue'
import TaskPriority from './components/TaskPriority.vue'
import TaskAssignee from './components/TaskAssignee.vue'
import TaskId from './components/TaskId.vue'
import TaskPredecessor from './components/TaskPredecessor.vue'
import TaskStatus from './components/TaskStatus.vue'

export default {
  name: 'App',
  components: {
    DAGCanvas,
    AIChat,
    TaskComment,
    TaskDate,
    TaskPriority,
    TaskAssignee,
    TaskId,
    TaskPredecessor,
    TaskStatus
  },
  setup() {
    const selectedFile = ref('Task1')
    const taskBtns = [
      '展示任务评论',
      '制定日期，截止日期',
      '优先级',
      '分配人员',
      '任务编号',
      '前驱节点',
      '任务状态'
    ]
    return {
      selectedFile,
      taskBtns
    }
  }
}
</script>

<style>
body, html {
  background: #f5f7fa;
}
::-webkit-scrollbar { display: none; }
html, body { margin: 0; padding: 0; height: 100vh; overflow: hidden; }
#app { height: 100vh; font-family: Avenir, Helvetica, Arial, sans-serif; background: #f5f7fa; color: #333; }
.app-container { height: 100vh; }
.app-header {
  background: #fff;
  border-bottom: 1px solid #e4e7ed;
  display: flex;
  align-items: center;
  height: 64px;
  padding: 0 32px;
  box-shadow: 0 2px 8px 0 rgba(0,0,0,0.03);
  border-radius: 0 0 12px 12px;
}
.app-main {
  padding: 24px 32px 32px 32px;
  height: calc(100vh - 64px);
  background: #f5f7fa;
  overflow: hidden;
}
.left-panel {
  background: #fff;
  border-radius: 16px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.06);
  height: 100%;
  padding: 32px 0 32px 0;
  display: flex;
  align-items: flex-start;
  justify-content: center;
}
.center-panel {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
}
.right-panel {
  height: 100%;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  padding: 0;
}
.task-btn-group {
  display: flex;
  flex-direction: column;
  gap: 20px;
  width: 90%;
}
.task-btn {
  width: 100%;
  margin: 0 auto;
  text-align: left;
  border-radius: 24px !important;
  font-size: 16px;
  font-weight: 500;
  background: #f5f7fa;
  color: #409eff;
  border: 1px solid #d9ecff;
  transition: box-shadow 0.2s, background 0.2s;
}
.task-btn:hover {
  background: #e8f4ff;
  box-shadow: 0 2px 8px 0 rgba(64,158,255,0.08);
}
.dag-placeholder {
  width: 100%;
  height: 500px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 16px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.06);
  border: 1px solid #e4e7ed;
  background: #fff;
}
.light-card {
  background: #fff !important;
  border-radius: 16px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.06);
  border: 1px solid #e4e7ed;
}
.el-radio-button { margin-right: 10px }
.el-col { height: 100% }
</style>
