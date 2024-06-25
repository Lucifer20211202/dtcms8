import {createRouter,createWebHashHistory} from "vue-router"
import {useUserStore} from "../stores/userStore.js"
import config from './config.js'

//检查是否重复跳转
const originalPush = createRouter.prototype.push
createRouter.prototype.push = function push(location) {
	return originalPush.call(this, location).catch(err => err)
}

//创建路由实例
const router = createRouter({
  history: createWebHashHistory(),
  routes: config.routes
})

//全局路由守卫
router.beforeEach((to, from, next) => {
	const store = useUserStore()
	//设置页面标题
	if (to.meta.title) {
		document.title = to.meta.title
	}
	// 检测路由配置中是否有requiresAuth这个meta属性
	if (to.matched.some(record => record.meta.requiresAuth)) {
		// 判断是否已登录
		if (store.isLoggedIn) {
			next()
			return
		}
		// 未登录则跳转到登录界面
		next('/login')
	} else {
		next()
	}
});

export default router