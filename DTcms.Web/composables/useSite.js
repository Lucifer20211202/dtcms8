//获取当前站点信息
export const useSite = async(type) => {
	//当前站点信息
	const siteState = async() => {
		const state = useState('dtcms_web_site')
		if(!state.value) {
			let host = ''
			if(process.server) {
				const event = useRequestEvent()
				host = event.req.headers.host
			} else {
				host = window.location.host
			}
			const { data:res } = await useAsyncData('site', () => useHttp({url: `/client/site/domain?host=${host}`}))
			if(res.value) {
				state.value =  res.value.data
			} else {
				throw createError({
					statusCode: 500,
					statusMessage: '该站点无法获取到匹配的域名，请刷新页面或稍候重试。'
				})
			}
		}
		return state.value
	}
	
	//上传配置信息
	const uploadState = async() => {
		const state = useState('dtcms_web_upload')
		if(!state.value) {
			const { data:res } = await useAsyncData('upload', () => useHttp({url: `/client/setting/uploadconfig`}))
			if(res.value) {
				state.value =  res.value.data
			} else {
				throw createError({
					statusCode: 500,
					statusMessage: '无法找到上传配置信息，请联系管理员。'
				})
			}
		}
		return state.value
	}
	
	//会员配置信息
	const memberState = async() => {
		const state = useState('dtcms_web_member')
		if(!state.value) {
			const { data:res } = await useAsyncData('member', () => useHttp({url: `/client/setting/memberconfig`}))
			if(res.value) {
				state.value =  res.value.data
			} else {
				throw createError({
					statusCode: 500,
					statusMessage: '无法找到会员配置信息，请联系管理员。'
				})
			}
		}
		return state.value
	}
	
	//系统配置信息
	const configState = async() => {
		const state = useState('dtcms_web_config')
		if(!state.value) {
			const { data:res } = await useAsyncData('config', () => useHttp({url: `/client/setting/sysconfig`}))
			if(res.value) {
				state.value =  res.value.data
			} else {
				throw createError({
					statusCode: 500,
					statusMessage: '无法找到系统配置信息，请联系管理员。'
				})
			}
		}
		return state.value
	}
	
	switch(type) {
		case 'site': 
			return await siteState()
			break
		case 'upload':
			return await uploadState()
			break
		case 'member':
			return await memberState()
			break
		default:
			return await configState()
	}
}