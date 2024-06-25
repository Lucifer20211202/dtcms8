//公共方法
export default {
	//转换成绝对地址
	loadFile: (url) => {
		if (!url) return
		if (!url.startsWith('/')) return url
		const runConfig = useRuntimeConfig()
		return runConfig.public.baseURL + url
	},
	//将图片视频转换成绝对地址
	replaceHtml: (val) => {
		if (!val) return
		const runConfig = useRuntimeConfig()
		let newVal = val.replace(/<img [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (!capture.toLowerCase().startsWith('http')) {
				return match.replace(capture, runConfig.public.baseURL + capture)
			}
			return match
		})
		newVal = newVal.replace(/<source [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (!capture.toLowerCase().startsWith('http')) {
				return match.replace(capture, runConfig.public.baseURL + capture)
			}
			return match
		})
		return newVal
	},
	//将图片视频转换成相对地址
	replaceEditor: (val) => {
		if (!val) return
		const runConfig = useRuntimeConfig()
		let newVal = val.replace(/<img [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (capture.indexOf(runConfig.public.baseURL) != -1) {
				return match.replace(runConfig.public.baseURL, '')
			}
			return match;
		});
		newVal = newVal.replace(/<source [^>]*src=['"]([^'"]+)[^>]*>/gi, (match, capture) => {
			if (capture.indexOf(runConfig.public.baseURL) != -1) {
				return match.replace(runConfig.public.baseURL, '')
			}
			return match
		});
		return newVal
	},
	//格式化字符串
	formatString: (template, ...args) => {
		return template.replace(/{(\d+)}/g, (match, index) => {
			return typeof args[index] !== 'undefined' ? args[index] : match
		})
	},
	//将字符串转换成日期时间
	convertDateTime: (dateString) => {
		return new Date(dateString).getTime()
	},
	//检查拼接GET参数
	appendParams: (baseUrl, param) => {
		if (!baseUrl) return baseUrl
		if (typeof param !== 'string' || param.trim() === '') return baseUrl
		if (baseUrl.indexOf('?') === -1) {
			return baseUrl + '?' + param
		} else {
			return baseUrl + '&' + param
		}
	},
	//删除HTML并返回指定长度
	clearHtml: (str, length) => {
		if(!str) return
		str = str.replace(/(\n)/g, "");
		str = str.replace(/(\t)/g, "");
		str = str.replace(/(\r)/g, "");
		str = str.replace(/<\/?[^>]*>/g, "");
		str = str.replace(/\s*/g, "");
	
		// 如果超出指定长度，则截取并使用省略号
		if(str.length >= length){
			var text = str.substr(0, length) + '...'
			return text
		} else {
			return str
		}
	},
	//跳转链接
	linkUrl: (...args) => {
		const router = useRouter()
		if(args.length == 2) {
			router.push({path: args[0], query: args[1]})
		} else {
			router.push({path: args[0]})
		}
	},
	//返回上一页
	back: (num) => {
		const router = useRouter()
		router.go(num)
	}
}