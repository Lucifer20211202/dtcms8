import router from "../router"

//VUE公共方法
export default {
	//跳转链接
	linkUrl(...args) {
		if(args.length == 2) {
			router.push({path: args[0], query: args[1]})
		} else {
			router.push({path: args[0]})
		}
	},
	//返回上一页
	back() {
		router.back()
	},
}