export default defineNuxtRouteMiddleware((to, from) => {
	//检查URL是否有ref参数
	if(to.query.ref && parseInt(to.query.ref)) {
		useState('dtcms_web_ref', () => to.query.ref)
	}
	//检查是否需要登录
	if(to.meta.auth) {
		let tokens = useToken().token
		if(!tokens || !tokens.accessToken) {
			console.log('未登录，跳转到login页面', to.fullPath)
			return navigateTo({
				path: '/account/login',
				query: {
					url: encodeURI(to.fullPath),
				}
			})
		}
	}
})