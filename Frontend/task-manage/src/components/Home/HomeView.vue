<template>
  <el-container class="app-container">
    <el-header class="app-header">
      <div class="header-content">
        <el-select v-model="selectedFile" placeholder="File: Task1" style="width: 180px" size="large">
          <el-option label="Task1" value="Task1" />
          <el-option label="Task2" value="Task2" />
        </el-select>
        <div class="center-title">
          <el-icon class="logo-icon"><Monitor /></el-icon>
          <h1 class="system-title">智能项目管理系统</h1>
        </div>
      </div>
    </el-header>
    <el-main class="app-main">
      <el-row :gutter="24" style="height: 100%">
        <el-col :span="5" class="left-panel">
          <el-collapse accordion>
            <el-collapse-item title="展示任务评论" name="0">
              <template #title>
                <div class="collapse-title">
                  <el-icon><ChatDotRound /></el-icon>
                  <span>展示任务评论</span>
                </div>
              </template>
              <TaskComment :taskId="taskId" />
            </el-collapse-item>
            <el-collapse-item title="制定日期，截止日期" name="1">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Calendar /></el-icon>
                  <span>制定日期，截止日期</span>
                </div>
              </template>
              <TaskDate />
            </el-collapse-item>
            <el-collapse-item title="优先级" name="2">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Star /></el-icon>
                  <span>优先级</span>
                </div>
              </template>
              <TaskPriority />
            </el-collapse-item>
            <el-collapse-item title="分配人员" name="3">
              <template #title>
                <div class="collapse-title">
                  <el-icon><User /></el-icon>
                  <span>分配人员</span>
                </div>
              </template>
              <TaskAssignee />
            </el-collapse-item>
            <el-collapse-item title="任务编号" name="4">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Document /></el-icon>
                  <span>任务编号</span>
                </div>
              </template>
              <TaskId />
            </el-collapse-item>
            <el-collapse-item title="前驱节点" name="5">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Connection /></el-icon>
                  <span>前驱节点</span>
                </div>
              </template>
              <TaskPredecessor />
            </el-collapse-item>
            <el-collapse-item title="任务状态" name="6">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Loading /></el-icon>
                  <span>任务状态</span>
                </div>
              </template>
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
import { Monitor, ChatDotRound, Calendar, Star, User, Document, Connection, Loading } from '@element-plus/icons-vue'
import DAGCanvas from './DAGCanvas.vue'
import AIChat from './AIChat.vue'
import TaskComment from './TaskComment.vue'
import TaskDate from './TaskDate.vue'
import TaskPriority from './TaskPriority.vue'
import TaskAssignee from './TaskAssignee.vue'
import TaskId from './TaskId.vue'
import TaskPredecessor from './TaskPredecessor.vue'
import TaskStatus from './TaskStatus.vue'

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
    TaskStatus,
    Monitor,
    ChatDotRound,
    Calendar,
    Star,
    User,
    Document,
    Connection,
    Loading
  },
  setup() {
    const selectedFile = ref('Task1')
    const taskId = ref('') // Assuming taskId is stored in a ref
    return {
      selectedFile,
      taskId
    }
  }
}
</script>

<style>
:root {
  --primary-gradient: linear-gradient(135deg, #43cea2 0%, #185a9d 100%);
  --accent-color: #ff9800;
  --text-primary: #2c3e50;
  --text-secondary: #546e7a;
  --bg-light: #eceff4;
  --border-color: #e0e6ed;
}

body, html {
  background: var(--bg-light);
  margin: 0;
  padding: 0;
  height: 100vh;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
}

.app-container {
  height: 100vh;
}

.app-header {
  background: var(--primary-gradient);
  padding: 0 32px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}

.header-content {
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: relative;
}

.center-title {
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  align-items: center;
  gap: 12px;
}

.logo-icon {
  font-size: 24px;
  color: white;
}

.system-title {
  color: white;
  font-size: 2.2rem;
  font-weight: 700;
  margin: 0;
  max-width: 60vw;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.app-main {
  padding: 24px;
  height: calc(100vh - 64px);
  background: var(--bg-light);
}

.left-panel {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
  padding: 16px;
}

.collapse-title {
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--text-primary);
}

.collapse-title .el-icon {
  color: var(--accent-color);
}

.center-panel {
  display: flex;
  align-items: center;
  justify-content: center;
}

.dag-placeholder {
  width: 100%;
  height: 100%;
  min-height: 600px;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
  background: white;
  border: 1px solid var(--border-color);
}

.right-panel {
  height: 100%;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
  background: white;
  border: 1px solid var(--border-color);
}

/* 响应式设计 */
@media (max-width: 1200px) {
  .el-col {
    width: 100%;
  }
  
  .left-panel, .center-panel, .right-panel {
    margin-bottom: 24px;
  }
}

/* Element Plus 组件样式覆盖 */
.el-collapse-item__header {
  transition: background 0.3s, color 0.3s, box-shadow 0.3s;
  border-radius: 8px;
  font-weight: 500;
  color: var(--text-primary);
}

.el-collapse-item__header:hover {
  background: #e3f2fd;
  color: #1976d2;
  box-shadow: 0 2px 8px 0 rgba(100,181,246,0.08);
  transform: translateY(-2px) scale(1.03);
}

.el-collapse-item__content {
  color: var(--text-secondary);
}

.el-button--primary {
  background: var(--primary-gradient);
  border: none;
}

.el-button--primary:hover {
  background: linear-gradient(135deg, #1976d2 0%, #0d47a1 100%);
}

.el-select .el-input__wrapper {
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
}

.el-select .el-input__wrapper:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}
</style>
