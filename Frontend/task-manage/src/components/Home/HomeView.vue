<template>
  <el-container class="app-container">
    <el-header class="app-header">
      <div class="header-content">
        <div class="header-left">
          <el-select v-model="selectedProject" placeholder="请选择项目" style="width: 180px" size="large" @change="handleProjectChange">
            <el-option
              v-for="project in projects"
              :key="project.id"
              :label="project.name"
              :value="project.id"
            />
          </el-select>
          <el-button 
            type="primary" 
            size="large" 
            @click="showCreateProjectDialog"
            style="margin-left: 10px"
          >
            新建项目
          </el-button>
        </div>
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
              <TaskComment :taskId="selectedTask?.id" />
            </el-collapse-item>
            <el-collapse-item title="制定日期，截止日期" name="1">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Calendar /></el-icon>
                  <span>制定日期，截止日期</span>
                </div>
              </template>
              <TaskDate :task="selectedTask" />
            </el-collapse-item>
            <el-collapse-item title="优先级" name="2">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Star /></el-icon>
                  <span>优先级</span>
                </div>
              </template>
              <TaskPriority :task="selectedTask" />
            </el-collapse-item>
            <el-collapse-item title="分配人员" name="3">
              <template #title>
                <div class="collapse-title">
                  <el-icon><User /></el-icon>
                  <span>分配人员</span>
                </div>
              </template>
              <TaskAssignee :task="selectedTask" />
            </el-collapse-item>
            <el-collapse-item title="任务编号" name="4">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Document /></el-icon>
                  <span>任务编号</span>
                </div>
              </template>
              <TaskId :task="selectedTask" />
            </el-collapse-item>
            <el-collapse-item title="前驱节点" name="5">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Connection /></el-icon>
                  <span>前驱节点</span>
                </div>
              </template>
              <TaskPredecessor :task="selectedTask" />
            </el-collapse-item>
            <el-collapse-item title="任务状态" name="6">
              <template #title>
                <div class="collapse-title">
                  <el-icon><Loading /></el-icon>
                  <span>任务状态</span>
                </div>
              </template>
              <TaskStatus :task="selectedTask" />
            </el-collapse-item>
          </el-collapse>
        </el-col>
        <el-col :span="13" class="center-panel">
          <div class="project-info light-card" style="background: #f0f2f5; padding: 20px; margin-bottom: 20px; border-radius: 8px;">
            <div class="info-item">
              <span class="label">项目描述：</span>
              <span class="value">{{ currentProject?.description || '暂无描述' }}</span>
            </div>
            <div class="info-item">
              <span class="label">创建时间：</span>
              <span class="value">{{ formatDate(currentProject?.createdAt) }}</span>
            </div>
            <div class="info-item">
              <span class="label">项目所有者：</span>
              <span class="value">{{ currentProject?.ownerUid || '未知' }}</span>
            </div>
            <div class="project-actions">
              <el-button 
                type="primary" 
                size="small" 
                :disabled="!selectedProject"
                @click="showUpdateProjectDialog"
              >
                编辑项目
              </el-button>
              <el-button 
                type="danger" 
                size="small" 
                :disabled="!selectedProject"
                @click="handleDeleteProject"
              >
                删除项目
              </el-button>
            </div>
          </div>
          <div class="dag-placeholder light-card">
            <DAGCanvas 
              :tasks="projectTasks" 
              :projectId="selectedProject" 
              @node-click="handleTaskNodeClick" 
              @task-added="handleTaskAdded"
            />
          </div>
        </el-col>
        <el-col :span="6" class="right-panel light-card">
          <AIChat />
        </el-col>
      </el-row>
    </el-main>
  </el-container>

  <!-- 新建项目对话框 -->
  <el-dialog
    v-model="createProjectDialogVisible"
    title="新建项目"
    width="500px"
    :close-on-click-modal="false"
  >
    <el-form
      ref="createProjectForm"
      :model="newProject"
      :rules="projectRules"
      label-width="100px"
    >
      <el-form-item label="项目名称" prop="name">
        <el-input v-model="newProject.name" placeholder="请输入项目名称" />
      </el-form-item>
      <el-form-item label="项目描述" prop="description">
        <el-input
          v-model="newProject.description"
          type="textarea"
          :rows="4"
          placeholder="请输入项目描述"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="createProjectDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleCreateProject" :loading="creating">
          创建
        </el-button>
      </span>
    </template>
  </el-dialog>

  <!-- 更新项目对话框 -->
  <el-dialog
    v-model="updateProjectDialogVisible"
    title="编辑项目"
    width="500px"
    :close-on-click-modal="false"
  >
    <el-form
      ref="updateProjectForm"
      :model="editingProject"
      :rules="projectRules"
      label-width="100px"
    >
      <el-form-item label="项目名称" prop="name">
        <el-input v-model="editingProject.name" placeholder="请输入项目名称" />
      </el-form-item>
      <el-form-item label="项目描述" prop="description">
        <el-input
          v-model="editingProject.description"
          type="textarea"
          :rows="4"
          placeholder="请输入项目描述"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="updateProjectDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleUpdateProject" :loading="updating">
          保存
        </el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script>
import { ref, onMounted } from 'vue'
import { Monitor, ChatDotRound, Calendar, Star, User, Document, Connection, Loading } from '@element-plus/icons-vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getAllProjects, deleteProject, getProjectById, createProject, updateProject, getProjectTasks } from '@/api/project'
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
    const taskId = ref('')
    const projects = ref([])
    const selectedProject = ref('')
    const currentProject = ref(null)
    const selectedTask = ref(null)
    const createProjectDialogVisible = ref(false)
    const createProjectForm = ref(null)
    const creating = ref(false)
    const newProject = ref({
      name: '',
      description: ''
    })
    const updateProjectDialogVisible = ref(false)
    const updateProjectForm = ref(null)
    const updating = ref(false)
    const editingProject = ref({
      id: null,
      name: '',
      description: ''
    })
    const projectTasks = ref([])

    // 获取项目列表
    const fetchProjects = async () => {
      try {
        const response = await getAllProjects()
        projects.value = response.data
      } catch (error) {
        ElMessage.error('获取项目列表失败')
        console.error('获取项目列表失败:', error)
        // 添加示例数据
        projects.value = [
          {
            id: 1,
            name: '示例项目',
            description: '这是一个示例项目，用于展示项目详情。该项目包含了多个任务节点，展示了任务之间的依赖关系。',
            createdAt: new Date().toISOString(),
            ownerUid: '1001'
          }
        ]
      }
    }

    // 获取项目详情
    const fetchProjectDetail = async (projectId) => {
      try {
        const response = await getProjectById(projectId)
        currentProject.value = response.data
      } catch (error) {
        ElMessage.error('获取项目详情失败')
        // 添加示例数据
        currentProject.value = {
          id: projectId,
          name: '示例项目',
          description: '这是一个示例项目，用于展示项目详情。该项目包含了多个任务节点，展示了任务之间的依赖关系。',
          createdAt: new Date().toISOString(),
          ownerUid: '1001'
        }
      }
    }

    // 处理项目选择变化
    const handleProjectChange = async (projectId) => {
      if (projectId) {
        selectedProject.value = projectId
        await fetchProjectDetail(projectId)
        await fetchProjectTasks(projectId)
      } else {
        selectedProject.value = ''
        currentProject.value = null
        projectTasks.value = []
      }
    }

    // 获取项目任务
    const fetchProjectTasks = async (projectId) => {
      try {
        const res = await getProjectTasks(projectId)
        projectTasks.value = res.data
      } catch (e) {
        console.error('获取项目任务失败:', e)
        projectTasks.value = []
      }
    }

    // 处理任务添加事件
    const handleTaskAdded = async () => {
      if (selectedProject.value) {
        await fetchProjectTasks(selectedProject.value)
        ElMessage.success('任务列表已更新')
      }
    }

    // 处理删除项目
    const handleDeleteProject = async () => {
      try {
        await ElMessageBox.confirm(
          '确定要删除这个项目吗？此操作不可恢复。',
          '警告',
          {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        
        await deleteProject(selectedProject.value)
        ElMessage.success('项目删除成功')
        selectedProject.value = ''
        await fetchProjects() // 重新加载项目列表
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('删除项目失败')
          console.error('删除项目失败:', error)
        }
      }
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return '未知'
      return new Date(date).toLocaleString()
    }

    // 表单验证规则
    const projectRules = {
      name: [
        { required: true, message: '请输入项目名称', trigger: 'blur' },
        { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
      ],
      description: [
        { required: true, message: '请输入项目描述', trigger: 'blur' },
        { min: 5, max: 500, message: '长度在 5 到 500 个字符', trigger: 'blur' }
      ]
    }

    // 显示创建项目对话框
    const showCreateProjectDialog = () => {
      createProjectDialogVisible.value = true
      newProject.value = {
        name: '',
        description: ''
      }
    }

    // 处理创建项目
    const handleCreateProject = async () => {
      if (!createProjectForm.value) return
      
      try {
        await createProjectForm.value.validate()
        creating.value = true
        
        await createProject(newProject.value)
        ElMessage.success('项目创建成功')
        createProjectDialogVisible.value = false
        await fetchProjects() // 刷新项目列表
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error(error.message || '创建项目失败')
          console.error('创建项目失败:', error)
        }
      } finally {
        creating.value = false
      }
    }

    // 显示更新项目对话框
    const showUpdateProjectDialog = () => {
      if (!currentProject.value) return
      
      editingProject.value = {
        id: currentProject.value.id,
        name: currentProject.value.name,
        description: currentProject.value.description
      }
      updateProjectDialogVisible.value = true
    }

    // 处理更新项目
    const handleUpdateProject = async () => {
      if (!updateProjectForm.value) return
      
      try {
        await updateProjectForm.value.validate()
        updating.value = true
        
        await updateProject(editingProject.value.id, editingProject.value)
        ElMessage.success('项目更新成功')
        updateProjectDialogVisible.value = false
        await fetchProjectDetail(editingProject.value.id) // 刷新项目详情
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error(error.message || '更新项目失败')
          console.error('更新项目失败:', error)
        }
      } finally {
        updating.value = false
      }
    }

    // 处理任务节点点击事件
    const handleTaskNodeClick = (task) => {
      selectedTask.value = task
      taskId.value = task.id
      ElMessage.success(`已选择任务: ${task.title}`)
    }

    // 组件挂载时获取项目列表
    onMounted(() => {
      fetchProjects()
    })

    return {
      taskId,
      projects,
      selectedProject,
      currentProject,
      selectedTask,
      handleProjectChange,
      handleDeleteProject,
      handleTaskNodeClick,
      formatDate,
      createProjectDialogVisible,
      createProjectForm,
      creating,
      newProject,
      projectRules,
      showCreateProjectDialog,
      handleCreateProject,
      updateProjectDialogVisible,
      updateProjectForm,
      updating,
      editingProject,
      showUpdateProjectDialog,
      handleUpdateProject,
      projectTasks,
      handleTaskAdded,
      fetchProjectTasks
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

.header-left {
  position: absolute;
  left: -2px;
  top: 50%;
  transform: translateY(-50%);
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
  margin-top: -2px;
  margin-bottom: 20px;
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
  flex-direction: column;
  gap: 20px;
  height: 100%;
}

.project-info {
  margin-bottom: 12px;
  padding: 10px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
}

.info-item {
  margin-bottom: 4px;
}

.info-item:last-child {
  margin-bottom: 0;
}

.label {
  font-weight: bold;
  color: #606266;
  margin-right: 10px;
}

.value {
  color: #303133;
}

.dag-placeholder {
  flex: 1;
  height: calc(100% - 170px);
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
  background: white;
  border: 1px solid var(--border-color);
  overflow: hidden;
}

.right-panel {
  height: 100%;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.05);
  background: white;
  border: 1px solid var(--border-color);
  padding: 20px;
  margin-top: -2px;
  margin-bottom: 20px;
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

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.project-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 12px;
  padding-top: 10px;
  border-top: 1px solid #eee;
}
</style>
