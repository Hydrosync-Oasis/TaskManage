<template>
  <div v-if="error" class="error-container">
    <el-alert
      :title="error.title"
      :description="error.message"
      :type="error.type"
      show-icon
      :closable="true"
      @close="clearError"
    />
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { ElAlert } from 'element-plus'

/* eslint-disable no-undef */
const props = defineProps({
  errorObject: {
    type: Object,
    default: null
  }
})
/* eslint-enable no-undef */

const error = ref(null)

// 监听错误对象变化
watch(() => props.errorObject, (newError) => {
  if (newError) {
    error.value = {
      title: newError.title || '错误',
      message: newError.message || '发生未知错误',
      type: newError.type || 'error'
    }
  } else {
    error.value = null
  }
}, { immediate: true })

// 清除错误
const clearError = () => {
  error.value = null
  // 触发自定义事件，通知父组件错误已被清除
  emit('clear')
}

/* eslint-disable no-undef */
const emit = defineEmits(['clear'])
/* eslint-enable no-undef */
</script>

<style scoped>
.error-container {
  margin: 10px 0;
}
</style>