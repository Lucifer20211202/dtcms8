import {createApp} from 'vue'
import store from './stores/index.js'
import router from './router/index.js'
import http from './request/index.js'
import common from './utils/common.js'
import ElementPlus from 'element-plus'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
import * as ElIcon from '@element-plus/icons-vue'
import 'element-plus/dist/index.css'
import './assets/iconfont/iconfont.css'
import './assets/scss/style.scss'
import App from './App.vue'
import DtLocation from "./components/layout/DtLocation.vue"

const app = createApp(App)
app.use(store)
app.use(router)
//全局引入El图标
Object.keys(ElIcon).forEach((key) => {
  app.component(key, ElIcon[key])
})
app.use(ElementPlus, { locale: zhCn })
app.config.globalProperties.$api = http
app.config.globalProperties.$common = common
//全局注册自定义组件
app.component('DtLocation', DtLocation)
app.mount('#app')

