import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import { ElRadioButton, ElRadioGroup } from 'element-plus'
import DAGCanvas from './components/DAGCanvas.vue'
import AIChat from './components/AIChat.vue'
import TaskPanel from './components/TaskPanel.vue'

const app = createApp(App)

// 注册 Element Plus 组件
app.use(ElementPlus)
app.component('ElRadioButton', ElRadioButton)
app.component('ElRadioGroup', ElRadioGroup)

// 注册自定义组件
app.component('DAGCanvas', DAGCanvas)
app.component('AIChat', AIChat)
app.component('TaskPanel', TaskPanel)

app.mount('#app')
