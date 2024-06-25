import axios from 'axios'
import {useUserStore} from '../stores/userStore.js'
import {ElMessage,ElLoading} from 'element-plus'
import config from '/public/config.js'

//请求配置属性
const httpConfig = {
	baseApi: config.baseApi,
	tokenKey: 'dtcms_admin_token', //Token存取的key
	isRefreshing: false, //是否正在刷新的标记
	requests: [] //重试队列，每一项将是一个待执行的函数形式
}

//获取Token
const getToken = () => {
	let tokenObj = JSON.parse(window.localStorage.getItem(httpConfig.tokenKey))
	return tokenObj ? tokenObj : {}
}
//存入Token
const setToken = (accessToken, refreshToken) => {
	let token = { 'accessToken' : accessToken, 'refreshToken' : refreshToken }
	window.localStorage.setItem(httpConfig.tokenKey, JSON.stringify(token))
}
//删除Token
const removeToken = () => {
	window.localStorage.removeItem(httpConfig.tokenKey)
}
//刷新Token
const refreshToken = (error) => {
	const store = useUserStore() //引用Store
	const config = error.response.config //记住请求参数设置
	let reToken = getToken().refreshToken
	
	if(!reToken) {
		store.logout()
		return Promise.reject(error)
	}
	if (!httpConfig.isRefreshing) {
		httpConfig.isRefreshing = true
		return new Promise((resolve, reject) => {
			instance.post('/auth/retoken', {'refreshToken': reToken}).then(res => {
				config.headers.Authorization = `Bearer ${res.data.accessToken}` //设置请求头
				setToken(res.data.accessToken, res.data.refreshToken) //存储Token
				//已经刷新了token，将所有队列中的请求进行重试
				httpConfig.requests.forEach(cb => cb(res.data.accessToken))
				//重试完了清空这个队列
				httpConfig.requests = []
				resolve(axios(config))
			}).catch(err => {
				//console.log(`获取Token失败:${err}`)
				reject(err)
			}).finally(() => {
				httpConfig.isRefreshing = false
			})
		})
	} else {
		//正在刷新token，返回一个未执行resolve的promise
		return new Promise((resolve) => {
			httpConfig.requests.push((accessToken) => {
				config.headers.Authorization = `Bearer ${accessToken}`;
				resolve(axios(config));
			})
		})
	}
}

//创建一个axios实例
const instance = axios.create({
	baseURL: httpConfig.baseApi,
	timeout: 200000,
	headers: {'Content-Type': 'application/json'}
})
//给实例添加请求拦截器
instance.interceptors.request.use(config => {
	const accessToken = getToken().accessToken
	if (accessToken) {
		config.headers.Authorization = `Bearer ${accessToken}`
	}
	return config
}, error => {
	return Promise.reject(error);
})
//给实例添加响应拦截器
instance.interceptors.response.use(response => {
	return response;
}, error => {
	let resp = error.response
	if (resp && resp.status === 401 && resp.headers['token-expired']) {
		return refreshToken(error)
	}
	return Promise.reject(error)
});

export default{
	//API地址
	baseApi: httpConfig.baseApi,
	//获取Token
	getToken,
	//存入Token
	setToken,
	//删除Token
	removeToken,
	//转换成绝对地址
	loadFile: (url) => {
		if (!url) return
		if (!url.startsWith('/')) return url
		return httpConfig.baseApi + url
	},
	//将编辑器图片视频转换成绝对地址
	loadEditor: (val) => {
		if (!val) return
		let newVal = val.replace(/<img [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (!capture.toLowerCase().startsWith('http')) {
				return match.replace(capture, httpConfig.baseApi + capture)
			}
			return match
		});
		newVal = newVal.replace(/<source [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (!capture.toLowerCase().startsWith('http')) {
				return match.replace(capture, httpConfig.baseApi + capture)
			}
			return match
		});
		return newVal
	},
	//将编辑器图片视频转换成相对地址
	replaceEditor: (val) => {
		if (!val) return
		let newVal = val.replace(/<img [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (capture.indexOf(httpConfig.baseApi) != -1) {
				return match.replace(httpConfig.baseApi, '')
			}
			return match;
		});
		newVal = newVal.replace(/<source [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (capture.indexOf(httpConfig.baseApi) != -1) {
				return match.replace(httpConfig.baseApi, '')
			}
			return match
		});
		return newVal
	},
	/*封装axios请求
	* 参数示例
	* request({method:'get',url:'auth/login',data:...});
	* @param {method,url,data,loading,beforeSend,progress,success,successMsg,error,complete,completeMsg}
	*/
	request: (params) => {
		const store = useUserStore() //引用Store
		//加载ElLoading
		let loadingInstance = params.loading ? ElLoading.service({ body: true, lock: true }) : null
		//发送请求前调用
		if (params.beforeSend) params.beforeSend()
		//定义参数配置
		let method = params.method?params.method.toLowerCase():null,
			url = params.url,
			callback = (res) => {
				if (loadingInstance) loadingInstance.close()
				if (!res.status) {
					ElMessage.error("请求失败，请重试...")
				}
				//成功
				else if (res.status === 200 || res.status === 204) {
					if (params.successMsg) {
						ElMessage.success(params.successMsg)
					}
					if (params.success) params.success(res)
				}
				//其它
				else {
					ElMessage.error(res.data.message)
				}
				if (params.complete) params.complete()
			},
			handleError = (err) => {
				if (loadingInstance) loadingInstance.close()
				//网络错误
				if (!err.response) {
					ElMessage.error('语法错误：' + err)
				}
				//401:没有权限
				else if (err.response.status === 401 && err.response.data.code === 405) {
					ElMessage.warning("请求失败，没有操作权限")
				}
				//401:未登录
				else if (err.response.status === 401 && !err.response.data) {
					//注销及跳转登录页
					store.logout()
				}
				//403:刷新TOKEN失败
				else if (err.response.status === 403) {
					//注销及跳转登录页
					store.logout()
				}
				//422:数据验证未通过
				else if (err.response.status === 422) {
					ElMessage.warning(err.response.data.message[0][0])
				}
				//其它错误
				else {
					console.log(err.message)
					let message = err.message
					if(typeof (err.response.data) != "undefined" && err.response.data.message) {
						message = err.response.data.message
					}
					ElMessage.error(message)
				}
				//执行完成之后的回调函数
				if (params.error) params.error(err)
				if (params.complete) params.complete(err)
			}
		//封装Promise
		return new Promise((resolve, reject) => {
			let config = {}
			if (method === "file") {
				config.headers = {
					"Content-Type": "multipart/form-data",
				}
			}
			if (!method || method === "get") {
				if (params.data) {
					instance.get(url, params.data).then(res => {
						resolve(callback(res))
					}).catch(handleError)
				} else {
					instance.get(url).then(res => {
						resolve(callback(res))
					}).catch(handleError)
				}
			} else {
				if (method === "post" || method === "delete" || method === "put" || method === "patch") {
					instance[method](url, params.data).then(res => {
						resolve(callback(res))
					}).catch(handleError)
				} else if (method === "file") {
					if (params.progress) {
						instance.post(url, params.data, {...config, onUploadProgress: params.progress}).then(res => {
							resolve(callback(res))
						}).catch(handleError)
					} else {
						instance.post(url, params.data, config).then(res => {
							resolve(callback(res))
						}).catch(handleError)
					}
				} else if (method === "down") {
					instance.get(url, { responseType: 'blob' }).then(res => {
						resolve(callback(res))
					}).catch(handleError)
				}
			}
		})
	}
}