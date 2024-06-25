import axios from 'axios'
//import { useRouter } from 'vue-router'
import { ElMessage, ElLoading } from 'element-plus'

//http请求参数
const httpConfig = {
	isRefreshing: false, //是否正在刷新的标记
	loadingInstance: null, //加载效果实例
	requests: [] //重试队列，每一项将是一个待执行的函数形式
}
/*
*请求封装
*参数示例 useHttp({method:'get',url:'auth/login',data:...})
*@params {method,url,data,loading,beforeSend,progress,success,successMsg,error,complete}
*/
export const useHttp = (params) => {
	//获取配置变量
	const router = useRouter()
	const runConfig = useRuntimeConfig()
	//获取token
	const tokens = useToken().token
	//获取axios实例
	const instance = axios.create()
	instance.defaults.baseURL = runConfig.public.baseURL
	instance.defaults.timeout = runConfig.public.timeout
	instance.defaults.headers = {'content-type': 'application/json'}
	//如果有token带上
	if(tokens && tokens.accessToken) {
		instance.defaults.headers.Authorization = `Bearer ${tokens.accessToken}`
	}
	//标准化参数
	params.method = params.method ? params.method.toLowerCase() : 'get'
	
	//axios响应拦截器
	instance.interceptors.response.use(response => response,
	async error => {
		const resp = error.response
		const originalRequest = error.config
		
		if (resp?.status === 401 && resp?.headers['token-expired']) {
			//如果正在刷新，进入队列
			if (!httpConfig.isRefreshing) {
				const refreshConfig = error.config
				httpConfig.isRefreshing = true
				
				return new Promise((resolve, reject) => {
					axios({
						method: 'post',
						url: `${runConfig.public.baseURL}/auth/retoken`,
						headers: {'content-type': 'application/json'},
						data: {'refreshToken': tokens.refreshToken}
					})
					.then(async(res) => {
						useToken().set(res.data.accessToken, res.data.refreshToken) //存储Token
						const newToken = res.data.accessToken
						refreshConfig.headers.Authorization = `Bearer ${newToken}`
						
						//重新发送队列中的请求
						httpConfig.requests.forEach((cb) => cb(newToken))
						httpConfig.requests = []
						
						resolve(axios(refreshConfig)) //直接用axios实例
					}).catch(async(err) => {
						//通知队列状态
						httpConfig.requests.forEach((cb) => cb(null))
						httpConfig.requests = []
						//刷新token失败，跳转到登录页面
						process.client && ElMessage.error('认证失败，请重新登录')
						await router.push('/account/login')
						reject(err)
					}).finally(() => {
						httpConfig.isRefreshing = false
					})
				})
			} else {
				return new Promise((resolve, reject) => {
					httpConfig.requests.push((newToken) => {
						if(newToken) {
							originalRequest.headers.Authorization = `Bearer ${newToken}`
							resolve(axios(originalRequest)) //直接用axios实例
						} else {
							reject(error)
						}
					})
				})
			}
		}
		//401没有权限或403刷新token失败
		else if(resp?.status === 401 || resp?.status === 403) {
			//process.client && ElMessage.warning('登录失效，请重新登录')
			useToken().remove()
			await router.push('/account/login')
		}
		
		return Promise.reject(error)
	})
	
	//成功回调
	const handleSuccess = (res) => {
		const message = {
			status: res.status,
			contentType: res.headers['content-type'],
			pagination: res.headers['x-pagination'] ? JSON.parse(res.headers['x-pagination']) : {},
			data: res.data
		}
		//成功消息(服务端禁用)
		if (process.client && params.successMsg) {
			ElMessage.success(params.successMsg)
		}
		if(params.success) {
			params.success(message)
		}
		return message
	}
	
	//错误回调
	const handleError = (error) => {
		const resp = error.response
		const message = {
			status: resp?.status,
			message: resp?.data?.message ? resp?.data?.message : error.message,
			data: null
		}
		//网络错误(服务端禁用)
		if (!resp) {
			process.client && ElMessage.error(`语法错误：${error}`)
		}
		//400:响应错误(服务端禁用)
		else if (resp.status === 400) {
			process.client && ElMessage.warning(message.message)
		}
		//415:请求方式有误(服务端禁用)
		else if (resp.status === 415) {
			process.client && ElMessage.warning("请求类型有误，请检查重试")
		}
		//422:数据验证未通过(服务端禁用)
		else if (resp.status === 422) {
			process.client && ElMessage.warning('数据验证未通过，请检查重试')
		}
		//500:响应错误(服务端禁用)
		else if (resp.status === 500) {
			process.client && ElMessage.error('服务器通讯异常，请稍候再试')
		}
		
		//错误回调
		if(params.error) params.error(message)
		
		return message
	}
	
	//抛出实例调用
	return new Promise((resolve, reject) => {
		let config = {}
		config.url = params.url
		config.method = params.method
		//上传文件
		if(params.method === 'file') {
			config.method = 'post'
			config.headers = { 'content-type': 'multipart/form-data' }
			config.onUploadProgress = params.progress
		}
		if(params.data) config.data = params.data
		
		//加载ElLoading(服务端禁用)
		if(process.client && params.loading && !httpConfig.loadingInstance) {
			httpConfig.loadingInstance = ElLoading.service({ body: true, lock: true })
		}
		//请求前调用
		if(params.beforeSend) params.beforeSend()
		//发送请求
		instance.request(config).then(res => {
			//成功回调
			resolve(handleSuccess(res))
		}).catch(error => {
			//错误回调
			reject(handleError(error))
		}).finally(() => {
			//完成调用
			if(params.complete) params.complete()
			if (httpConfig.loadingInstance) httpConfig.loadingInstance.close()
		})
	})
}