module.exports = {
    root: true,
    env: {
        browser: true,
        node: true,
        es2021: true,
    },
    parser: 'vue-eslint-parser',
    parserOptions: {
        parser: 'espree', // 默认 JavaScript 解析器
        ecmaVersion: 2021,
        sourceType: 'module',
    },
    extends: [
        'eslint:recommended',
        'plugin:vue/vue3-recommended',
    ],
    plugins: ['vue'],
    rules: {
        // 根据你个人喜好调整规则
        'vue/multi-word-component-names': 'off', // 允许单文件组件名是单词
        'no-unused-vars': 'warn',
        'no-console': 'off',
        'vue/max-attributes-per-line': ['warn', {
            singleline: {
                max: 3
            },  // 单行最多允许 5 个属性
            multiline: {
                max: 3
            }
        }],
    },
};
